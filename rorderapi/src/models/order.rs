use bson::doc;
use rust_decimal::prelude::*;
use serde::{Deserialize, Serialize};
use chrono::{DateTime, Utc};

use super::{address::Address, customerinfo::CustomerInfo, orderitem::OrderItem, orderstatus::OrderStatus, paymentinfo::PaymentInfo};

#[derive(Serialize, Deserialize, Clone, Debug)]
#[serde(rename_all = "PascalCase")]
pub struct Order {
    #[serde(rename = "_id")] 
    #[serde(default)]
    pub id: bson::Uuid,
    pub order_status: OrderStatus,
    pub customer_info: CustomerInfo,
    pub items: Vec<OrderItem>,
    pub total_price: Decimal,
    pub shipping_address: Address,
    pub payment_info: PaymentInfo,
    pub use_cashback: Decimal,
    #[serde(default)]
    #[serde(with = "bson::serde_helpers::chrono_datetime_as_bson_datetime")]
    pub creation_date_time: DateTime<Utc>
}