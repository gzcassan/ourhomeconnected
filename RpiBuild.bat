del /q "C:\ohc\OHC-build\*"
FOR /D %%p IN ("C:\ohc\OHC-build\*.*") DO rmdir "%%p" /s /q
dotnet restore
dotnet publish --self-contained --runtime ubuntu.16.04-arm --output c:\ohc\OHC-build