@echo off

cd /D ../Valuator
echo Starting web application on port 5001...
start "" dotnet run --urls "http://127.0.0.1:5001"

echo Starting another web application on port 5002...
start "" dotnet run --urls "http://127.0.0.1:5002"

echo Starting Nginx proxy server...
start "" /D "../nginx" nginx.exe
это start.bat