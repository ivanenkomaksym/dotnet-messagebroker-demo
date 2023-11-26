use mongodb::bson::Binary;
use serde::{Serialize, Deserialize, Serializer};

use uuid::Uuid;

#[derive(Debug, Serialize, Deserialize)]
pub struct Product {
    //pub _id: Oid,
    #[serde(rename = "_id", serialize_with = "serialize_uuid_id")]
    pub _id: Binary,
    #[serde(rename = "Name")]
    pub name: String,
    #[serde(rename = "Category")]
    pub category: String,
    #[serde(rename = "Summary")]
    pub summary: String,
    #[serde(rename = "ImageFile")]
    pub image_file: String,
    #[serde(rename = "Price")]
    pub price: f32
}

impl Product {
    pub fn get_uuid_id(&self) -> Uuid {
        // Assuming that Binary subtype Uuid is stored in the "_id" field
        let bytes = self._id.bytes.clone();
        Uuid::from_slice(&bytes).unwrap_or_default()
    }
}

// Serialize implementation for converting Binary to UUID String
fn serialize_uuid_id<S>(id: &Binary, serializer: S) -> Result<S::Ok, S::Error>
where
    S: Serializer,
{
    let uuid = Uuid::from_slice(&id.bytes).map_err(serde::ser::Error::custom)?;
    uuid.to_string().serialize(serializer)
}