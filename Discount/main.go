package main

import (
	"encoding/json"
	"log"
	"os"

	"context"

	"github.com/gin-gonic/gin"
)

func main() {
	configuration := readConfiguration()
	ctx, client := CreateClient(configuration)

	defer func() {
		if err := client.Disconnect(context.TODO()); err != nil {
			panic(err)
		}
	}()

	collection := client.Database(configuration.DatabaseSettings.DatabaseName).Collection(configuration.DatabaseSettings.CollectionName)

	SeedData(configuration, ctx, collection)

	router := gin.Default()
	router.GET("/api/discounts", getDiscounts(ctx, collection))
	router.GET("/api/discounts/:productId", getDiscountByProductId(ctx, collection))

	if err := router.Run(configuration.ServerSettings.ApplicationUrl); err != nil {
		panic(err)
	}
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
