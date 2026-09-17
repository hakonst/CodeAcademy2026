{ pkgs ? import <nixpkgs> { } }:

let
  node = pkgs.nodejs_24;

  corepack = pkgs.writeShellScriptBin "corepack" ''
    dir="''${COREPACK_HOME:-$HOME/.cache/corepack}/bin"
    mkdir -p "$dir"
    case "''${1-}" in
      enable | disable)
        cmd="$1"
        shift
        exec ${node}/bin/corepack "$cmd" --install-directory "$dir" "$@"
        ;;
    esac
    exec ${node}/bin/corepack "$@"
  '';
in
pkgs.mkShell {
  packages = [ corepack node ];

  shellHook = ''
    export COREPACK_HOME="$HOME/.cache/corepack"
    export COREPACK_ENABLE_DOWNLOAD_PROMPT=0
    corepack enable yarn
    export PATH="$COREPACK_HOME/bin:$PATH"
  '';
}
