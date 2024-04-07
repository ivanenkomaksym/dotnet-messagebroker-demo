use lapin::{options::*, types::FieldTable, BasicProperties, Connection, ConnectionProperties};

use crate::configuration;

pub async fn publish_event<T>(config: &configuration::settings::RabbitMQSettings, exchange_name: &str, event: T) -> Result<(), lapin::Error> 
    where T: serde::Serialize
{
    let message = serde_json::to_string(&event).unwrap();
    
    publish_message(config, exchange_name, message).await
}

pub async fn publish_message(config: &configuration::settings::RabbitMQSettings, exchange_name: &str, message: String) -> Result<(), lapin::Error> 
{
    let conn = Connection::connect(&config.ampq_connection_string, ConnectionProperties::default())
        .await
        .expect("Failed to connect to RabbitMQ");

    let channel = conn.create_channel().await?;

    channel.exchange_declare(
        exchange_name,
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
        
    let payload = message.as_bytes();
    channel.basic_publish(
        exchange_name,
        "",
        BasicPublishOptions::default(),
        payload,
        BasicProperties::default(),
    ).await?;

    println!("Sent `{}` event with content: {}", exchange_name, message);

    Ok(())
}
