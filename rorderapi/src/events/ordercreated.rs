use bson::DateTime;
use serde::{Serialize, Deserialize};

use crate::models::{address::Address, customerinfo::CustomerInfo, orderitem::OrderItem, orderstatus::OrderStatus, paymentinfo::PaymentInfo};

#[derive(Serialize, Deserialize, Clone, Debug)]
#[serde(rename_all = "PascalCase")]
pub struct OrderCreated {
    pub order_id: bson::Uuid,
    pub customer_info: CustomerInfo,
    pub order_status: OrderStatus,
    pub shipping_address: Address,
    pub payment_info: PaymentInfo,
    pub items: Vec<OrderItem>,
    pub creation_date_time: DateTime
}