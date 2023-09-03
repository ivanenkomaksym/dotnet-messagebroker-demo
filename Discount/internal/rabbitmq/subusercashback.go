package rabbitmq

import (
	"discount/internal/models"
)

type SubUserCashback struct {
	CustomerInfo models.CustomerInfo `json:"customerInfo"`
	Cashback     string              `json:"cashback"`
}
