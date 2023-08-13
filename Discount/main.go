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

	discountsCollection := client.Database(configuration.DatabaseSettings.DatabaseName).Collection(configuration.DatabaseSettings.DiscountsCollectionName)
	userPromosCollection := client.Database(configuration.DatabaseSettings.DatabaseName).Collection(configuration.DatabaseSettings.UserPromosCollectionName)

	SeedData(configuration, ctx, discountsCollection)

	router := gin.Default()
	router.GET("/api/discounts", getDiscounts(ctx, discountsCollection))
	router.GET("/api/discounts/:productId", getDiscountByProductId(ctx, discountsCollection))

	router.GET("/api/userpromos", getUserPromos(ctx, userPromosCollection))
	router.GET("/api/userpromos/:customerId", getUserPromoForCustomerId(ctx, userPromosCollection))
	router.POST("/api/userpromos", createUserPromo(ctx, userPromosCollection))

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
