package main

import (
	"encoding/json"
	"log"
	"os"

	"context"
)

func main() {
	configuration := readConfiguration()
	ctx, client := CreateClient(configuration)

	defer func() {
		var err error
		if err = client.Disconnect(context.TODO()); err != nil {
			panic(err)
		}
	}()

	collection := client.Database(configuration.DatabaseSettings.DatabaseName).Collection(configuration.DatabaseSettings.CollectionName)

	SeedData(configuration, ctx, collection)
}

func readConfiguration() Configuration {
	f, err := os.ReadFile("appsettings.json")
	if err != nil {
		log.Println(err)
	}
	configuration := Configuration{}
	json.Unmarshal([]byte(f), &configuration)

	return configuration
}
