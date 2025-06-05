dotnet publish Colorado/Colorado.csproj -r linux-x64 -p:PublishSingleFile=true --self-contained false
cp Colorado/bin/Release/net9.0/linux-x64/publish/Colorado dist/
