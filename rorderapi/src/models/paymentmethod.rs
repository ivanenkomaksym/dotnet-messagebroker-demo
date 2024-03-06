use serde::{Deserialize, Deserializer, Serialize, Serializer};

#[derive(Clone, Debug, FromPrimitive)]
#[repr(u32)]
pub enum PaymentMethod
    {
        CreditCardAlwaysExpire,
        Crypto,
        PayPalAlwaysFail
    }

impl Serialize for PaymentMethod {
    fn serialize<S>(&self, serializer: S) -> Result<S::Ok, S::Error>
    where
        S: Serializer,
    {
        // Convert the enum variant to an integer for serialization
        let variant_int = match self {
            PaymentMethod::CreditCardAlwaysExpire => 0,
            PaymentMethod::Crypto => 1,
            PaymentMethod::PayPalAlwaysFail => 2,
        };

        serializer.serialize_u32(variant_int)
    }
}

impl<'de> Deserialize<'de> for PaymentMethod {
    fn deserialize<D>(deserializer: D) -> Result<Self, D::Error>
    where
        D: Deserializer<'de>,
    {
        let value: u32 = Deserialize::deserialize(deserializer)?;

        match num::FromPrimitive::from_u32(value) {
            Some(status) => Ok(status),
            None => Err(serde::de::Error::custom(format!("Invalid PaymentMethod value: {}", value))),
        }
    }
}