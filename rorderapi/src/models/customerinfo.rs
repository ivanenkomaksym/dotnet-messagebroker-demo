use bson::doc;
use serde::{Serialize, Deserialize};

#[derive(Serialize, Deserialize, Clone, Debug)]
#[serde(rename_all = "PascalCase")]
pub struct CustomerInfo {
    pub customer_id: bson::Uuid,
    pub first_name: Option<String>,
    pub last_name: Option<String>,
    pub email: Option<String>
}