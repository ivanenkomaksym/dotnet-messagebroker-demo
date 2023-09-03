package rabbitmq

import (
	"discount/internal/models"
)

type AddUserCashback struct {
	CustomerInfo models.CustomerInfo `json:"customerInfo"`
	Cashback     string              `json:"cashback"`
	ValidUntil   CustomTime          `json:"validUntil"`
}
