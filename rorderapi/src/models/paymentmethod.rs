use serde::{Serialize, Deserialize, Deserializer};

#[derive(Serialize, Clone, Debug, FromPrimitive)]
pub enum PaymentMethod
    {
        CreditCardAlwaysExpire,
        Crypto,
        PayPalAlwaysFail
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