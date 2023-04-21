docker-compose build
.\start_minikube.ps1
cd .\charts\helmapp
helm upgrade --install helmapp .
minikube dashboard
minikube service helmapp-orderapi