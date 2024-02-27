use bson::doc;
use serde::{Serialize, Deserialize};

use super::paymentmethod::PaymentMethod;

#[derive(Serialize, Deserialize, Clone, Debug)]
#[serde(rename_all = "PascalCase")]
pub struct PaymentInfo {
    pub card_name: String,
    pub card_number: String,
    pub expiration: String,
    #[serde(rename = "CVV")] 
    pub cvv: String,
    pub payment_method: PaymentMethod
}