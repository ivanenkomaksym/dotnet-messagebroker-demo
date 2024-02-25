mod configuration;
mod api;
mod constants;
mod models;
mod services;

use std::io;

use configuration::settings::Settings;
use services::orderservice;

#[actix_rt::main]
async fn main() -> io::Result<()> {
    env_logger::init_from_env(env_logger::Env::new().default_filter_or("info"));

    let settings_result: Result<Settings, config::ConfigError> = Settings::new();

    let settings = match settings_result  {
        Err(e) => panic!("Problem loading settings: {:?}", e),
        Ok(s) => s,
    };

    let _init_result = orderservice::init(&settings).await;

    api::httpserver::start_http_server(settings).await
}
