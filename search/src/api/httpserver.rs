use crate::{constants::APPLICATION_JSON, settings::Settings, data::productrepository};

use actix_web::{middleware, App, HttpServer, HttpResponse, web};
use mongodb::{Collection, bson::Document};

use std::io;

use serde::{Serialize, Deserialize};

#[derive(Serialize, Deserialize, Debug)]
struct Response {
    message: String
}

pub async fn start_http_server(settings: &Settings, products_collection: Collection::<Document>) -> io::Result<()> {
    HttpServer::new(move || {
        App::new()
            // enable logger - always register actix-web Logger middleware last
            .wrap(middleware::Logger::default())
            // register HTTP requests handlers
            .service(hello)
            // Register the products route
            .service(products)
            // Add the products_collection as shared state accessible by handlers
            .app_data(web::Data::new(products_collection.clone()))
    })
    .bind(&settings.apiserver.application_url)?
    .run()
    .await
}

#[get("/hello")]
async fn hello() -> HttpResponse {
HttpResponse::Ok()
    .content_type(APPLICATION_JSON)
    .json(Response { message: String::from("hello")})
}

#[get("/products")]
async fn products(products_collection: web::Data<Collection<Document>>) -> HttpResponse {

    let products = productrepository::get_products(&products_collection).await.unwrap();
HttpResponse::Ok()
    .content_type(APPLICATION_JSON)
    .json(products)
}