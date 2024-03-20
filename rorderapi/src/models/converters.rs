pub(crate) use super::order::Order;
use super::{customerinfo::CustomerInfo, customerinfodb::CustomerInfoDb, orderdb::OrderDb, orderitem::OrderItem, orderitemdb::OrderItemDb};

pub fn to_order(order: &OrderDb) -> Order {
    return Order {
        id: order.id,
        order_status: order.order_status.clone(),
        customer_info: CustomerInfo {
            customer_id: order.customer_info.customer_id,
            first_name: order.customer_info.first_name.clone(),
            last_name: order.customer_info.last_name.clone(),
            email: order.customer_info.email.clone(),
        },
        items: order.items.iter().map(|orderitem| to_orderitem(&orderitem)).collect(),
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
        customer_info: CustomerInfoDb {
            customer_id: order.customer_info.customer_id,
            first_name: order.customer_info.first_name.clone(),
            last_name: order.customer_info.last_name.clone(),
            email: order.customer_info.email.clone(),
        },
        items: order.items.iter().map(|orderitem| to_orderitem_db(&orderitem)).collect(),
        total_price: order.total_price,
        shipping_address: order.shipping_address.clone(),
        payment_info: order.payment_info.clone(),
        use_cashback: order.use_cashback,
        creation_date_time: order.creation_date_time.into(),
    }
}

fn to_orderitem(orderitem: &OrderItemDb) -> OrderItem {
    return OrderItem {
        id: orderitem.id,
        product_id: orderitem.product_id,
        product_name: orderitem.product_name.clone(),
        product_price: orderitem.product_price,
        quantity: orderitem.quantity,
        image_file: orderitem.image_file.clone(),
    }
}

fn to_orderitem_db(orderitem: &OrderItem) -> OrderItemDb {
    return OrderItemDb {
        id: orderitem.id,
        product_id: orderitem.product_id,
        product_name: orderitem.product_name.clone(),
        product_price: orderitem.product_price,
        quantity: orderitem.quantity,
        image_file: orderitem.image_file.clone(),
    }
}