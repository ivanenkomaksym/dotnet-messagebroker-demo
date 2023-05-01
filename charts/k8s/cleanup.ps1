kubectl delete -f app_configmap.yaml

kubectl delete -f notifications.yaml
kubectl delete -f orderapi.yaml
kubectl delete -f orderprocessor.yaml
kubectl delete -f warehouse.yaml
kubectl delete -f customerapi.yaml

kubectl delete -f rabbitmq.yaml
kubectl delete -f rabbitmq_service.yaml

kubectl delete -f orderapi_service.yaml
kubectl delete -f customerapi_service.yaml