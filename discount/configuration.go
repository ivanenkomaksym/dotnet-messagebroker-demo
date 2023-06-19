package main

type ApiSettings struct {
	GatewayAddress string
}

type DatabaseSettings struct {
	ConnectionString string
	DatabaseName     string
}

type Configuration struct {
	ApiSettings      ApiSettings
	DatabaseSettings DatabaseSettings
}
