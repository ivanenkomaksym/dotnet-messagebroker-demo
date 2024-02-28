use crate::configuration::settings::Settings;
use crate::constants::APPLICATION_JSON;
use crate::services::orderservice::OrderTrait;

use actix_web::middleware;
use actix_web::{App, HttpResponse, get, web};
use actix_web::HttpServer;
use serde::{Serialize, Deserialize};
use std::io;
use std::sync::Mutex;

#[derive(Serialize, Deserialize, Debug)]
struct Response {
    message: String
}

pub struct AppData {
    pub order_service: Box<dyn OrderTrait>
}

pub async fn start_http_server(settings: Settings, order_service: Box<dyn OrderTrait>) -> io::Result<()> {
    let application_url = settings.apiserver.application_url.clone();

    let appdata = web::Data::new(Mutex::new(AppData { order_service }));

    HttpServer::new(move|| {
        App::new()
            // enable logger - always register actix-web Logger middleware last
            .wrap(middleware::Logger::default())
            // register HTTP requests handlers
            .service(hello)
            .service(orders)
            .service(get_order_byid)
            .app_data(web::Data::clone(&appdata))
    })
    .bind(application_url)?
    .run()
    .await
}

#[get("/hello")]
async fn hello() -> HttpResponse {
HttpResponse::Ok()
    .content_type(APPLICATION_JSON)
    .json(Response { message: String::from("hello")})
}

#[get("/Order")]
async fn orders(appdata: web::Data<Mutex<AppData>>) -> HttpResponse {
    match appdata.lock().unwrap().order_service.get_orders().await {
        Err(err) => {
            log::error!("{}", err);
            return HttpResponse::InternalServerError()
                .finish();
        }
        Ok(orders) => {
            HttpResponse::Ok()
                .content_type(APPLICATION_JSON)
                .json(orders)
        }
    }
}

#[get("/Order/{orderid}")]
async fn get_order_byid(path: web::Path<String>, appdata: web::Data<Mutex<AppData>>) -> HttpResponse {
    let orderid = path.into_inner();
    if orderid.is_empty() {
        return HttpResponse::BadRequest()
            .finish();
    }
    
    let uuid = match bson::Uuid::parse_str(orderid) {
        Ok(result) => result,
        Err(err) => {
            return HttpResponse::BadRequest().body(err.to_string())
        }
    };
    
    match appdata.lock().unwrap().order_service.find(&uuid).await {
        None => {
            return HttpResponse::NotFound()
                .finish();
        }
        Some(order) => {
            HttpResponse::Ok()
                .content_type(APPLICATION_JSON)
                .json(order)
        }
    }
}