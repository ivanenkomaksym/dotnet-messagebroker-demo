"use strict";

// Access the customerId variable defined in the Razor Page
console.log(customerIdStr);

var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub?customerId=" + customerIdStr).build();

connection.on("ReceiveReserveStockResultNotification", function (orderId, reserveResult, failedProductNames) {
    console.log("typeof failedProductNames" + typeof failedProductNames);
    toastr.options.timeOut = 5000;
    switch (reserveResult) {
        case 0: // Reserved
            break;
        case 1: // Failed
            toastr.error("Failed to reserve stock items for order `" + orderId + "`! Insufficient stock levels for product names: " + failedProductNames.join());
            break;
    }
});

connection.on("ReceivePaymentResultNotification", function (orderId, paymentStatus) {
    toastr.options.timeOut = 5000;
    switch (paymentStatus) {
        case 0: // Unpaid
            break;
        case 1: // Failed
            toastr.warning("Payment for order `" + orderId + "` failed! You can update payment information and try again.");
            break;
        case 2: // Expired
            toastr.warning("Payment for order `" + orderId + "` expired! You can update payment information and try again.");
            break;
        case 3: // Paid
            toastr.success("Your payment for order `" + orderId + "` succeded! Order will be shipped soon.");
            break;
        case 4: // Refunding
            break;
        case 5: // Refunded
            break;
    }
});

connection.on("ReceiveShipmentResultNotification", function (orderId, deliveryStatus) {
    toastr.options.timeOut = 5000;
    switch (deliveryStatus) {
        case 0: // None
            break;
        case 1: // Shipping
            toastr.success("Your order `" + orderId + "` was sent for shipment.");
            break;
        case 2: // Shipped
            toastr.success("Your order `" + orderId + "` has been delivered.");
            break;
        case 3: // Collecting
            toastr.success("Collect your order `" + orderId + "` and confirm collection.");
            break;
        case 4: // Collected
            break;
        case 5: // Returning
            break;
        case 6: // Returned
            toastr.success("Your order `" + orderId + "` was successfully returned. Refund process will start shortly.");
            break;
    }
});

connection.start();