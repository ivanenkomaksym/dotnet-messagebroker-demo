use bson::doc;
use serde::{Serialize, Deserialize};

use super::{orderitem::OrderItem, orderstatus::OrderStatus};

#[derive(Serialize, Deserialize, Clone, Debug)]
#[serde(rename_all = "PascalCase")]
pub struct Order {
    pub id: bson::Uuid,
    pub order_status: OrderStatus,
    pub items: Vec<OrderItem>
}