package rabbitmq

import (
	"discount/internal/models"
)

type SubUserCashback struct {
	CustomerInfo models.CustomerInfo `json:"customerInfo"`
	Cashback     float64             `json:"cashback"`
}
