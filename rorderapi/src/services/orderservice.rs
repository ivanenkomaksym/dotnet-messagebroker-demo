use crate::{configuration::settings::Settings, models::order::Order};
use mongodb::{ bson::doc, options::{ ClientOptions, ServerApi, ServerApiVersion }, Client };
use log::info;

use super::orderserviceerror::OrderServiceError;

pub async fn init(settings: &Settings) -> Result<(), OrderServiceError>{

    let mut client_options = ClientOptions::parse(&settings.database.connection_string).await?;
    // Set the server_api field of the client_options object to Stable API version 1
    let server_api = ServerApi::builder().version(ServerApiVersion::V1).build();
    client_options.server_api = Some(server_api);
    // Create a new client and connect to the server
    let client = Client::with_options(client_options)?;
    // Send a ping to confirm a successful connection
    client.database("admin").run_command(doc! { "ping": 1 }, None).await?;
    info!("Pinged your deployment. You successfully connected to MongoDB!");

    let _collection = Some(client.database(&settings.database.database_name).collection::<Order>(&settings.database.collection_name));
    Ok(())
}