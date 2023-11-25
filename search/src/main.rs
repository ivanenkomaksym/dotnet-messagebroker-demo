pub mod constants;

#[macro_use]
extern crate actix_web;

use std::{env, io};

use crate::constants::APPLICATION_JSON;

use actix_web::{middleware, App, HttpServer, HttpResponse};

use serde::{Serialize, Deserialize};

#[derive(Serialize, Deserialize, Debug)]
struct Response {
    message: String
}

#[actix_rt::main]
async fn main() -> io::Result<()> {
    env::set_var("RUST_LOG", "actix_web=debug,actix_server=info");
    env_logger::init();

    HttpServer::new(|| {
        App::new()
            // enable logger - always register actix-web Logger middleware last
            .wrap(middleware::Logger::default())
            // register HTTP requests handlers
            .service(hello)
    })
    .bind("localhost:80")?
    .run()
    .await
}

#[get("/hello")]
pub async fn hello() -> HttpResponse {
    let message = String::from("hello");

    HttpResponse::Ok()
        .content_type(APPLICATION_JSON)
        .json(Response { message: String::from("hello")})
}