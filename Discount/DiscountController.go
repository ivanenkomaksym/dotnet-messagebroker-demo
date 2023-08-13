package main

import (
	"context"
	"net/http"

	"github.com/gin-gonic/gin"
	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
)

func getDiscounts(ctx context.Context, collection *mongo.Collection) gin.HandlerFunc {
	fn := func(c *gin.Context) {
		cursor, err := collection.Find(ctx, bson.D{})
		if err != nil {
			panic(err)
		}

		var results []Discount
		if err = cursor.All(ctx, &results); err != nil {
			c.IndentedJSON(http.StatusInternalServerError, gin.H{"message": "InternalServerError. Details: " + err.Error()})
			panic(err)
		}

		c.IndentedJSON(http.StatusOK, results)
	}

	return gin.HandlerFunc(fn)
}

func getDiscountByProductId(ctx context.Context, collection *mongo.Collection) gin.HandlerFunc {
	fn := func(c *gin.Context) {
		productId := c.Param("productId")

		filter := bson.D{{Key: "productId", Value: productId}}
		opts := options.FindOne()

		var result Discount
		err := collection.FindOne(ctx, filter, opts).Decode(&result)
		if err != nil {
			c.IndentedJSON(http.StatusNotFound, gin.H{"message": "NotFound. Details: " + err.Error()})
			panic(err)
		}

		c.IndentedJSON(http.StatusOK, result)
	}

	return gin.HandlerFunc(fn)
}
