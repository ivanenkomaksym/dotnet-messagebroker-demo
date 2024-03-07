use bson::doc;
use serde::{Serialize, Deserialize};

#[derive(Serialize, Deserialize, Clone, Debug)]
#[serde(rename_all = "PascalCase")]
pub struct CustomerInfo {
    #[serde(rename(serialize = "CustomerId", deserialize = "_id"))]
    pub customer_id: bson::Uuid,
    pub first_name: Option<String>,
    pub last_name: Option<String>,
    pub email: Option<String>
}