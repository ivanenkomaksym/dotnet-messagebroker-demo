minikube start --kubernetes-version=v1.28.4 --disk-size=2GB
minikube docker-env
minikube -p minikube docker-env --shell powershell | Invoke-Expression

# for cmd:
# @for /f "tokens=*" %i in ('minikube -p minikube docker-env --shell cmd') do @%i

minikube addons enable metrics-server
minikube tunnel