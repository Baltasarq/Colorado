dotnet build -c:Release Colorado
ilrepack /out:dist/Colorado.dll Colorado/bin/Release/net10.0/Colorado.dll Colorado/bin/Release/net10.0/*.dll
