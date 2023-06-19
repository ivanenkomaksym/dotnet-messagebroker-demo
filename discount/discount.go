package main

// album represents data about a record album.
type Discount struct {
	ID          string  `json:"id"`
	ProductId   string  `json:"productId"`
	ProductName string  `json:"productName"`
	Discount    float64 `json:"discount"`
}
