package main

type ApiSettings struct {
	GatewayAddress string
}

type DatabaseSettings struct {
	ConnectionString string
	DatabaseName     string
	CollectionName   string
}

type ServerSettings struct {
	ApplicationUrl string
}

type Configuration struct {
	ApiSettings      ApiSettings
	DatabaseSettings DatabaseSettings
	ServerSettings   ServerSettings
}
