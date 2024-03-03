use crate::configuration::settings::Settings;
use crate::constants::APPLICATION_JSON;
use crate::models::order::Order;
use crate::services::orderservice::OrderTrait;

use actix_web::{delete, middleware, post};
use actix_web::{App, HttpResponse, get, web};
use actix_web::HttpServer;
use bson::Uuid;
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
            .service(get_orders)
            .service(get_order_byid)
            .service(get_orders_by_customerid)
            .service(create_order)
            .service(delete_order)
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
async fn get_orders(appdata: web::Data<Mutex<AppData>>) -> HttpResponse {
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
    let orderid    = match convert_to_uuid(path) {
        Err(err) => return err,
        Ok(result) => result
    };
    
    match appdata.lock().unwrap().order_service.find(&orderid).await {
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

#[get("/Order/GetOrdersByCustomerId/{customerid}")]
async fn get_orders_by_customerid(path: web::Path<String>, appdata: web::Data<Mutex<AppData>>) -> HttpResponse {
    let customerid    = match convert_to_uuid(path) {
        Err(err) => return err,
        Ok(result) => result
    };
    
    match appdata.lock().unwrap().order_service.get_orders_by_customerid(&customerid).await {
        Err(err) => {
            log::error!("{}", err);
            return HttpResponse::InternalServerError().body(err.to_string());
        }
        Ok(orders) => {
            HttpResponse::Ok()
                .content_type(APPLICATION_JSON)
                .json(orders)
        }
    }
}

#[post("/Order")]
async fn create_order(new_order: web::Json<Order>, appdata: web::Data<Mutex<AppData>>) -> HttpResponse {
    match appdata.lock().unwrap().order_service.create_order(new_order.0).await {
        Err(err) => {
            log::error!("{}", err);
            return HttpResponse::InternalServerError()
                .finish();
        }
        Ok(_) => {
            return HttpResponse::Created()
            .finish();
        }
    }
}

#[delete("/Order/{orderid}")]
async fn delete_order(path: web::Path<String>, appdata: web::Data<Mutex<AppData>>) -> HttpResponse {
    let orderid = match convert_to_uuid(path) {
        Err(err) => return err,
        Ok(result) => result
    };
    
    match appdata.lock().unwrap().order_service.delete_order(&orderid).await {
        Err(err) => {
            log::error!("{}", err);
            return HttpResponse::InternalServerError()
                .finish();
        }
        Ok(_) => {
            return HttpResponse::NoContent()
            .finish();
        }
    }
}

fn convert_to_uuid(path: web::Path<String>) -> Result<Uuid, HttpResponse>
{
    let orderid = path.into_inner();
    if orderid.is_empty() {
        return Err(HttpResponse::BadRequest()
            .finish());
    }
    
    match bson::Uuid::parse_str(orderid) {
        Ok(result) => return Ok(result),
        Err(err) => {
            return Err(HttpResponse::BadRequest().body(err.to_string()))
        }
    };
}