using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageTransformer
{
    public class ContentFilter
    {
        public NewReply GetRelevantData(OrderReplyMessage message)
        {
            return new NewReply() { DaysForDelivery = message.DaysForDelivery, ItemsInStock = message.ItemsInStock, ShippingCharge = message.ShippingCharge };
        }
    }

    public class NewReply
    {
        public int ItemsInStock { get; set; }
        public int DaysForDelivery { get; set; }
        public decimal ShippingCharge { get; set; }
    }
}
