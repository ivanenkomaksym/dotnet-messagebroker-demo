mod configuration;

use configuration::settings::Settings;

fn main() {
    let settings_result: Result<Settings, config::ConfigError> = Settings::new();

    let _settings = match settings_result  {
        Err(e) => panic!("Problem loading settings: {:?}", e),
        Ok(s) => s,
    };
}
