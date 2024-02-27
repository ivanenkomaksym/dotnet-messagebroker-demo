use bson::doc;
use serde::{Serialize, Deserialize};

#[derive(Serialize, Deserialize, Clone, Debug)]
#[serde(rename_all = "PascalCase")]
pub struct Address {
    pub first_name: String,
    pub last_name: String,
    pub email: String,
    pub address_line: String,
    pub country: String,
    pub zip_code: String,
}