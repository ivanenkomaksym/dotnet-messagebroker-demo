package data

import (
	"encoding/json"
	"net/http"

	"discount/internal/config"
)

func getProducts(config config.Configuration) []Product {
	resp, err := http.Get(config.ApiSettings.GatewayAddress + "/gateway/Catalog")
	if err != nil {
		panic(err)
	}
	defer resp.Body.Close()

	var products []Product
	json.NewDecoder(resp.Body).Decode(&products)

	return products
}
