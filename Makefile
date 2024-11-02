build:
	dotnet build && cp "bin/Debug/netstandard2.1/VRCrypt.dll" "out/VRCrypt.dll"
link:
	sh scripts/link.sh
