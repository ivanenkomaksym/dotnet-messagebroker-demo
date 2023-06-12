cd charts\k8s
.\cleanup.ps1

# RabbitMQ
kubectl apply -f rabbitmq.yaml
kubectl apply -f rabbitmq-service.yaml

# Mongo
kubectl apply -f mongo-secret.yaml
kubectl apply -f mongo-deployment.yaml
kubectl apply -f mongo-service.yaml

# CatalogAPI
kubectl apply -f catalogapi.yaml
kubectl apply -f catalogapi-service.yaml

# CustomerAPI
kubectl apply -f customerapi.yaml
kubectl apply -f customerapi-service.yaml

# Notifications
kubectl apply -f notifications.yaml

# OcelotApiGateway
kubectl apply -f ocelot-configmap.yaml
kubectl apply -f ocelotapigateway.yaml
kubectl apply -f ocelotapigateway-service.yaml

# OrderAPI
kubectl apply -f orderapi.yaml
kubectl apply -f orderapi-service.yaml

# OrderGRPC
kubectl apply -f ordergrpc.yaml
kubectl apply -f ordergrpc-service.yaml

# OrderProcessor
kubectl apply -f orderprocessor.yaml

# Payment
kubectl apply -f payment.yaml

# Shipment
kubectl apply -f shipment.yaml

# ShoppingCartAPI
kubectl apply -f shoppingcartapi.yaml
kubectl apply -f shoppingcartapi-service.yaml

# Warehouse
kubectl apply -f warehouse.yaml

# WarehouseAPI
kubectl apply -f warehouseapi.yaml
kubectl apply -f warehouseapi-service.yaml

# WebUI
kubectl apply -f webui.yaml
kubectl apply -f webui-service.yaml

# WebUIAggregatorAPI
kubectl apply -f webuiaggregatorapi.yaml
kubectl apply -f webuiaggregatorapi-service.yaml

minikube dashboard
Start-Process -FilePath "cmd" -ArgumentList "/c minikube service shopdb" -WorkingDirectory ".\"
Start-Process -FilePath "cmd" -ArgumentList "/c minikube service webui" -WorkingDirectory ".\"
cd ..\..\