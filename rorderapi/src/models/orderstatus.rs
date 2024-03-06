use serde::{Deserialize, Deserializer, Serialize, Serializer};

#[derive(Clone, Debug, FromPrimitive)]
#[repr(u32)]
pub enum OrderStatus
    {
        New,
        StockReserveFailed,
        PaymentProcessing,
        PaymentFailed,
        Paid,
        AwaitingShipment,
        Shipping,
        AwaitingCollection,
        Completed,
        AwaitingReturn,
        Refunding,
        Refunded,
        Cancelled
    }

impl Serialize for OrderStatus {
    fn serialize<S>(&self, serializer: S) -> Result<S::Ok, S::Error>
    where
        S: Serializer,
    {
        // Convert the enum variant to an integer for serialization
        let variant_int = match self {
            OrderStatus::New => 0,
            OrderStatus::StockReserveFailed => 1,
            OrderStatus::PaymentProcessing => 2,
            OrderStatus::PaymentFailed => 3,
            OrderStatus::Paid => 4,
            OrderStatus::AwaitingShipment => 5,
            OrderStatus::Shipping => 6,
            OrderStatus::AwaitingCollection => 7,
            OrderStatus::Completed => 8,
            OrderStatus::AwaitingReturn => 9,
            OrderStatus::Refunding => 10,
            OrderStatus::Refunded => 11,
            OrderStatus::Cancelled => 12,
        };

        serializer.serialize_u32(variant_int)
    }
}

impl<'de> Deserialize<'de> for OrderStatus {
    fn deserialize<D>(deserializer: D) -> Result<Self, D::Error>
    where
        D: Deserializer<'de>,
    {
        let value: u32 = Deserialize::deserialize(deserializer)?;

        match num::FromPrimitive::from_u32(value) {
            Some(status) => Ok(status),
            None => Err(serde::de::Error::custom(format!("Invalid OrderStatus value: {}", value))),
        }
    }
}