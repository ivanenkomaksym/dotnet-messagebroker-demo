use mongodb::{Client, Database, bson::doc};

use crate::settings::Settings;

pub async fn create_client(settings: &Settings) -> mongodb::error::Result<Database> {
    // Create a new client and connect to the server
    let client = Client::with_uri_str(&settings.database.connection_string).await?;

    // Get a handle on the movies collection
    let database = client.database(&settings.database.database_name);

    database.run_command(doc! { "ping": 1 }, None).await?;

    println!("You successfully connected to MongoDB!");

    return Ok(database);
}