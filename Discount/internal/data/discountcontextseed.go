package data

import (
	"context"
	"fmt"
	"log"
	"math/rand"

	"github.com/google/uuid"
	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"

	"discount/internal/config"
	"discount/internal/models"
)

func SeedData(configuration config.Configuration, ctx context.Context, collection *mongo.Collection) {
	// Check if the collection is empty
	opts := options.Count().SetHint("_id_")
	count, _ := collection.CountDocuments(ctx, bson.D{}, opts)
	if count > 0 {
		return
	}

	fmt.Println("SeedData started.")

	var discounts []models.Discount
	products := getProducts(configuration)
	for _, product := range products {
		discounts = append(discounts, models.Discount{
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

	var err error
	_, err = collection.InsertMany(ctx, interfaceStmts)
	if err != nil {
		log.Fatal(err)
	}

	fmt.Println("SeedData finished.")
}
