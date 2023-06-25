package main

import (
	"context"
	"log"
	"time"

	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
)

func CreateClient(config Configuration) (context.Context, *mongo.Client) {
	// ctx, cancel := context.WithTimeout(context.Background(), 20*time.Second)
	// defer cancel()
	// client, err := mongo.Connect(ctx, options.Client().ApplyURI(config.DatabaseSettings.ConnectionString))
	// if err != nil {
	// 	panic(err)
	// }

	// return ctx, client
	// Set up a MongoDB client
	client, err := mongo.NewClient(options.Client().ApplyURI(config.DatabaseSettings.ConnectionString))
	if err != nil {
		log.Fatal(err)
	}

	// Set up a connection to the MongoDB server
	ctx, cancel := context.WithTimeout(context.Background(), 10*time.Second)
	defer cancel()

	err = client.Connect(ctx)
	if err != nil {
		log.Fatal(err)
	}

	return ctx, client
}
