use lapin::{options::*, types::FieldTable, BasicProperties, Connection, ConnectionProperties};

const QUEUE_NAME: &str = "hello";

pub async fn publisher() -> Result<(), lapin::Error> {
    let addr = "amqp://guest:guest@localhost:5672";
    let conn = Connection::connect(&addr, ConnectionProperties::default())
        .await
        .expect("Failed to connect to RabbitMQ");

    let channel = conn.create_channel().await?;

    channel
        .queue_declare(
            QUEUE_NAME,
            QueueDeclareOptions::default(),
            FieldTable::default(),
        )
        .await?;

    let message = "Hello, RabbitMQ!";
    channel
        .basic_publish(
            "",
            QUEUE_NAME,
            BasicPublishOptions::default(),
            &message.as_bytes().to_vec(),
            BasicProperties::default(),
        )
        .await?;

    println!("Sent message: {}", message);

    Ok(())
}
