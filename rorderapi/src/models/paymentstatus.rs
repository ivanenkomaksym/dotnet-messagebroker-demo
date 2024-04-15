use num_derive::FromPrimitive;
use serde::{Deserialize, Deserializer, Serialize, Serializer};

#[derive(Clone, Debug, FromPrimitive)]
#[repr(u32)]
pub enum PaymentStatus
    {
        Unpaid,
        Failed,
        Expired,
        Paid,
        Refunding,
        Refunded
    }

impl Serialize for PaymentStatus {
    fn serialize<S>(&self, serializer: S) -> Result<S::Ok, S::Error>
    where
        S: Serializer,
    {
        // Convert the enum variant to an integer for serialization
        let variant_int = match self {
            PaymentStatus::Unpaid => 0,
            PaymentStatus::Failed => 1,
            PaymentStatus::Expired => 2,
            PaymentStatus::Paid => 2,
            PaymentStatus::Refunding => 2,
            PaymentStatus::Refunded => 2,
        };

        serializer.serialize_u32(variant_int)
    }
}

impl<'de> Deserialize<'de> for PaymentStatus {
    fn deserialize<D>(deserializer: D) -> Result<Self, D::Error>
    where
        D: Deserializer<'de>,
    {
        let value: u32 = Deserialize::deserialize(deserializer)?;

        match num::FromPrimitive::from_u32(value) {
            Some(status) => Ok(status),
            None => Err(serde::de::Error::custom(format!("Invalid PaymentStatus value: {}", value))),
        }
    }
}