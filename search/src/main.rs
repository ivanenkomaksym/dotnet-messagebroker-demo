pub mod constants;
pub mod settings;
pub mod data;
pub mod api;

#[macro_use]
extern crate actix_web;

use std::{env, io};

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

    match data::dataclient::create_client(&settings).await {
        Err(e) => panic!("Failed to connect to database: {:?}", e),
        Ok(r) => r,
    };
    
    api::httpserver::start_http_server(&settings).await
}