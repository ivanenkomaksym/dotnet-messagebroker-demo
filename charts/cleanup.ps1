kubectl delete -f notifications.yaml
kubectl delete -f orderapi.yaml
kubectl delete -f orderprocessor.yaml
kubectl delete -f warehouse.yaml

kubectl delete -f notifications_service.yaml
kubectl delete -f orderapi_service.yaml
kubectl delete -f orderprocessor_service.yaml
kubectl delete -f warehouse_service.yaml