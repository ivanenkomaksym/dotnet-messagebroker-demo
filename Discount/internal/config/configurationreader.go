package config

import (
	_ "embed"
	"encoding/json"
	"os"
)

//go:embed appsettings.json
var appsettingsContent []byte

func ReadConfiguration() Configuration {
	configuration := Configuration{}
	json.Unmarshal(appsettingsContent, &configuration)

	if databaseSettingsConnectionStringEnvVar := os.Getenv("DatabaseSettings__ConnectionString"); databaseSettingsConnectionStringEnvVar != "" {
		configuration.DatabaseSettings.ConnectionString = databaseSettingsConnectionStringEnvVar
	}

	if apiSettingsGatewayAddressEnvVar := os.Getenv("ApiSettings__GatewayAddress"); apiSettingsGatewayAddressEnvVar != "" {
		configuration.ApiSettings.GatewayAddress = apiSettingsGatewayAddressEnvVar
	}

	if rabbitMQSettingsAMQPConnectionStringEnvVar := os.Getenv("RabbitMQSettings__AMQPConnectionString"); rabbitMQSettingsAMQPConnectionStringEnvVar != "" {
		configuration.RabbitMQSettings.AMQPConnectionString = rabbitMQSettingsAMQPConnectionStringEnvVar
	}

	if serverSettingsApplicationUrlStringEnvVar := os.Getenv("ServerSettings__ApplicationUrl"); serverSettingsApplicationUrlStringEnvVar != "" {
		configuration.ServerSettings.ApplicationUrl = serverSettingsApplicationUrlStringEnvVar
	}

	return configuration
}
