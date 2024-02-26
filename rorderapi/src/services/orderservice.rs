use crate::{configuration, models::order::Order};
use mongodb::{ bson::doc, options::{ ClientOptions, ServerApi, ServerApiVersion }, Client, Collection };
use async_trait::async_trait;
use log::info;
use futures_util::TryStreamExt;

use super::orderserviceerror::OrderServiceError;

#[async_trait]
pub trait OrderTrait: Send + Sync {

    async fn init(&mut self) -> Result<(), OrderServiceError>;
    async fn get_orders(&self) -> Result<Vec<Order>, OrderServiceError>;
}

pub struct OrderService {
    database_config: configuration::settings::Database,
    collection: Option<Collection<Order>>
}

impl OrderService {
    pub fn new(config: configuration::settings::Database) -> OrderService {
        OrderService {
            database_config: config,
            collection: None
        }
    }
}

#[async_trait]
impl OrderTrait for OrderService {
    async fn init(&mut self) -> Result<(), OrderServiceError>{

        let mut client_options = ClientOptions::parse(&self.database_config.connection_string).await?;
        // Set the server_api field of the client_options object to Stable API version 1
        let server_api = ServerApi::builder().version(ServerApiVersion::V1).build();
        client_options.server_api = Some(server_api);
        // Create a new client and connect to the server
        let client = Client::with_options(client_options)?;
        // Send a ping to confirm a successful connection
        client.database("admin").run_command(doc! { "ping": 1 }, None).await?;
        info!("Pinged your deployment. You successfully connected to MongoDB!");

        self.collection = Some(client.database(&self.database_config.database_name).collection::<Order>(&self.database_config.collection_name));
        Ok(())
    }

    async fn get_orders(&self) -> Result<Vec<Order>, OrderServiceError> {
        let coll = match &self.collection {
            Some(value) => value,
            None => return Ok([].to_vec())
        };
        
        let cursor = coll.find(
            doc! {}, None
        ).await?;
        
        let orders: Vec<Order> = cursor.try_collect().await.expect("");

        Ok(orders.into_iter().collect())
    }
}