use crate::configuration;
use super::{orderservice, orderserviceerror::OrderServiceError};

pub async fn create_order_service(config: configuration::settings::Settings) -> Result<Box<dyn orderservice::OrderTrait>, OrderServiceError> {
    let mut order_service: Box<dyn orderservice::OrderTrait> = Box::new(orderservice::OrderService::new(config));
    order_service.init().await?;
    Ok(order_service)
}