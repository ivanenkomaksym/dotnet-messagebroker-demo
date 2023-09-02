package rabbitmq

import (
	"discount/internal/config"
	"log"

	amqp "github.com/rabbitmq/amqp091-go"
	"go.mongodb.org/mongo-driver/mongo"
)

func failOnError(err error, msg string) {
	if err != nil {
		log.Panicf("%s: %s", msg, err)
	}
}

func StartListening(configuration config.Configuration, collection *mongo.Collection) {
	conn, err := amqp.Dial(configuration.RabbitMQSettings.AMQPConnectionString)
	failOnError(err, "Failed to connect to RabbitMQ")
	defer conn.Close()

	ch, err := conn.Channel()
	failOnError(err, "Failed to open a channel")
	defer ch.Close()

	go ConsumeAddUserCashback(ch, collection)
	go ConsumeSubUserCashback(ch, collection)

	var forever chan struct{}

	log.Printf(" [*] Waiting for logs. To exit press CTRL+C")
	<-forever
}
