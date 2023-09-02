package rabbitmq

import (
	"encoding/json"
	"log"

	"github.com/google/uuid"
	"github.com/rabbitmq/amqp091-go"
	"go.mongodb.org/mongo-driver/mongo"

	"discount/internal/data"
	"discount/internal/models"
)

type AddRabbitMQMessage struct {
	AddUserCashback AddUserCashback `json:"message"`
}

func ConsumeAddUserCashback(channel *amqp091.Channel, collection *mongo.Collection) {
	err := channel.ExchangeDeclare(
		"Common.Events:AddUserCashback", // name
		"fanout",                        // type
		true,                            // durable
		false,                           // auto-deleted
		false,                           // internal
		false,                           // no-wait
		nil,                             // arguments
	)
	failOnError(err, "Failed to declare an exchange")

	q, err := channel.QueueDeclare(
		"",    // name
		false, // durable
		false, // delete when unused
		true,  // exclusive
		false, // no-wait
		nil,   // arguments
	)
	failOnError(err, "Failed to declare a queue")

	err = channel.QueueBind(
		q.Name,                          // queue name
		"",                              // routing key
		"Common.Events:AddUserCashback", // exchange
		false,
		nil)
	failOnError(err, "Failed to bind a queue")

	msgs, err := channel.Consume(
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

			var rabbitmqMessage AddRabbitMQMessage
			json.Unmarshal(d.Body, &rabbitmqMessage)

			var addUserCashback = rabbitmqMessage.AddUserCashback

			var newUserPromo = models.UserPromo{
				ID:            uuid.New().String(),
				CustomerId:    addUserCashback.CustomerInfo.CustomerId,
				CustomerEmail: addUserCashback.CustomerInfo.Email,
				Cashback:      addUserCashback.Cashback,
				ValidUntil:    addUserCashback.ValidUntil.Time,
			}

			data.CreateOrUpdateUserPromo(newUserPromo, collection, data.Add)
		}
	}()

	log.Printf(" [*] Waiting for logs. To exit press CTRL+C")
	<-forever
}
