use chrono::{DateTime, Utc};
use serde::{Deserialize, Serialize, Serializer};

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
    #[serde(serialize_with = "serialize_datetime")]
    pub creation_date_time: DateTime<Utc>
}

// Custom serializer for DateTime<Utc>
pub fn serialize_datetime<S>(date_time: &DateTime<Utc>, serializer: S) -> Result<S::Ok, S::Error>
where
    S: Serializer,
{
    let formatted = date_time.format("%Y-%m-%dT%H:%M:%S%.6f%:z").to_string();
    serializer.serialize_str(&formatted)
}
