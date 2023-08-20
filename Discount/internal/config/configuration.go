package config

type ApiSettings struct {
	GatewayAddress string
}

type DatabaseSettings struct {
	ConnectionString         string
	DatabaseName             string
	DiscountsCollectionName  string
	UserPromosCollectionName string
}

type ServerSettings struct {
	ApplicationUrl string
}

type RabbitMQSettings struct {
	AMQPConnectionString string
}

type Configuration struct {
	ApiSettings      ApiSettings
	DatabaseSettings DatabaseSettings
	ServerSettings   ServerSettings
	RabbitMQSettings RabbitMQSettings
}
