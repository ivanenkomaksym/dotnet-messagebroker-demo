use rust_decimal::prelude::*;
use serde::{Deserialize, Serialize};
use chrono::{DateTime, Utc};

use super::{address::Address, customerinfo::CustomerInfo, orderitem::OrderItem, orderstatus::OrderStatus, paymentinfo::PaymentInfo};

#[derive(Serialize, Deserialize, Clone, Debug)]
#[serde(rename_all = "PascalCase")]
pub struct Order {
    pub id: bson::Uuid,
    pub order_status: OrderStatus,
    pub customer_info: CustomerInfo,
    pub items: Vec<OrderItem>,
    pub total_price: Decimal,
    pub shipping_address: Address,
    pub payment_info: PaymentInfo,
    pub use_cashback: Decimal,
    #[serde(default = "Utc::now")]
    pub creation_date_time: DateTime<Utc>
}