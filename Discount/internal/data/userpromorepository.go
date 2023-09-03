package data

import (
	"context"

	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"

	"discount/internal/models"
)

type base int

const (
	Add base = iota
	Sub
)

func CreateOrUpdateUserPromo(userPromo models.UserPromo, collection *mongo.Collection, cashbackOperation base) *models.UserPromo {
	foundUserPromo := FindUserPromoForCustomerId(userPromo.CustomerId, collection)
	if foundUserPromo == nil {
		if cashbackOperation == Sub {
			return nil
		}

		CreateUserPromo(userPromo, collection)
		return &userPromo
	}

	calculatedCashback := foundUserPromo.Cashback
	calculatedValidUntil := foundUserPromo.ValidUntil
	if cashbackOperation == Add {
		calculatedCashback += userPromo.Cashback
		calculatedValidUntil = userPromo.ValidUntil
	} else {
		calculatedCashback -= userPromo.Cashback
	}

	update := bson.D{
		{Key: "$set", Value: bson.D{
			{Key: "cashback", Value: calculatedCashback},
			{Key: "validUntil", Value: calculatedValidUntil},
		}},
	}
	filter := bson.D{{"id", foundUserPromo.ID}}
	updateResult, err := collection.UpdateOne(context.Background(), filter, update)
	if err != nil {
		panic(err)
	}

	if updateResult.ModifiedCount == 0 {
		return nil
	}

	return &userPromo
}

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
