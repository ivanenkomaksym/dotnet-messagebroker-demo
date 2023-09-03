package models

import (
	"time"

	"go.mongodb.org/mongo-driver/bson/primitive"
)

type UserPromo struct {
	ID            string               `bson:"id"`
	CustomerId    string               `bson:"customerId"`
	CustomerEmail string               `bson:"customerEmail"`
	Cashback      primitive.Decimal128 `bson:"cashback"`
	ValidUntil    time.Time            `bson:"validUntil"`
}
