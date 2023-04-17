minikube start
minikube docker-env
minikube -p minikube docker-env --shell powershell | Invoke-Expression

kubectl apply -f notifications.yaml
kubectl apply -f orderapi.yaml
kubectl apply -f orderprocessor.yaml
kubectl apply -f warehouse.yaml

kubectl apply -f notifications_service.yaml
kubectl apply -f orderapi_service.yaml
kubectl apply -f orderprocessor_service.yaml
kubectl apply -f warehouse_service.yaml

minikube service orderapi-service