use bson::doc;
use rust_decimal::prelude::*;
use serde::{Serialize, Deserialize};

#[derive(Serialize, Deserialize, Clone, Debug)]
#[serde(rename_all = "PascalCase")]
pub struct OrderItem {
    pub id: bson::Uuid,
    pub product_id: bson::Uuid,
    pub product_name: Option<String>,
    #[serde(with = "rust_decimal::serde::float")]
    pub product_price: Decimal,
    pub quantity: u16,
    pub image_file: Option<String>
}