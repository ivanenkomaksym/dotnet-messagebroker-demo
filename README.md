# dotnet-messagebroker-demo

This repository showcases a sample implementation of a message broker using .NET technologies. It provides a demonstration of how to build a distributed messaging system using popular message broker patterns, along with Docker images, Kubernetes deployment, and Helm charts.

## Features

* Demonstrates the use of a message broker for decoupling and asynchronous communication between microservices.
* Implements common messaging patterns such as publish-subscribe, request-reply, and event-driven architecture.
* Utilizes popular message broker technologies, including RabbitMQ or Apache Kafka, to showcase different options and configurations.
* Provides example code and documentation for setting up message producers, consumers, and message handlers.
* Includes sample microservices that communicate through the message broker, highlighting the benefits of loose coupling and scalability.
* Offers insights and best practices for building resilient, scalable, and fault-tolerant distributed systems.
* Includes Dockerfiles and Docker Compose configuration for containerizing the sample microservices.
* Provides Kubernetes deployment manifests and Helm charts for deploying the microservices to a Kubernetes cluster.
* Demonstrates how to scale and manage the message broker infrastructure using Kubernetes and Helm.
* Seamless integration of Rust implementation for Order functionality with the existing infrastructure without requiring any modifications to the database schema or the message broker configuration.
* Configure OpenTelemetry to export traces directly to Jaeger.
* Supports ArgoCD using Helm charts.

![Alt text](docs/architecture.png?raw=true "Application architecture")

## Dependencies
[.NET 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

[.NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview)

[Docker](https://docs.docker.com/engine/install/)

[Minikube](https://minikube.sigs.k8s.io/docs/start/)

[Helm](https://helm.sh/docs/intro/install/)

[ArgoCD](https://argo-cd.readthedocs.io/en/stable/)

## Application structure

Application consists of several .NET microservices all communicating with RabbitMQ:
1. **CatalogAPI** provides basic API for querying products. Data persisted in mongo **CatalogDb**.
2. **CustomerAPI** basic authentication microservice that allows to register, log in and delete users. Data persisted in mongo **CustomerDb**.
3. **Notifications** observes events on the message broker and sends user specific events to clients.
4. **OcelotAPIGateway** basic Gateway microservice that stands in front of all microservices.
5. **OrderAPI** provides basic API for managing orders. It interfaces with MongoDB for data storage and RabbitMQ for message queuing, and it exposes a RESTful API for managing orders. Data persisted in mongo **OrderDb**.
6. **OrderGrpc** internal microservice for managing orders via GRPC instead of REST API, used by **OrderProcessor**.
7. **OrderProcessor** main ordering business logic, uses message broker to produce and consume messages to and from other services.
8. **PaymentService** basic payment microservice, consumes and produces message from the message broker. Data persisted in mongo **PaymentDb**.
9. **Shipment** basic shipment microservice, consumes and produces message from the message broker. Data persisted in mongo **ShipmentDb**.
10. **ShoppingCartAPI** manages user shopping carts via REST API. Data persisted in mongo **ShoppingCartDb**.
11. **Warehouse** basic warehouse microservice, consumes and produces message from the message broker.
12. **WarehouseAPI** provides REST API to get products stock levels. Used by **WebUI**. Data persisted in mongo **WarehouseDb**.
13. **WebUI** main UI application built with .NET Razor pages. Uses **WebUIAggregator** and **OcelotAPIGateway**. Supports SignalR notifications to browser clients.
14. **WebUIAggregator** aggregates requests to multiple microservices. Used by **WebUI**.
15. **roderapi** Rust implementation of **OrderAPI** seamlessly integrated with the existing infrastructure without requiring any modifications to the database schema or the message broker configuration. This demonstrates the interoperability and flexibility of the system architecture, allowing developers to leverage different programming languages while maintaining compatibility with the existing ecosystem.

## Sagas

### Create order

![Alt text](docs/saga_create_order.png?raw=true "Create order saga")

You can check the data model and other sagas in ![solution diagrams](docs/DotNetRabbitMQIntegration.drawio "solution diagrams").

## How to run this sample

You can run this sample in 5 different ways:
1. Run .NET apps locally with RabbitMQ and Mongo running in docker.
2. Multi-Container .NET apps, RabbitMQ and Mongo running in Kubernetes.
3. Using [.NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview).
4. Using [a Helm chart](https://github.com/helm/helm).
5. Using [ArgoCD](https://argo-cd.readthedocs.io/en/stable/).

### Run local

**Run with Powershell** script **01_run_local.ps1**. This will start:
1. RabbitMQ and Mongo in Docker.
2. All .NET microservices will get started in separate windows.
3. [http://localhost:8011/](http://localhost:8011/) will be opened in your browser's window.
4. Wait for all the services to start up and hit Refresh. You should see the main page.

![Alt text](docs/run_local.png?raw=true "Run local")

![Index page](docs/index_page.png?raw=true "Index page")

### Run in Kubernetes

1. **Run with Powershell** script **03_start_minikube.ps1** to start **minikube**. Wait for the script to complete.
2. **Run with Powershell** script **04_run_minimal_k8s.ps1** to deploy docker images to Kubernetes.
3. Observe the deployment in opened Kubernetes dashboard. Wait for all the pods change the status to **Running**.
4. Main **WebUI** service and Jaeger UI are going to be exposed by the minikube.
![Alt text](docs/traces_jaeger.png?raw=true "Traces in Jaeger")
5. **Run with Powershell** script **04_stop_k8s.ps1** to delete the deployment.

![Alt text](docs/run_k8s.png?raw=true "Run in Kubernetes")

### Run with Aspire

1. Make sure you have docker running locally
2. Execute in `\eshop-aspire.AppHost`
```
dotnet run
```
3. Aspire dashboard page should be opened

![Alt text](docs/aspire.png?raw=true "Aspire dashboard")

### Using Helm chart

1. **Run with Powershell** script **03_start_minikube.ps1** to start **minikube**. Wait for the script to complete.
2. **Run with Powershell** script **05_run_minimal_helm.ps1** to install helm chart.
3. Observe the deployment in opened Kubernetes dashboard. Wait for all the pods change the status to **Running**.
4. Main **WebUI** service is going to be exposed by the minikube.
5. Stop it by running **05_stop_helm.ps1**.

```
|-----------|-------|-------------|---------------------------|
| NAMESPACE | NAME  | TARGET PORT |            URL            |
|-----------|-------|-------------|---------------------------|
| default   | webui | http/80     | http://192.168.49.2:32289 |
|-----------|-------|-------------|---------------------------|
* Starting tunnel for service webui.
|-----------|-------|-------------|------------------------|
| NAMESPACE | NAME  | TARGET PORT |          URL           |
|-----------|-------|-------------|------------------------|
| default   | webui |             | http://127.0.0.1:62184 |
|-----------|-------|-------------|------------------------|
```

11. **Run with Powershell** script **05_stop_helm.ps1** to uninstall the helm chart.

![Alt text](docs/helmchart_structure.png?raw=true "Helm chart structure")

### Using ArgoCD

1. **Run with Powershell** script **03_start_minikube.ps1** to start **minikube**. Wait for the script to complete.
2. **Run with Powershell** script **06_run_argocd.ps1** to start ArgoCD deployment.

ArgoCD dashboard is going to be opened in https://localhost:8080/. Main **WebUI** service is going to be exposed by the minikube. You can as well observe deployment in minikube dashboard inside 
**argocd-eshop-minimal** namespace. You can stop the deployment and delete pods with **07_stop_argocd.ps1** Powershell script.

![Alt text](docs/argocd.png?raw=true "ArgoCD dashboard")

## References
[RabbitMQ .NET Tutorial](https://www.rabbitmq.com/tutorials/tutorial-one-dotnet.html)

[Building Microservices, 2nd Edition. Sam Newman. Message Brokers](https://learning.oreilly.com/library/view/building-microservices-2nd/9781492034018/ch05.html#:-:text=Message%20Brokers)

[Kubernetes: Host Multi-Container ASP.NET Core app to Multiple Pods](https://www.yogihosting.com/aspnet-core-kubernetes-multi-pods/)

[How to use Helm for ASP.NET Core with Kubernetes](https://www.yogihosting.com/helm-charts-aspnet-core-kubernetes/)

[Creating a Helm chart for an ASP.NET Core app](https://andrewlock.net/deploying-asp-net-core-applications-to-kubernetes-part-4-creating-a-helm-chart-for-an-aspnetcore-app/)

[Difference between pushing a docker image and installing helm image](https://stackoverflow.com/questions/70093925/difference-between-pushing-a-docker-image-and-installing-helm-image)

[How to create a helm chart to deploy multiple applications using the same value.yaml file](https://stackoverflow.com/questions/48806009/how-to-create-a-helm-chart-to-deploy-multiple-applications-using-the-same-value)

[RabbitMQ: How to combine a task queue and a fanout/routing/topic models](https://stackoverflow.com/questions/36112650/rabbitmq-how-to-combine-a-task-queue-and-a-fanout-routing-topic-models)

[pubsub with multiple instances of each consumer](https://softwareengineering.stackexchange.com/questions/354400/pubsub-with-multiple-instances-of-each-consumer)
