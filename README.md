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

![Alt text](docs/architecture.png?raw=true "Application architecture")

## Dependencies
[.NET 7](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

[Docker](https://docs.docker.com/engine/install/)

[Minikube](https://minikube.sigs.k8s.io/docs/start/)

[Helm](https://helm.sh/docs/intro/install/)

## Application structure

Application consists of several .NET microservices all communicating with RabbitMQ:
1. **CatalogAPI** provides basic API for querying products. Data persisted in mongo **CatalogDb**.
2. **CustomerAPI** basic authentication microservice that allows to register, log in and delete users. Data persisted in mongo **CustomerDb**.
3. **Notifications** observes events on the message broker and sends user specific events to clients.
4. **OcelotAPIGateway** basic Gateway microservice that stands in front of all microservices.
5. **OrderAPI** provides basic API for managing orders. Data persisted in mongo **OrderDb**.
6. **OrderGrpc** internal microservice for managing orders via GRPC instead of REST API, used by **OrderProcessor**.
7. **OrderProcessor** main ordering business logic, uses message broker to produce and consume messages to and from other services.
8. **PaymentService** basic payment microservice, consumes and produces message from the message broker. Data persisted in mongo **PaymentDb**.
9. **Shipment** basic shipment microservice, consumes and produces message from the message broker. Data persisted in mongo **ShipmentDb**.
10. **ShoppingCartAPI** manages user shopping carts via REST API. Data persisted in mongo **ShoppingCartDb**.
11. **Warehouse** basic warehouse microservice, consumes and produces message from the message broker.
12. **WarehouseAPI** provides REST API to get products stock levels. Used by **WebUI**. Data persisted in mongo **WarehouseDb**.
13. **WebUI** main UI application built with .NET Razor pages. Uses **WebUIAggregator** and **OcelotAPIGateway**. Supports SignalR notifications to browser clients.
14. **WebUIAggregator** aggregates requests to multiple microservices. Used by **WebUI**.

## Sagas

### Create order

![Alt text](docs/saga_create_order.png?raw=true "Create order saga")

You can check the data model and other sagas in ![solution diagrams](docs/DotNetRabbitMQIntegration.drawio "solution diagrams").

## How to run this sample

You can run this sample in 3 different ways:
1. Run .NET apps locally with RabbitMQ and Mongo running in docker.
2. Multi-Container .NET apps, RabbitMQ and Mongo running in Kubernetes.
3. Using [a Helm chart](https://github.com/helm/helm).

### Run local

**Run with Powershell** script **01_run_local.ps1**. This will start:
1. RabbitMQ and Mongo in Docker.
2. All .NET microservices will get started in separate windows.
3. [http://localhost:8011/](http://localhost:8011/) will be opened in your browser's window.
4. Wait for all the services to start up and hit Refresh. You should see the main page.

![Alt text](docs/run_local.png?raw=true "Run local")

![Index page](docs/index_page.png?raw=true "Index page")

### Run in Kubernetes

1. **Run with Powershell** script **02_docker_build.ps1** to build docker images of all microservices. Wait for the script to complete.
2. **Run with Powershell** script **03_start_minikube.ps1** to start **minikube**. Wait for the script to complete.
3. **Run with Powershell** script **04_run_k8s.ps1** to deploy docker images to Kubernetes.
4. Observe the deployment in opened Kubernetes dashboard. Wait for all the pods change the status to **Running**.
5. 
8. Navigate to second URL (e.g. http://127.0.0.1:57613) to access **RabbitMQ Management** UI (login: **guest**, password: **guest**).

```
* Starting tunnel for service rabbitmq-service.
|-----------|------------------|-------------|------------------------|
| NAMESPACE |       NAME       | TARGET PORT |          URL           |
|-----------|------------------|-------------|------------------------|
| default   | rabbitmq-service |             | http://127.0.0.1:57612 |
|           |                  |             | http://127.0.0.1:57613 |
|-----------|------------------|-------------|------------------------|
```

10. Observe logs in pods in Kubernetes dashboard from step 4.
11. **Run with Powershell** script **04_stop_k8s.ps1** to delete the deployment.

![Alt text](docs/run_k8s.png?raw=true "Run in Kubernetes")

### Using Helm chart

1. **Run with Powershell** script **03_start_minikube.ps1** to start **minikube**. Wait for the script to complete.
2. **Run with Powershell** script **05_run_helm.ps1** to install helm chart and start **helmapp-orderapi** with **helmapp-rabbitmq**.
3. Observe the deployment in opened Kubernetes dashboard. Wait for all the pods change the status to **Running**.
8. Navigate to second URL (e.g. http://127.0.0.1:58024) to access **RabbitMQ Management** UI (login: **guest**, password: **guest**).

```
* Starting tunnel for service helmapp-rabbitmq.
|-----------|------------------|-------------|------------------------|
| NAMESPACE |       NAME       | TARGET PORT |          URL           |
|-----------|------------------|-------------|------------------------|
| default   | helmapp-rabbitmq |             | http://127.0.0.1:58023 |
|           |                  |             | http://127.0.0.1:58024 |
|-----------|------------------|-------------|------------------------|
```

11. **Run with Powershell** script **05_stop_helm.ps1** to uninstall the helm chart.

![Alt text](docs/helmchart_structure.png?raw=true "Helm chart structure")

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
