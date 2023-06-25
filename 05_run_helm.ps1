helm upgrade --install helmapp .\charts\helmapp
minikube dashboard
Start-Process -FilePath "cmd" -ArgumentList "/c minikube service webui" -WorkingDirectory ".\"