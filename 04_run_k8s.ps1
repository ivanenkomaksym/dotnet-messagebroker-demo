cd charts\k8s
.\cleanup.ps1

kubectl apply -f app_configmap.yaml
kubectl apply -f notifications.yaml
kubectl apply -f orderapi.yaml
kubectl apply -f orderprocessor.yaml
kubectl apply -f warehouse.yaml
kubectl apply -f customerapi.yaml

kubectl apply -f rabbitmq.yaml
kubectl apply -f rabbitmq_service.yaml

kubectl apply -f orderapi_service.yaml
kubectl apply -f customerapi_service.yaml

minikube dashboard
Start-Process -FilePath "cmd" -ArgumentList "/c minikube service orderapi-service" -WorkingDirectory ".\"
Start-Process -FilePath "cmd" -ArgumentList "/c minikube service customerapi-service" -WorkingDirectory ".\"
Start-Process -FilePath "cmd" -ArgumentList "/c minikube service rabbitmq-service" -WorkingDirectory ".\"
cd ..\..\