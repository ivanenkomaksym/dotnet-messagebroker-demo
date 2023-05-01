helm upgrade --install helmapp .\charts\helmapp
minikube dashboard
Start-Process -FilePath "cmd" -ArgumentList "/c minikube service helmapp-orderapi" -WorkingDirectory ".\"
Start-Process -FilePath "cmd" -ArgumentList "/c minikube service helmapp-customerapi" -WorkingDirectory ".\"
Start-Process -FilePath "cmd" -ArgumentList "/c minikube service helmapp-ocelotapigateway" -WorkingDirectory ".\"
Start-Process -FilePath "cmd" -ArgumentList "/c minikube service helmapp-rabbitmq" -WorkingDirectory ".\"