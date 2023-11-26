use mongodb::{Collection, bson::Document};
use futures::TryStreamExt;
use models::product::Product;
use mongodb::bson;

use crate::models::{self};

pub async fn get_products(collection: &Collection::<Document>) -> mongodb::error::Result<Vec<Product>> {
    let mut cursor = collection.find(None, None).await?;

    let mut products: Vec<Product> = vec![];
    while let Some(result) = cursor.try_next().await? {
        //println!("{:?}", result);
        let product: Product = bson::from_document(result)?;

        print_product(&product);

        products.push(product);
    }

    Ok(products)
}

fn print_product(product: &Product) {
    let serialized_product = serde_json::to_string(&product).unwrap();
    println!("{}", serialized_product);
}