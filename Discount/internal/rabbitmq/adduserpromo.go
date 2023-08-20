package rabbitmq

import (
	"discount/internal/models"
)

type AddUserPromo struct {
	CustomerInfo models.CustomerInfo `json:"customerInfo"`
	Promo        float64             `json:"promo"`
	ValidUntil   CustomTime          `json:"validUntil"`
}
