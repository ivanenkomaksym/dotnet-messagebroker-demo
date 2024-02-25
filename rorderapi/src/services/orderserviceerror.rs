use std::io;

use thiserror::Error;

#[derive(Error, Debug)]
pub enum OrderServiceError {
    #[error("service connection error")]
    ConnectionError(#[from] mongodb::error::Error),
    #[error("internal error")]
    IOError(#[from] io::Error),
    #[error("unknown data store error")]
    InternalHttpClientError(#[from] reqwest::Error)
}