package api

import (
	"context"
	"net/http"

	"github.com/gin-gonic/gin"
	"go.mongodb.org/mongo-driver/mongo"

	"discount/internal/data"
	"discount/internal/models"
)

func getUserPromos(ctx context.Context, collection *mongo.Collection) gin.HandlerFunc {
	fn := func(c *gin.Context) {
		var results = data.GetUserPromos(collection)

		c.IndentedJSON(http.StatusOK, results)
	}

	return gin.HandlerFunc(fn)
}

func getUserPromoForCustomerId(ctx context.Context, collection *mongo.Collection) gin.HandlerFunc {
	fn := func(c *gin.Context) {
		customerId := c.Param("customerId")

		var result = data.FindUserPromoForCustomerId(customerId, collection)
		if result == nil {
			c.IndentedJSON(http.StatusNotFound, gin.H{"message": "UserPromo not found."})
			return
		}

		c.IndentedJSON(http.StatusOK, *result)
	}

	return gin.HandlerFunc(fn)
}

func createUserPromo(ctx context.Context, collection *mongo.Collection) gin.HandlerFunc {
	fn := func(c *gin.Context) {
		var newUserPromo models.UserPromo

		// Call BindJSON to bind the received JSON to UserPromo.
		if err := c.BindJSON(&newUserPromo); err != nil {
			c.IndentedJSON(http.StatusBadRequest, gin.H{"message": "Bad request. Cannor serialize UserPromo. Details: " + err.Error()})
			return
		}

		var result = data.CreateUserPromo(newUserPromo, collection)

		if result == nil {
			c.IndentedJSON(http.StatusConflict, gin.H{"message": "Conflict. UserPromo already exists."})
		}

		c.IndentedJSON(http.StatusCreated, *result)
	}

	return gin.HandlerFunc(fn)
}
