package main

import (
	"context"
	"net/http"

	"github.com/gin-gonic/gin"
	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
)

func getUserPromos(ctx context.Context, collection *mongo.Collection) gin.HandlerFunc {
	fn := func(c *gin.Context) {
		cursor, err := collection.Find(ctx, bson.D{})
		if err != nil {
			panic(err)
		}

		var results []UserPromo
		if err = cursor.All(ctx, &results); err != nil {
			c.IndentedJSON(http.StatusInternalServerError, gin.H{"message": "InternalServerError. Details: " + err.Error()})
			panic(err)
		}

		c.IndentedJSON(http.StatusOK, results)
	}

	return gin.HandlerFunc(fn)
}

func getUserPromoForCustomerId(ctx context.Context, collection *mongo.Collection) gin.HandlerFunc {
	fn := func(c *gin.Context) {
		customerId := c.Param("customerId")

		filter := bson.D{{Key: "customerId", Value: customerId}}
		opts := options.FindOne()

		var result UserPromo
		err := collection.FindOne(ctx, filter, opts).Decode(&result)
		if err != nil {
			c.IndentedJSON(http.StatusNotFound, gin.H{"message": "NotFound. Details: " + err.Error()})
			panic(err)
		}

		c.IndentedJSON(http.StatusOK, result)
	}

	return gin.HandlerFunc(fn)
}

func createUserPromo(ctx context.Context, collection *mongo.Collection) gin.HandlerFunc {
	fn := func(c *gin.Context) {
		var newUserPromo UserPromo

		// Call BindJSON to bind the received JSON to UserPromo.
		if err := c.BindJSON(&newUserPromo); err != nil {
			c.IndentedJSON(http.StatusBadRequest, gin.H{"message": "Bad request. Cannor serialize UserPromo. Details: " + err.Error()})
			return
		}

		_, err := collection.InsertOne(context.TODO(), newUserPromo)
		if err != nil {
			c.IndentedJSON(http.StatusConflict, gin.H{"message": "UserPromo already exists"})
			panic(err)
		}

		c.IndentedJSON(http.StatusCreated, newUserPromo)
	}

	return gin.HandlerFunc(fn)
}
