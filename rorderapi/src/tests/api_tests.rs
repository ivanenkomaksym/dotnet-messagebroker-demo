#[cfg(test)]
mod tests {
    use rorderapi::configuration::settings::Settings;
    
    #[actix_web::test]
    async fn test_index_get1() {
    }
    
    fn setup_settings() -> Settings {
        return Settings {
            database: todo!(),
            apiserver: todo!(),
            apisettings: todo!(),
            rabbitmqsettings: todo!(),
        }
    }
}