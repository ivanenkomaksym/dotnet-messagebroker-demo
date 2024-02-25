mod configuration;
mod api;
mod constants;

use std::io;

use configuration::settings::Settings;

#[actix_rt::main]
async fn main() -> io::Result<()> {
    //env::set_var("RUST_LOG", "actix_web = debug, actix_server = info");
    env_logger::init_from_env(env_logger::Env::new().default_filter_or("info"));

    let settings_result: Result<Settings, config::ConfigError> = Settings::new();

    let settings = match settings_result  {
        Err(e) => panic!("Problem loading settings: {:?}", e),
        Ok(s) => s,
    };

    match api::httpserver::start_http_server(settings).await {
        Ok(_) => todo!(),
        Err(e)  => panic!("Problem starting server: {:?}", e),
    }
}
