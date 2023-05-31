Start-Process -FilePath "cmd" -ArgumentList "/c docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.11-management" -WorkingDirectory ".\"
Start-Process -FilePath "cmd" -ArgumentList "/c docker run -it --rm --name mongo -p 27017:27017 mongo:latest" -WorkingDirectory ".\"
Start-Sleep -Seconds 15
Start-Process -FilePath "cmd" -ArgumentList "/c dotnet run" -WorkingDirectory ".\Notifications"
Start-Process -FilePath "cmd" -ArgumentList "/c dotnet run" -WorkingDirectory ".\OrderAPI"
Start-Process -FilePath "cmd" -ArgumentList "/c dotnet run" -WorkingDirectory ".\OrderProcessor"
Start-Process -FilePath "cmd" -ArgumentList "/c dotnet run" -WorkingDirectory ".\Warehouse"
Start-Process -FilePath "cmd" -ArgumentList "/c dotnet run" -WorkingDirectory ".\CustomerAPI"
Start-Process -FilePath "cmd" -ArgumentList "/c dotnet run" -WorkingDirectory ".\OcelotAPIGateway"