using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Messages;
using Objects;

namespace Customer
{
    public class CustomerGateway
    {
        private int timeout = 10000;
        private IBus _bus = RabbitHutch.CreateBus("host=localhost");
        private CustomerReply _replyMessages;


        public CustomerReply SendRequest(int cusId, int prodId, string country)
        {

            _bus.Receive<OrderReplyMessage>("ReplyQueue" + cusId, message => HandleOrderEvent(message));

            CustomerOrderRequestMessage request = new CustomerOrderRequestMessage
            {
                CustomerId = cusId,
                ProductId = prodId,
                Country = country
            };

            _bus.Send("retailerQueue", request);

            bool gotReply;
            lock (this)
            {
                // Block this thread so that the Retailer program will not exit.
                gotReply = Monitor.Wait(this, timeout);     
            }

            if (gotReply)
                return _replyMessages;
            else
                return new CustomerReply() { Message = "no reply received" };
        }


        private void HandleOrderEvent(OrderReplyMessage message)
        {
            _replyMessages = new CustomerReply() { Message = "reply received", ReplyMessage = message };
            lock (this)
            {
                Monitor.Pulse(this);
            }

        }

    }
}
