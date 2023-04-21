cd charts\k8s
.\cleanup.ps1

kubectl apply -f notifications_configmap.yaml
kubectl apply -f notifications.yaml
kubectl apply -f orderapi_configmap.yaml
kubectl apply -f orderapi.yaml
kubectl apply -f orderprocessor_configmap.yaml
kubectl apply -f orderprocessor.yaml
kubectl apply -f warehouse_configmap.yaml
kubectl apply -f warehouse.yaml

kubectl apply -f rabbitmq.yaml
kubectl apply -f rabbitmq_service.yaml

kubectl apply -f orderapi_service.yaml

minikube dashboard
minikube service orderapi-service