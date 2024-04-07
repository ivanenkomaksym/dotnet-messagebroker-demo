use config::{Config, ConfigError, File};
use serde::Deserialize;
use std::env;
use clap::Parser;

#[derive(Clone, Debug, Deserialize)]
#[allow(unused)]
pub struct ApiServer {
    pub application_url: String,
    pub hostname: String
}

#[derive(Clone, Debug, Deserialize)]
#[allow(unused)]
pub struct ApiSettings {
    pub gateway_address: String
}

#[derive(Clone, Debug, Deserialize)]
#[allow(unused)]
pub struct Database {
    pub connection_string: String,
    pub database_name: String,
    pub collection_name: String
}

#[derive(Clone, Debug, Deserialize)]
#[allow(unused)]
pub struct RabbitMQSettings {
    pub ampq_connection_string: String
}

#[derive(Clone, Debug, Deserialize)]
#[allow(unused)]
pub struct Settings {
    pub apiserver: ApiServer,
    pub apisettings: ApiSettings,
    pub database: Database,
    pub rabbitmqsettings: RabbitMQSettings
}

#[derive(Parser)]
struct Args {
    /// Address this service will be running on
    #[arg(short, long)]
    application_url: Option<String>
}

impl Settings {
    pub fn new() -> Result<Self, ConfigError> {
        let run_mode = env::var("RUN_MODE").unwrap_or_else(|_| "development".into());

        let mut config_builder = Box::new(Config::builder()
            // Start off by merging in the "default" configuration file
            //.add_source(File::with_name("config/default").required(false))
            // Add in the current environment file
            // Default to 'development' env
            // Note that this file is _optional_
            .add_source(
                File::with_name(&format!("src/configuration/{}.toml", run_mode))
                    .required(true),
            ));

        let args = Args::parse();

        if let Some(value) = args.application_url {
            config_builder = Box::new(config_builder.clone()
                .set_override("apiserver.application_url", value.clone())?
                .set_override("apiserver.hostname", value)?);
        }

        let config = config_builder
            // Add in a local configuration file
            // This file shouldn't be checked in to git
            //.add_source(File::with_name("config/local").required(false))
            // Add in settings from the environment (with a prefix of APP)
            // Eg.. `APP_DEBUG=1 ./target/app` would set the `debug` key
            //.add_source(Environment::with_prefix("app"))
            // You may also programmatically change settings
            // .set_override("database.url", "postgres://")?
            .build()?;

        // You can deserialize (and thus freeze) the entire configuration as
        config.try_deserialize()
    }
}