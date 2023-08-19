package main

type AddUserPromo struct {
	CustomerInfo CustomerInfo `json:"customerInfo"`
	Promo        float64      `json:"promo"`
	ValidUntil   CustomTime   `json:"validUntil"`
}
