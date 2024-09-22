kubectl create namespace argocd-eshop-minimal
kubectl apply -f .\charts\argocdapp\eshop_minimal.yaml
Start-Process -FilePath "cmd" -ArgumentList "/c kubectl port-forward svc/argocd-server -n argocd 8080:443"
Start-Process "https://localhost:8080"
Start-Process -FilePath "cmd" -ArgumentList "/c minikube -n argocd-eshop-minimal service webui" -WorkingDirectory ".\"