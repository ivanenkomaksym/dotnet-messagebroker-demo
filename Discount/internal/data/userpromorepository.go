package data

import (
	"context"

	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"

	"discount/internal/models"

	"github.com/shopspring/decimal"
	"go.mongodb.org/mongo-driver/bson/primitive"
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

	// Convert mongo primitive.Decimal128 to decimal.Decimal, calculate cashback and convert back to primitive.Decimal128
	calculatedCashback, _ := decimal.NewFromString(foundUserPromo.Cashback.String())
	currentCashback, _ := decimal.NewFromString(userPromo.Cashback.String())
	calculatedValidUntil := foundUserPromo.ValidUntil
	if cashbackOperation == Add {
		calculatedCashback = calculatedCashback.Add(currentCashback)
		calculatedValidUntil = userPromo.ValidUntil
	} else {
		calculatedCashback = calculatedCashback.Sub(currentCashback)
	}

	parsedCashback, err := primitive.ParseDecimal128(calculatedCashback.String())
	if err != nil {
		panic(err)
	}

	update := bson.D{
		{Key: "$set", Value: bson.D{
			{Key: "cashback", Value: parsedCashback},
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
