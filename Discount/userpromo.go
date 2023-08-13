package main

import "time"

type UserPromo struct {
	ID            string    `bson:"id"`
	CustomerId    string    `bson:"customerId"`
	CustomerEmail string    `bson:"customerEmail"`
	Promo         float64   `bson:"promo"`
	ValidUntil    time.Time `bson:"validUntil"`
}
