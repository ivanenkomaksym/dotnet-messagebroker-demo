pub(crate) use super::order::Order;
use super::orderdb::OrderDb;

pub fn to_order(order: &OrderDb) -> Order {
    return Order {
        id: order.id,
        order_status: order.order_status.clone(),
        customer_info: order.customer_info.clone(),
        items: order.items.clone(),
        total_price: order.total_price,
        shipping_address: order.shipping_address.clone(),
        payment_info: order.payment_info.clone(),
        use_cashback: order.use_cashback,
        creation_date_time: order.creation_date_time.into(),
    }
}

pub fn to_order_db(order: &Order) -> OrderDb {
    return OrderDb {
        id: order.id,
        order_status: order.order_status.clone(),
        customer_info: order.customer_info.clone(),
        items: order.items.clone(),
        total_price: order.total_price,
        shipping_address: order.shipping_address.clone(),
        payment_info: order.payment_info.clone(),
        use_cashback: order.use_cashback,
        creation_date_time: order.creation_date_time.into(),
    }
}