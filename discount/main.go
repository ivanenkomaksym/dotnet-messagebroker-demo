package main

import (
	"encoding/json"
	"log"
	"os"
)

func main() {
	f, err := os.ReadFile("appsettings.json")
	if err != nil {
		log.Println(err)
	}
	configuration := Configuration{}
	json.Unmarshal([]byte(f), &configuration)

	getProducts(configuration)
}
