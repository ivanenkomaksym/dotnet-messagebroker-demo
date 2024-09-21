helm upgrade --install -f .\charts\helmapp\values_minimal.yaml helmapp .\charts\helmapp
minikube dashboard
Start-Process -FilePath "cmd" -ArgumentList "/c minikube service webui" -WorkingDirectory ".\"
# or port forward with
# kubectl port-forward svc/webui 8080:80