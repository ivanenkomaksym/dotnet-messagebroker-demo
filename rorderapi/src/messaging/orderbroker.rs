use chrono::Local;

use crate::{configuration, events::{cancelorder::CancelOrder, ordercollected::OrderCollected, ordercreated::OrderCreated, orderupdated::OrderUpdated, returnorder::ReturnOrder}, models::order::Order};

use super::{constants::{CANCEL_ORDER_EXCHANGE, ORDER_COLLECTED_EXCHANGE, ORDER_CREATED_EXCHANGE, ORDER_UPDATED_EXCHANGE, RETURN_ORDER_EXCHANGE}, publisher};

pub async fn create_order(config: &configuration::settings::RabbitMQSettings, order: &Order) -> Result<(), lapin::Error>
{
    let order_created_event = OrderCreated { 
        order_id: order.id, 
        customer_info: order.customer_info.clone(), 
        order_status: order.order_status.clone(), 
        shipping_address: order.shipping_address.clone(), 
        payment_info: order.payment_info.clone(), 
        items: order.items.clone(),
        creation_date_time: order.creation_date_time
    };

    Ok(publisher::publish_event(&config, ORDER_CREATED_EXCHANGE, order_created_event).await?)
}

pub async fn update_order(config: &configuration::settings::RabbitMQSettings, order: &Order) -> Result<(), lapin::Error>
{
    let order_updated_event = OrderUpdated { 
        order_id: order.id, 
        customer_info: order.customer_info.clone(), 
        order_status: order.order_status.clone(), 
        shipping_address: order.shipping_address.clone(), 
        payment_info: order.payment_info.clone(), 
        items: order.items.clone(),
        // TODO: Fill updated payment_status
        payment_status: crate::models::paymentstatus::PaymentStatus::Paid
    };

    Ok(publisher::publish_event(&config, ORDER_UPDATED_EXCHANGE, order_updated_event).await?)
}

pub async fn cancel_order(config: &configuration::settings::RabbitMQSettings, order_id: bson::Uuid) -> Result<(), lapin::Error>
{
    let cancel_order_event = CancelOrder { 
        order_id, 
        cancel_date_time: Local::now().to_utc()
    };

    Ok(publisher::publish_event(&config, CANCEL_ORDER_EXCHANGE, cancel_order_event).await?)
}

pub async fn order_collected(config: &configuration::settings::RabbitMQSettings, order_id: bson::Uuid) -> Result<(), lapin::Error>
{
    let order_collected_event = OrderCollected { 
        order_id, 
        collected_date_time: Local::now().to_utc()
    };

    Ok(publisher::publish_event(&config, ORDER_COLLECTED_EXCHANGE, order_collected_event).await?)
}

pub async fn return_order(config: &configuration::settings::RabbitMQSettings, order_id: bson::Uuid) -> Result<(), lapin::Error>
{
    let return_order_event = ReturnOrder { 
        order_id, 
        return_date_time: Local::now().to_utc()
    };

    Ok(publisher::publish_event(&config, RETURN_ORDER_EXCHANGE, return_order_event).await?)
}