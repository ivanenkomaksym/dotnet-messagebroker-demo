# dotnet-messagebroker-demo

This sample demonstrates how to integrate .NET microservices with RabbitMQ. Worker queues and exchanges are used for asynchronous communication. It also includes deploying apps to Kubernetes and using Helm charts.

![Alt text](docs/architecture.png?raw=true "Application architecture")

## Dependencies
[.NET 7](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

[Docker](https://docs.docker.com/engine/install/)

[Minikube](https://minikube.sigs.k8s.io/docs/start/)

[Helm](https://helm.sh/docs/intro/install/)

## Application structure

Application consists of several .NET microservices all communicating with RabbitMQ:
1. **OrderAPI** provides basic API for managing orders and pushes messages to **queue.order** worker queue.
2. **OrderProcessor** reads messages from **queue.order** worker queue, processes them and pushes results to **exchange.order.status** fanout exchange.
3. **Warehouse** reads messages from **queue.order.status.warehouse** worker queue and processes them.
4. **Notifications** reads messages from **queue.order.status.notifications** worker queue and processes them.

![Alt text](docs/rabbitmq_integration.png?raw=true "Application structure")

**OrderAPI** pushes messages to a worker queue so they are distribued in round-robin way to all registered consumers in sequence. If **OrderProcessor** is scaled out, only one instance will process the message.
Messages pushed to **exchange.order.status** exchange will be distributed to all bind worker queues. Both Warehouse and Notifications microservices create additional worker queues and bind them to that single **exchange.order.status** exchange. This is done so that if microservice is scaled out, only one instance will process the message.

## Data model

![Alt text](docs/datamodel.png?raw=true "Data model")

## Sagas

### Create order

![Alt text](docs/saga_create_order.png?raw=true "Create order saga")

## How to run this sample

You can run this sample in 3 different ways:
1. Run .NET apps locally with RabbitMQ in docker.
2. Multi-Container .NET apps and RabbitMQ running in Kubernetes.
3. Using [a Helm chart](https://github.com/helm/helm) for deploying .NET apps and RabbitMQ to Kubernetes.

### Run local

**Run with Powershell** script **01_run_local.ps1**. This will start:
1. RabbitMQ in Docker.
2. All .NET microservices in separate windows.
3. Navigate to https://localhost:7137/swagger to access **OrderAPI**.
4. Navigate to http://localhost:15672/ to access **RabbitMQ Management** UI (login: **guest**, password: **guest**).
5. Execute **POST /api/Order**.
6. Observe logs in other microservices.
7. Observe bindings in http://localhost:15672/#/exchanges/%2F/exchange.order.status
8. Close windows manually.

![Alt text](docs/run_local.png?raw=true "Run local")

### Run in Kubernetes

1. **Run with Powershell** script **02_docker_build.ps1** to build docker images of all microservices. Wait for the script to complete.
2. **Run with Powershell** script **03_start_minikube.ps1** to start **minikube**. Wait for the script to complete.
3. **Run with Powershell** script **04_run_k8s.ps1** to deploy docker images to Kubernetes and start **orderapi-service** with **rabbitmq-service**.
4. Observe the deployment in opened Kubernetes dashboard. Wait for all the pods change the status to **Running**.
5. Link to **orderapi-service** service will get opened in your browser (e.g. http://127.0.0.1:57611/).
6. Append **/swagger** to access **OrderAPI** swagger page.
7. In another command prompt **rabbitmq-service** is exposed with URLs to access it.
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

9. Execute **POST /api/Order** on **OrderAPI** swagger page from step 6.
10. Observe logs in pods in Kubernetes dashboard from step 4.
11. **Run with Powershell** script **04_stop_k8s.ps1** to delete the deployment.

![Alt text](docs/run_k8s.png?raw=true "Run in Kubernetes")

### Using Helm chart

1. **Run with Powershell** script **02_docker_build.ps1** to build docker images of all microservices. Wait for the script to complete.
2. **Run with Powershell** script **03_start_minikube.ps1** to start **minikube**. Wait for the script to complete.
3. **Run with Powershell** script **05_run_helm.ps1** to install helm chart and start **helmapp-orderapi** with **helmapp-rabbitmq**.
4. Observe the deployment in opened Kubernetes dashboard. Wait for all the pods change the status to **Running**.
5. Link to **helmapp-orderapi** service will get opened in your browser (e.g. http://127.0.0.1:58021/).
6. Append **/swagger** to access **OrderAPI** swagger page.
7. In another command prompt **helmapp-rabbitmq** is exposed with URLs to access it.
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

9. Execute **POST /api/Order** on **OrderAPI** swagger page from step 6.
10. Observe logs in pods in Kubernetes dashboard from step 4.
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
