extern crate num;

mod configuration;
mod api;
mod constants;
mod models;
mod services;
mod messaging;
mod events;

use std::{env, io};

use configuration::settings::Settings;
use services::orderservicefactory;

#[actix_rt::main]
async fn main() -> io::Result<()> {
    env_logger::init_from_env(env_logger::Env::new().default_filter_or("debug"));
    env::set_var("RUST_BACKTRACE", "1");

    let settings_result: Result<Settings, config::ConfigError> = Settings::new();

    let settings = match settings_result  {
        Err(e) => panic!("Problem loading settings: {:?}", e),
        Ok(s) => s,
    };

    let order_service = match orderservicefactory::create_order_service(settings.clone()).await {
        Err(e) => panic!("Problem constructing order service: {:?}", e),
        Ok(s) => s,
    };

    api::httpserver::start_http_server(settings, order_service).await
}
