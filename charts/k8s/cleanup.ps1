# RabbitMQ
kubectl delete -f rabbitmq.yaml
kubectl delete -f rabbitmq-service.yaml

# Mongo
kubectl delete -f mongo-secret.yaml
kubectl delete -f mongo-deployment.yaml
kubectl delete -f mongo-service.yaml

# CatalogAPI
kubectl delete -f catalogapi.yaml
kubectl delete -f catalogapi-service.yaml

# CustomerAPI
kubectl delete -f customerapi.yaml
kubectl delete -f customerapi-service.yaml

# Notifications
kubectl delete -f notifications.yaml

# OcelotApiGateway
kubectl delete -f ocelot-configmap.yaml
kubectl delete -f ocelotapigateway.yaml
kubectl delete -f ocelotapigateway-service.yaml

# OrderAPI
kubectl delete -f orderapi.yaml
kubectl delete -f orderapi-service.yaml

# OrderGRPC
kubectl delete -f ordergrpc.yaml
kubectl delete -f ordergrpc-service.yaml

# OrderProcessor
kubectl delete -f orderprocessor.yaml

# Payment
kubectl delete -f payment.yaml

# Shipment
kubectl delete -f shipment.yaml

# ShoppingCartAPI
kubectl delete -f shoppingcartapi.yaml
kubectl delete -f shoppingcartapi-service.yaml

# Warehouse
kubectl delete -f warehouse.yaml

# WarehouseAPI
kubectl delete -f warehouseapi.yaml
kubectl delete -f warehouseapi-service.yaml

# WebUI
kubectl delete -f webui.yaml
kubectl delete -f webui-service.yaml

# WebUIAggregatorAPI
kubectl delete -f webuiaggregatorapi.yaml
kubectl delete -f webuiaggregatorapi-service.yaml

# FeedbackAPI
kubectl delete -f feedbackapi.yaml
kubectl delete -f feedbackapi-service.yaml

# DiscountAPI
kubectl delete -f discount.yaml
kubectl delete -f discount-service.yaml