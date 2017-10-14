using Messages;
using MessageTransformer;
using System;
using System.Threading.Tasks;

namespace Customer
{
   public class CustomerProgram
    {
        static void Main(string[] args)
        {
            // I have no warehouse in Sweeden (SE), so this request will eventually,
            // after a timeout, be forwarded to all warehouses.
            //new CustomerGateway().SendRequest(2,1, "SE");

            // Because I have a warehouse in Denmark (DK) with product number 1 in
            // stock, this order request will be processed immediately by the local
            // warehouse. This also means, that the customer program will receive
            // a reply for this request before it receives a reply for the request
            // from Sweeden above.
            Console.WriteLine("sending request from cus 1");
            var response1 = new CustomerGateway().SendRequest(1, 1, "DK");
            Console.WriteLine(response1.Message);
            if (response1.ReplyMessage != null)
                PrintData(response1.ReplyMessage);



            // A customer placing an order for a product which is not in stock.
            Console.WriteLine("sending request from cus 3");
            var response2 = new CustomerGateway().SendRequest(3, 100, "DK");
            Console.WriteLine(response2.Message);
            if (response2.ReplyMessage != null)
                PrintData(response2.ReplyMessage);

            Console.ReadLine();
        }
        public static void PrintData(OrderReplyMessage mes)
        {
            var temp = new ContentFilter().GetRelevantData(mes);
            Console.WriteLine("Items in stock:" + temp.ItemsInStock);
            Console.WriteLine($"Days to deliver:" + temp.DaysForDelivery);
            Console.WriteLine($"Shipping cost:" + temp.ShippingCharge);
            Console.WriteLine();
        }


    }
}
