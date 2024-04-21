cd charts\k8s
.\cleanup.ps1

# RabbitMQ
kubectl apply -f rabbitmq.yaml
kubectl apply -f rabbitmq-service.yaml

# Mongo
kubectl apply -f mongo-secret.yaml
kubectl apply -f mongo-deployment.yaml
kubectl apply -f mongo-service.yaml

# Jaeger
kubectl apply -f jaeger.yaml
kubectl apply -f jaeger-service.yaml

# CatalogAPI Stub

# CustomerAPI Stub

# Notifications

# OcelotApiGateway
kubectl apply -f ocelot-configmap.yaml
kubectl apply -f ocelotapigateway.yaml
kubectl apply -f ocelotapigateway-service.yaml

# rorderapi
kubectl apply -f rorderapi.yaml
kubectl apply -f rorderapi-service.yaml

# OrderGRPC
kubectl apply -f ordergrpc.yaml
kubectl apply -f ordergrpc-service.yaml

# OrderProcessor
kubectl apply -f orderprocessor.yaml

# Payment
kubectl apply -f payment.yaml

# Shipment
kubectl apply -f shipment.yaml

# ShoppingCartAPI Stub

# Warehouse
kubectl apply -f warehouse.yaml

# WarehouseAPI Stub

# WebUI
kubectl apply -f webui_with_stubs.yaml
kubectl apply -f webui-service.yaml

# WebUIAggregatorAPI Stub

# FeedbackAPI Empty

# DiscountAPI Empty

minikube dashboard
Start-Process -FilePath "cmd" -ArgumentList "/c minikube service shopdb" -WorkingDirectory ".\"
Start-Process -FilePath "cmd" -ArgumentList "/c minikube service webui" -WorkingDirectory ".\"
cd ..\..\