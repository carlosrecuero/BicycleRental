cd .\BicycleRental\BicycleRental.Service

dapr run --app-id cardservice --app-port 5001 --app-protocol http --dapr-http-port 56001 --resources-path ../resources -- dotnet run --urls=http://localhost:5001/


cd .\BicycleRental\src\BicycleRental.Client\

dapr run --app-id bicycleclient -- dotnet run