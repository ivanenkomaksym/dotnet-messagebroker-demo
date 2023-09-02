package rabbitmq

import (
	"discount/internal/models"
)

type AddUserCashback struct {
	CustomerInfo models.CustomerInfo `json:"customerInfo"`
	Cashback     float64             `json:"cashback"`
	ValidUntil   CustomTime          `json:"validUntil"`
}
