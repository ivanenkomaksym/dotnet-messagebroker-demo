package data

import (
	"context"

	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"

	"discount/internal/models"
)

func CreateUserPromo(userPromo models.UserPromo, collection *mongo.Collection) *models.UserPromo {
	insertOneResult, err := collection.InsertOne(context.Background(), userPromo)
	if err != nil {
		panic(err)
	}

	if insertOneResult.InsertedID == nil {
		return nil
	}

	return &userPromo
}

func DeleteUserPromoForCustomerId(customerId string, collection *mongo.Collection) bool {
	filter := bson.D{{Key: "customerId", Value: customerId}}

	result, err := collection.DeleteOne(context.Background(), filter)
	if err != nil {
		panic(err)
	}

	return result.DeletedCount > 0
}

func GetUserPromos(collection *mongo.Collection) []models.UserPromo {
	cursor, err := collection.Find(context.Background(), bson.D{})
	if err != nil {
		panic(err)
	}

	var results []models.UserPromo
	if err = cursor.All(context.Background(), &results); err != nil {
		panic(err)
	}

	return results
}

func FindUserPromoForCustomerId(customerId string, collection *mongo.Collection) *models.UserPromo {
	filter := bson.D{{Key: "customerId", Value: customerId}}

	var result models.UserPromo
	var foundResult = collection.FindOne(context.Background(), filter)
	if foundResult.Err() == nil {
		foundResult.Decode(&result)

		return &result
	}

	return nil
}
