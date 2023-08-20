package api

import (
	"context"

	"github.com/gin-gonic/gin"
	"go.mongodb.org/mongo-driver/mongo"
)

func StartHttpServer(ctx context.Context, discountsCollection *mongo.Collection, userPromosCollection *mongo.Collection) {
	router := gin.Default()
	router.GET("/api/discounts", getDiscounts(ctx, discountsCollection))
	router.GET("/api/discounts/:productId", getDiscountByProductId(ctx, discountsCollection))

	router.GET("/api/userpromos", getUserPromos(ctx, userPromosCollection))
	router.GET("/api/userpromos/:customerId", getUserPromoForCustomerId(ctx, userPromosCollection))
	router.POST("/api/userpromos", createUserPromo(ctx, userPromosCollection))

	if err := router.Run(":80"); err != nil {
		panic(err)
	}
}
