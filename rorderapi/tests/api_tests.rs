#[cfg(test)]
mod tests {
    use std::sync::Mutex;

    use actix_web::{middleware, test, web, App};
    use rorderapi::{api::httpserver::{hello, AppData}, services::orderservice::MockOrderTrait};
    
    #[actix_web::test]
    async fn test_index_get() {
        let mock_order_service = MockOrderTrait::new();

        let appdata = web::Data::new(Mutex::new(AppData {order_service: Box::new(mock_order_service)}));
        
        let app = test::init_service({
            App::new()
                // enable logger - always register actix-web Logger middleware last
                .wrap(middleware::Logger::default())
                // register HTTP requests handlers
                .service(hello)
                .app_data(web::Data::clone(&appdata))
        }).await;
        
        let req = test::TestRequest::get().uri("/hello").to_request();
        let resp = test::call_service(&app, req).await;
        assert!(resp.status().is_success());
    }
}