@echo off

echo Start Nats server
start "" /D "../" nats-server.exe

cd /D ../Valuator
echo Starting web application on port 5001...
start "" dotnet run --urls "http://127.0.0.1:5001"

echo Starting another web application on port 5002...
start "" dotnet run --urls "http://127.0.0.1:5002"

cd ../RankCalculator
start "" dotnet run

cd ../EventsLogger
start "" dotnet run --urls "http://127.0.0.1:5003"
start "" dotnet run --urls "http://127.0.0.1:5004"

echo Starting Nginx proxy server...


start "" /D "../nginx" nginx.exe
echo это start.bat