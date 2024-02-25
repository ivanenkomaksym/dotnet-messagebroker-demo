use crate::configuration::settings::Settings;
use crate::constants::APPLICATION_JSON;

use actix_web::middleware;
use actix_web::{App, HttpResponse, get};
use actix_web::HttpServer;
use serde::{Serialize, Deserialize};
use std::io;

#[derive(Serialize, Deserialize, Debug)]
struct Response {
    message: String
}

pub async fn start_http_server(settings: Settings) -> io::Result<()> {
    let application_url = settings.apiserver.application_url.clone();

    HttpServer::new(move|| {
        App::new()
            // enable logger - always register actix-web Logger middleware last
            .wrap(middleware::Logger::default())
            // register HTTP requests handlers
            .service(hello)
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