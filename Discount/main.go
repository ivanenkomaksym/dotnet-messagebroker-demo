package main

import (
	"encoding/json"
	"log"
	"os"
)

func main() {
	f, err := os.ReadFile("appsettings.json")
	if err != nil {
		log.Println(err)
	}
	configuration := Configuration{}
	json.Unmarshal([]byte(f), &configuration)

	// ctx, client := CreateClient(configuration)

	// db := client.Database(configuration.DatabaseSettings.DatabaseName)
	// err = db.CreateCollection(ctx, configuration.DatabaseSettings.CollectionName)
	// if err == nil {
	// 	log.Println(err)
	// 	return
	// }
	// coll := db.Collection(configuration.DatabaseSettings.CollectionName)

	// SeedData(configuration, ctx, coll)

	// defer func() {
	// 	err := client.Disconnect(ctx)
	// 	if err != nil {
	// 		panic(err)
	// 	}
	// }()

	SeedData(configuration)
}
