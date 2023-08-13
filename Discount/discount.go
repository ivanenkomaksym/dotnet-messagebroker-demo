package main

type Discount struct {
	ID          string  `bson:"id"`
	ProductId   string  `bson:"productId"`
	ProductName string  `bson:"productName"`
	Discount    float64 `bson:"discount"`
}
