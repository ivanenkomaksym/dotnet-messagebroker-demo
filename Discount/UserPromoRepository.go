package main

import (
	"context"

	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
)

func CreateUserPromo(userPromo UserPromo, collection *mongo.Collection) *UserPromo {
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

func GetUserPromos(collection *mongo.Collection) []UserPromo {
	cursor, err := collection.Find(context.Background(), bson.D{})
	if err != nil {
		panic(err)
	}

	var results []UserPromo
	if err = cursor.All(context.Background(), &results); err != nil {
		panic(err)
	}

	return results
}

func FindUserPromoForCustomerId(customerId string, collection *mongo.Collection) *UserPromo {
	filter := bson.D{{Key: "customerId", Value: customerId}}

	var result UserPromo
	var foundResult = collection.FindOne(context.Background(), filter)
	if foundResult.Err() != nil {
		foundResult.Decode(&result)

		return &result
	}

	return nil
}
