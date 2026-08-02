{
  description = "enki.totp - TOTP generator";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
  };

  outputs = { self, nixpkgs }:
    let
      system = "x86_64-linux";
      pkgs = nixpkgs.legacyPackages.${system};
    in {
      packages.${system}.totp.uiterm = pkgs.stdenv.mkDerivation {
        pname = "totp.uiterm";
        version = "1.0.0";
        src = pkgs.lib.cleanSource self;
        nativeBuildInputs = with pkgs; [ dotnet-sdk_8 ];
        buildPhase = ''
          dotnet publish src/totp.uiterm/totp.uiterm.csproj \
            --self-contained \
            --runtime linux-x64 \
            -c Release \
            -o ./publish-output
        '';
        installPhase = ''
          mkdir -p $out/bin
          cp -r ./publish-output/* $out/bin/
          mv $out/bin/totp.uiterm $out/bin/.totp.uiterm-bin
          cat > $out/bin/totp.uiterm << 'WRAPPER'
          #!/bin/sh
          cd "$(dirname "$0")"
          exec ./.totp.uiterm-bin "$@"
          WRAPPER
          chmod +x $out/bin/totp.uiterm
        '';
        meta.mainProgram = "totp.uiterm";
      };

      packages.${system}.default = self.packages.${system}.totp.uiterm;

      apps.${system}.default = {
        type = "app";
        program = "${self.packages.${system}.totp.uiterm}/bin/totp.uiterm";
      };

      devShells.${system}.default = pkgs.mkShell {
        nativeBuildInputs = with pkgs; [ dotnet-sdk_8 ];
      };
    };
}