cd .\charts\helmapp
helm upgrade --install helmapp .
minikube dashboard
minikube service helmapp-orderapi