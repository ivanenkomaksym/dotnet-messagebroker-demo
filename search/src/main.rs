pub mod constants;
pub mod settings;
pub mod data;
pub mod api;
pub mod models;

#[macro_use]
extern crate actix_web;

use std::{env,io};

use data::productrepository;
use mongodb::bson::Document;
use settings::Settings;

#[actix_rt::main]
async fn main() -> io::Result<()> {
    let settings_result: Result<Settings, config::ConfigError> = Settings::new();

    let settings = match settings_result  {
        Err(e) => panic!("Problem loading settings: {:?}", e),
        Ok(s) => s,
    };

    env::set_var("RUST_LOG", "actix_web=debug,actix_server=info");
    env_logger::init();

    let database = match data::dataclient::create_client(&settings).await {
        Err(e) => panic!("Failed to connect to database: {:?}", e),
        Ok(r) => r,
    };

    let products_collection = database.collection::<Document>(&settings.database.collection_name);
    let _products = productrepository::get_products(&products_collection).await;
    
    api::httpserver::start_http_server(&settings, products_collection).await
}
