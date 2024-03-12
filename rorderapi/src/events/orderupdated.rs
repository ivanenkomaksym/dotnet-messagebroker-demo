use serde::{Deserialize, Serialize};

use crate::models::{address::Address, customerinfo::CustomerInfo, orderitem::OrderItem, orderstatus::OrderStatus, paymentinfo::PaymentInfo, paymentstatus::PaymentStatus};

#[derive(Serialize, Deserialize, Clone, Debug)]
#[serde(rename_all = "PascalCase")]
pub struct OrderUpdated {
    pub order_id: bson::Uuid,
    pub payment_status: PaymentStatus,
    pub customer_info: CustomerInfo,
    pub order_status: OrderStatus,
    pub shipping_address: Address,
    pub payment_info: PaymentInfo,
    pub items: Vec<OrderItem>,
}