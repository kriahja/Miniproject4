﻿using System;
using EasyNetQ;
using Messages;
using System.Threading;

namespace Retailer
{
    public class Retailer
    {
        private string replyQueueName = "replyQueueForRetailerOrderRequestMessage";
        private IBus bus = null;
        private int orderId = 0;

        public void Start()
        {
            using (bus = RabbitHutch.CreateBus("host=localhost"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Retailer started. Waiting for customer requests\n");
                Console.ResetColor();

                // Listen for order request messages from customers
                bus.Receive<CustomerOrderRequestMessage>("retailerQueue", message => HandleOrderRequest(message));

                // Listen for order reply messages from warehouses
                bus.Receive<OrderReplyMessage>(replyQueueName, message => HandleOrderReplyMessage(message));

                lock (this)
                {
                    // Block this thread so that the Retailer program will not exit.
                    Monitor.Wait(this);
                }
            }
        }

        private void HandleOrderRequest(CustomerOrderRequestMessage request)
        {
            int customerId = request.CustomerId;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Request received from customer " + customerId);
            Console.WriteLine("Trying to send the request to a local warehouse.");
            Console.ResetColor();

            OrderRequestMessageToLocalWarehouse requestMessage = new OrderRequestMessageToLocalWarehouse
            {
                ProductId = request.ProductId,
                CustomerId = request.CustomerId,
                Country = request.Country,
                OrderId = ++orderId,
                ReplyTo = replyQueueName

            };

            // Uses Topic Based Routing to send the request to a local warehouse. The topic
            // is requestMessage.Country.
            bus.Publish<OrderRequestMessageToLocalWarehouse>(requestMessage, requestMessage.Country);
        }

        private void HandleOrderReplyMessage(OrderReplyMessage message)
        {
            Console.WriteLine("Reply received");

            if (message.ItemsInStock > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Reply received from warehouse" + message.WarehouseId);
                Console.WriteLine("Order Id: " + message.OrderId);
                Console.WriteLine("Items in stock: " + message.ItemsInStock);
                Console.WriteLine("Days for delivery: " + message.DaysForDelivery);
                Console.WriteLine("Shipping charge: " + message.ShippingCharge);
                Console.ResetColor();

                // Uses Topic Based Routing to send the reply to a customer.
                // The topic ís the customerId from the reply message.
                bus.Publish<OrderReplyMessage>(message, message.CustomerId.ToString());
            }
            else if (message.DaysForDelivery == 2)
            {
                // Publish the message again to all warehouses, if the reply
                // was from a local warehouse (DaysForDelivery = 2) with no
                // items in stock.
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Publishing to all warehouses");
                Console.ResetColor();

                OrderBroadcastRequestMessage broadcastRequestMessage = new OrderBroadcastRequestMessage
                {
                    OrderId = message.OrderId,
                    ProductId = message.ProductId,
                    CustomerId = message.CustomerId,
                    ReplyTo = replyQueueName,
                    Country = ""
                };

                bus.Publish<OrderBroadcastRequestMessage>(broadcastRequestMessage);
            }
        }

    }
}
