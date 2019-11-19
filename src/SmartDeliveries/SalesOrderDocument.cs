using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartDeliveries.SalesOrder
{
    public class SalesOrderDocument
    {
        public SalesOrderDocument()
        {
            Fulfillments = new List<Fulfillment>();
        }

        [BsonId]
        public string OrderId { get; set; }

        public DateTime CreatedAt { get; set; }

        public OrderStatus Status { get; set; }

        public enum OrderStatus
        {
            Open = 0,
            Closed = 1
        }

        public List<Fulfillment> Fulfillments { get; set; }

        public class Fulfillment
        {
            public string FulfillmentId { get; set; }
            public DateTime FulfillmentDate { get; set; }

            public List<FulfillmentLineItem> LineItems { get; set; }
            public class FulfillmentLineItem
            {
                public int ProductId { get; set; }
                public int Quantity { get; set; }
            }
        }
    }
}
