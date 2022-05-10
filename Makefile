# To learn makefiles: https://makefiletutorial.com/
# On windows, use NMake: https://docs.microsoft.com/pt-br/cpp/build/reference/nmake-reference?view=msvc-160
dotnetFramework = net6.0
solution = ./enki.token.sln
distPath = ./dist

run-clean: clean restore build run

all : clean restore build

clean:
	dotnet clean ${solution}

restore:
	dotnet restore ${solution}

build:
	dotnet build -c Release ${solution}

# Mais em: https://renatogroffe.medium.com/net-nuget-atualizando-packages-via-linha-de-comando-b0c6b596ed2
# Para instalar dependência: dotnet tool install --global dotnet-outdated-tool
update-dependencies:
	ifeq (, $(shell which dotnet-outdated))
		$(error "No dotnet-outdated in PATH, consider doing `dotnet tool install --global dotnet-outdated-tool`")
	else
		dotnet-outdated -u:Prompt ${solution}
	endif

# Mais em: https://devblogs.microsoft.com/nuget/how-to-scan-nuget-packages-for-security-vulnerabilities/
check-vulnerabilities:
	dotnet list package --vulnerable

test-all:
	dotnet test ${solution}

publish-cli:
	dotnet publish ./src/totp.cli/totp.cli.csproj --self-contained --runtime linux-x64 -o ${distPath}/totp.cli

publish-console:
	dotnet publish ./src/totp.console/totp.console.csproj --self-contained --runtime linux-x64 -o ${distPath}/totp.console

publish-gtksharp:
	dotnet publish ./src/totp.gtksharp/totp.gtksharp.csproj --self-contained --runtime linux-x64 -o ${distPath}/totp.gtk

publish-uiterm:
	dotnet publish ./src/totp.uiterm/totp.uiterm.csproj --self-contained --runtime linux-x64 -o ${distPath}/totp.uiterm