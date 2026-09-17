import { act, renderHook } from "@testing-library/react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { useEventSource } from "@/hooks/useEventSource";

class FakeEventSource {
  static instances: FakeEventSource[] = [];
  static readonly CONNECTING = 0;
  static readonly OPEN = 1;
  static readonly CLOSED = 2;

  readyState = FakeEventSource.CONNECTING;
  closed = false;
  private listeners = new Map<string, Set<(event: Event) => void>>();

  constructor(public url: string) {
    FakeEventSource.instances.push(this);
  }

  addEventListener(type: string, fn: (event: Event) => void) {
    if (!this.listeners.has(type)) this.listeners.set(type, new Set());
    this.listeners.get(type)!.add(fn);
  }

  removeEventListener(type: string, fn: (event: Event) => void) {
    this.listeners.get(type)?.delete(fn);
  }

  close() {
    this.closed = true;
    this.readyState = FakeEventSource.CLOSED;
  }

  emit(type: string, data?: unknown) {
    const event = Object.assign(new Event(type), { data: JSON.stringify(data ?? {}) });
    for (const fn of this.listeners.get(type) ?? []) fn(event);
  }
}

beforeEach(() => {
  FakeEventSource.instances = [];
  vi.stubGlobal("EventSource", FakeEventSource);
});

afterEach(() => {
  vi.unstubAllGlobals();
});

const only = () => FakeEventSource.instances[0];

describe("useEventSource", () => {
  it("åpner én forbindelse mot url og starter i connecting", () => {
    const { result } = renderHook(() => useEventSource("/api/events/demo", () => {}));

    expect(FakeEventSource.instances).toHaveLength(1);
    expect(only().url).toBe("/api/events/demo");
    expect(result.current).toBe("connecting");
  });

  it("slår om til open på ready, og ber om resynk", () => {
    const onEvent = vi.fn();
    const { result } = renderHook(() => useEventSource("/api/events/demo", onEvent));

    act(() => only().emit("ready", { connectedAt: "nå" }));

    expect(result.current).toBe("open");
    expect(onEvent).toHaveBeenCalledTimes(1);
  });

  it("videresender ordrehendelser", () => {
    const onEvent = vi.fn();
    renderHook(() => useEventSource("/api/events/demo", onEvent));

    act(() => only().emit("order.status-changed", { status: "BREWING" }));
    act(() => only().emit("order.created", { status: "PENDING" }));

    expect(onEvent).toHaveBeenCalledTimes(2);
  });

  it("kobler IKKE opp på nytt når onEvent er en ny funksjon per render", () => {
    const { rerender } = renderHook(({ fn }) => useEventSource("/api/events/demo", fn), {
      initialProps: { fn: () => {} },
    });

    rerender({ fn: () => {} });
    rerender({ fn: () => {} });

    expect(FakeEventSource.instances).toHaveLength(1);
    expect(only().closed).toBe(false);
  });

  it("bruker den ferskeste onEvent, ikke den fra første render", () => {
    const first = vi.fn();
    const second = vi.fn();
    const { rerender } = renderHook(({ fn }) => useEventSource("/api/events/demo", fn), {
      initialProps: { fn: first },
    });

    rerender({ fn: second });
    act(() => only().emit("order.status-changed"));

    expect(first).not.toHaveBeenCalled();
    expect(second).toHaveBeenCalledTimes(1);
  });

  it("sier closed når EventSource har gitt opp, connecting når den prøver igjen", () => {
    const { result } = renderHook(() => useEventSource("/api/events/demo", () => {}));

    act(() => only().emit("ready"));
    expect(result.current).toBe("open");

    act(() => {
      only().readyState = FakeEventSource.CONNECTING;
      only().emit("error");
    });
    expect(result.current).toBe("connecting");

    act(() => {
      only().readyState = FakeEventSource.CLOSED;
      only().emit("error");
    });
    expect(result.current).toBe("closed");
  });

  it("lukker forbindelsen når komponenten forsvinner", () => {
    const { unmount } = renderHook(() => useEventSource("/api/events/demo", () => {}));

    const source = only();
    unmount();

    expect(source.closed).toBe(true);
  });

  it("bygger ny forbindelse når url endres", () => {
    const { rerender } = renderHook(({ url }) => useEventSource(url, () => {}), {
      initialProps: { url: "/api/events/demo" },
    });

    rerender({ url: "/api/events" });

    expect(FakeEventSource.instances).toHaveLength(2);
    expect(FakeEventSource.instances[0].closed).toBe(true);
    expect(FakeEventSource.instances[1].url).toBe("/api/events");
  });
});
