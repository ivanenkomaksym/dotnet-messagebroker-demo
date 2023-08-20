package rabbitmq

import (
	"encoding/json"
	"log"

	"github.com/google/uuid"
	amqp "github.com/rabbitmq/amqp091-go"
	"go.mongodb.org/mongo-driver/mongo"

	"discount/internal/data"
	"discount/internal/models"
)

type RabbitMQMessage struct {
	AddUserPromo AddUserPromo `json:"message"`
}

func failOnError(err error, msg string) {
	if err != nil {
		log.Panicf("%s: %s", msg, err)
	}
}

func ConsumeAddUserPromo(collection *mongo.Collection) {
	// TODO: extract this to a configuration file
	conn, err := amqp.Dial("amqp://guest:guest@localhost:5672")
	failOnError(err, "Failed to connect to RabbitMQ")
	defer conn.Close()

	ch, err := conn.Channel()
	failOnError(err, "Failed to open a channel")
	defer ch.Close()

	err = ch.ExchangeDeclare(
		"Common.Events:AddUserPromo", // name
		"fanout",                     // type
		true,                         // durable
		false,                        // auto-deleted
		false,                        // internal
		false,                        // no-wait
		nil,                          // arguments
	)
	failOnError(err, "Failed to declare an exchange")

	q, err := ch.QueueDeclare(
		"",    // name
		false, // durable
		false, // delete when unused
		true,  // exclusive
		false, // no-wait
		nil,   // arguments
	)
	failOnError(err, "Failed to declare a queue")

	err = ch.QueueBind(
		q.Name,                       // queue name
		"",                           // routing key
		"Common.Events:AddUserPromo", // exchange
		false,
		nil)
	failOnError(err, "Failed to bind a queue")

	msgs, err := ch.Consume(
		q.Name, // queue
		"",     // consumer
		true,   // auto-ack
		false,  // exclusive
		false,  // no-local
		false,  // no-wait
		nil,    // args
	)
	failOnError(err, "Failed to register a consumer")

	var forever chan struct{}

	go func() {
		for d := range msgs {
			log.Printf(" [x] %s", d.Body)

			var rabbitmqMessage RabbitMQMessage
			json.Unmarshal(d.Body, &rabbitmqMessage)

			var addUserPromo = rabbitmqMessage.AddUserPromo

			var newUserPromo = models.UserPromo{
				ID:            uuid.New().String(),
				CustomerId:    addUserPromo.CustomerInfo.CustomerId,
				CustomerEmail: addUserPromo.CustomerInfo.Email,
				Promo:         addUserPromo.Promo,
				ValidUntil:    addUserPromo.ValidUntil.Time,
			}

			data.CreateUserPromo(newUserPromo, collection)
		}
	}()

	log.Printf(" [*] Waiting for logs. To exit press CTRL+C")
	<-forever
}
