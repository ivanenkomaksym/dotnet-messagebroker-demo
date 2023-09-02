package models

import "time"

type UserPromo struct {
	ID            string    `bson:"id"`
	CustomerId    string    `bson:"customerId"`
	CustomerEmail string    `bson:"customerEmail"`
	Cashback      float64   `bson:"cashback"`
	ValidUntil    time.Time `bson:"validUntil"`
}
