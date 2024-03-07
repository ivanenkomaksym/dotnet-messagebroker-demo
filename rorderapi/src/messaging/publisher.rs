use lapin::{options::*, types::FieldTable, BasicProperties, Connection, ConnectionProperties};

use crate::events::ordercreated::OrderCreated;

const QUEUE_NAME: &str = "Common.Events:OrderCreated";

pub async fn publish_order_created(order_created: OrderCreated) -> Result<(), lapin::Error> {
    let addr = "amqp://guest:guest@localhost:5672";
    let conn = Connection::connect(&addr, ConnectionProperties::default())
        .await
        .expect("Failed to connect to RabbitMQ");

    let channel = conn.create_channel().await?;

    channel.exchange_declare(
            QUEUE_NAME,
            lapin::ExchangeKind::Fanout,
            ExchangeDeclareOptions {
                passive: true,
                durable: true,
                auto_delete: false,
                internal: false,
                nowait: false
            },
            FieldTable::default()
        )
        .await?;

    let q= channel
        .queue_declare(
            "OrderProcessor",
            QueueDeclareOptions {
                passive: true,
                durable: false,
                exclusive: true,
                auto_delete: false,
                nowait: false
            },
            FieldTable::default(),
        )
        .await?;

    channel.queue_bind(
        q.name().as_str(),
        QUEUE_NAME,
        "",
        QueueBindOptions { nowait: false },
        FieldTable::default()).await?;

    let message = serde_json::to_string(&order_created).unwrap();
    let payload = message.as_bytes();
    channel.basic_publish(
        QUEUE_NAME,
        "",
        BasicPublishOptions::default(),
        payload,
        BasicProperties::default(),
    ).await?;

    println!("Sent message: {}", message);

    Ok(())
}
