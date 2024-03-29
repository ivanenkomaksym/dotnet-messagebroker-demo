use chrono::{DateTime, Utc};
use serde::{Deserialize, Serialize};

use crate::models::{address::Address, customerinfo::CustomerInfo, orderitem::OrderItem, orderstatus::OrderStatus, paymentinfo::PaymentInfo};

use super::serializers::serialize_datetime;

#[derive(Serialize, Deserialize, Clone, Debug)]
#[serde(rename_all = "PascalCase")]
pub struct OrderCreated {
    pub order_id: bson::Uuid,
    pub customer_info: CustomerInfo,
    pub order_status: OrderStatus,
    pub shipping_address: Address,
    pub payment_info: PaymentInfo,
    pub items: Vec<OrderItem>,
    #[serde(serialize_with = "serialize_datetime")]
    pub creation_date_time: DateTime<Utc>
}