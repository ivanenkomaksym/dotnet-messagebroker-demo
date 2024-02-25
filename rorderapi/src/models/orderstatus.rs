use serde::{Serialize, Deserialize};

#[derive(Serialize, Deserialize, Clone, Debug)]
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