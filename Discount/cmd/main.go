package main

import (
	"context"
	"discount/internal/api"
	"discount/internal/config"
	"discount/internal/data"
	"discount/internal/rabbitmq"
)

func main() {
	configuration := config.ReadConfiguration()
	ctx, client := data.CreateClient(configuration)

	defer func() {
		if err := client.Disconnect(context.TODO()); err != nil {
			panic(err)
		}
	}()

	discountsCollection := client.Database(configuration.DatabaseSettings.DatabaseName).Collection(configuration.DatabaseSettings.DiscountsCollectionName)
	userPromosCollection := client.Database(configuration.DatabaseSettings.DatabaseName).Collection(configuration.DatabaseSettings.UserPromosCollectionName)

	data.SeedData(configuration, ctx, discountsCollection)

	go rabbitmq.ConsumeAddUserPromo(configuration, userPromosCollection)

	api.StartHttpServer(ctx, discountsCollection, userPromosCollection)
}
