﻿using Common.Models.Payment;
using Common.Models;

namespace Common.SeedData
{
    public static class CustomerSeed
    {
        public static IEnumerable<Customer> GetPreconfiguredCustomer()
        {
            return new List<Customer>()
            {
                new() {
                    Id = Guid.NewGuid(),
                    FirstName = "Alice",
                    LastName = "Liddell",
                    Email = "alice@gmail.com",
                    Password = "alice",
                    PaymentInfo = new PaymentInfo
                    {
                        CardName = "Alice Liddell",
                        CardNumber = "1234 1234 1234 1234",
                        CVV = "123",
                        Expiration = "01/30",
                        PaymentMethod = PaymentMethod.CreditCard_AlwaysExpire
                    },
                    ShippingAddress = new Address
                    {
                        FirstName = "Alice",
                        LastName = "Liddell",
                        Email = "alice@gmail.com",
                        Country = "England",
                        AddressLine = "London",
                        ZipCode = "12345"
                    },
                    CreationDateTime = DateTime.Now,
                },
                new() {
                    Id = Guid.NewGuid(),
                    FirstName = "Bob",
                    LastName = "Liddell",
                    Email = "bob@gmail.com",
                    Password = "bob",
                    PaymentInfo = new PaymentInfo
                    {
                        CardName = "Bob Liddell",
                        CardNumber = "9876 9876 9876 9876",
                        CVV = "987",
                        Expiration = "12/29",
                        PaymentMethod = PaymentMethod.PayPal_AlwaysFail
                    },
                    ShippingAddress = new Address
                    {
                        FirstName = "Bob",
                        LastName = "Liddell",
                        Email = "alice@gmail.com",
                        Country = "England",
                        AddressLine = "London",
                        ZipCode = "12345"
                    },
                    CreationDateTime = DateTime.Now,
                    UserRole = UserRole.Admin
                }
            };
        }
    }
}
