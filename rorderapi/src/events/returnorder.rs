use chrono::{DateTime, Utc};
use serde::{Deserialize, Serialize};

use super::serializers::serialize_datetime;

#[derive(Serialize, Deserialize, Clone, Debug)]
#[serde(rename_all = "PascalCase")]
pub struct ReturnOrder {
    pub order_id: bson::Uuid,
    #[serde(serialize_with = "serialize_datetime")]
    pub return_date_time: DateTime<Utc>
}