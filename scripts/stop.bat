@echo off

echo Stopping web application on all ports
taskkill /F /IM dotnet.exe 

echo Stop nats server
taskkill /F /IM nats-server.exe

echo Stopping Nginx proxy server...
taskkill /F /IM nginx.exe

echo All services stopped.