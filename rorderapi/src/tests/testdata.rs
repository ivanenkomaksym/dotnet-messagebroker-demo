use chrono::Utc;
use models::{address::Address, customerinfodb::CustomerInfoDb, orderdb::OrderDb, orderitemdb::OrderItemDb, orderstatus::OrderStatus, paymentinfo::PaymentInfo};
use rust_decimal::Decimal;

use crate::models;

pub fn create_test_order() -> OrderDb {
    OrderDb {
        id: bson::Uuid::new(),
        order_status: OrderStatus::New,
        customer_info: CustomerInfoDb {
            customer_id: bson::Uuid::new(),
            first_name: Some("Alice".to_string()),
            last_name: Some("Liddell".to_string()),
            email: Some("alice@gmail.com".to_string()),
        },
        items: vec![
            OrderItemDb { 
                id: bson::Uuid::new(),
                product_id: bson::Uuid::new(),
                product_name: Some("Product 1".to_string()),
                product_price: Decimal::new(10, 2),
                quantity: 1,
                image_file: Some("image_file".to_string())
            }
        ],
        total_price: Decimal::new(100, 2), // Example total price with value 100.00
        shipping_address: Address {
            first_name: "Alice".to_string(),
            last_name: "Liddell".to_string(),
            email: "alice@gmail.com".to_string(),
            address_line: "London".to_string(),
            country: "GB".to_string(),
            zip_code: "10000".to_string(),
            // Define Address fields here
        },
        payment_info: PaymentInfo {
            card_name: "Alice Liddell".to_string(),
            card_number: "1234 5678 9101 1121 3141".to_string(),
            expiration: "01/28".to_string(),
            cvv: "123".to_string(),
            payment_method: models::paymentmethod::PaymentMethod::CreditCardAlwaysExpire
            // Define PaymentInfo fields here
        },
        use_cashback: Decimal::new(10, 2), // Example use cashback with value 10.00
        creation_date_time: Utc::now().into(), // Example creation date time as current UTC time
    }
}