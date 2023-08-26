package api

import (
	"context"
	"net/http"

	"github.com/gin-gonic/gin"
	"go.mongodb.org/mongo-driver/mongo"

	"discount/internal/data"
)

func getDiscounts(ctx context.Context, collection *mongo.Collection) gin.HandlerFunc {
	fn := func(c *gin.Context) {
		var results = data.GetDiscounts(collection)

		c.IndentedJSON(http.StatusOK, results)
	}

	return gin.HandlerFunc(fn)
}

func getDiscountByProductId(ctx context.Context, collection *mongo.Collection) gin.HandlerFunc {
	fn := func(c *gin.Context) {
		productId := c.Param("productId")

		var result = data.GetDiscountByProductId(productId, collection)
		if result == nil {
			c.IndentedJSON(http.StatusNotFound, gin.H{"message": "Discount not found."})
			return
		}

		c.IndentedJSON(http.StatusOK, result)
	}

	return gin.HandlerFunc(fn)
}
