use crate::{configuration, models::order::Order};
use mongodb::{ bson::doc, options::{ ClientOptions, FindOptions, ServerApi, ServerApiVersion }, Client, Collection };
use async_trait::async_trait;
use log::info;
use futures_util::TryStreamExt;

use super::orderserviceerror::OrderServiceError;

#[async_trait]
pub trait OrderTrait: Send + Sync {

    async fn init(&mut self) -> Result<(), OrderServiceError>;
    async fn get_orders(&self) -> Result<Vec<Order>, OrderServiceError>;
    async fn find(&mut self, uuid: &bson::Uuid) -> Option<Order>;
    async fn get_orders_by_customerid(&self, customerid: &bson::Uuid) -> Result<Vec<Order>, OrderServiceError>;
    async fn create_order(&mut self, new_order: Order) -> Result<(), OrderServiceError>;
    async fn delete_order(&mut self, uuid: &bson::Uuid) -> Result<(), OrderServiceError>;
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
        
        let orders: Vec<Order> = cursor.try_collect().await?;

        Ok(orders.into_iter().collect())
    }

    async fn find(&mut self, uuid: &bson::Uuid) -> Option<Order> {
        let find_result = self.collection.as_mut().unwrap().find_one(
            doc! { "_id": uuid }, None
        ).await;

        match find_result {
            Ok(option_result) => {
                return option_result
            },
            Err(err) => {
                log::warn!("{}", err);
                return None
            }
        }
    }

    async fn get_orders_by_customerid(&self, customerid: &bson::Uuid) -> Result<Vec<Order>, OrderServiceError> {
        let coll = match &self.collection {
            Some(value) => value,
            None => return Ok([].to_vec())
        };

        // Define the query to find orders by customer ID
        let query = doc! {
            "CustomerInfo._id": customerid
        };

        // Set any additional options for the find operation
        let find_options = FindOptions::builder().build();

        // Perform the find operation
        let cursor = coll.find(query, find_options).await?;

        // Convert the cursor to a vector of Order structs
        let orders: Vec<Order> = cursor.try_collect().await?;

        Ok(orders.into_iter().collect())
    }

    async fn create_order(&mut self, new_order: Order) -> Result<(), OrderServiceError>
    {
        self.collection.as_mut().unwrap().insert_one(new_order, None).await?;
        Ok(())
    }

    async fn delete_order(&mut self, uuid: &bson::Uuid) -> Result<(), OrderServiceError>
    {
        let res = self.collection.as_mut().unwrap().delete_one(doc! { "_id": uuid }, None).await?;
        if res.deleted_count == 0 {
            return Err(OrderServiceError::NotFound)
        }

        Ok(())
    }
}