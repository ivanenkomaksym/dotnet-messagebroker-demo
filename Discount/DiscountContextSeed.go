package main

import (
	"context"
	"fmt"
	"log"
	"math/rand"
	"time"

	"github.com/google/uuid"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
)

func SeedData(configuration Configuration) {

	client, err := mongo.NewClient(options.Client().ApplyURI(configuration.DatabaseSettings.ConnectionString))
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

	db := client.Database(configuration.DatabaseSettings.DatabaseName)

	// Create a collection
	collection := db.Collection(configuration.DatabaseSettings.CollectionName)

	// Check if the collection is empty
	count, _ := collection.CountDocuments(ctx, nil)
	if count > 0 {
		return
	}

	fmt.Println("SeedData started")

	var discounts []Discount
	products := getProducts(configuration)
	for _, product := range products {
		discounts = append(discounts, Discount{
			ID:          uuid.New().String(),
			ProductId:   product.Id,
			ProductName: product.Name,
			Discount:    (float64)(rand.Intn(20)) / 100.0,
		})
	}

	interfaceStmts := []interface{}{}
	for _, d := range discounts {
		interfaceStmts = append(interfaceStmts, d)
	}

	_, err = collection.InsertMany(ctx, interfaceStmts)
	if err != nil {
		log.Fatal(err)
	}
}
