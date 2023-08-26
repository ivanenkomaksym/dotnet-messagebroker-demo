package data

import (
	"context"

	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"

	"discount/internal/models"
)

func GetDiscounts(collection *mongo.Collection) []models.Discount {
	cursor, err := collection.Find(context.Background(), bson.D{})
	if err != nil {
		panic(err)
	}

	var results []models.Discount
	if err = cursor.All(context.Background(), &results); err != nil {
		panic(err)
	}

	return results
}

func GetDiscountByProductId(productId string, collection *mongo.Collection) *models.Discount {
	filter := bson.D{{Key: "productId", Value: productId}}

	var result models.Discount
	var foundResult = collection.FindOne(context.Background(), filter)
	if foundResult.Err() == nil {
		foundResult.Decode(&result)

		return &result
	}

	return nil
}
