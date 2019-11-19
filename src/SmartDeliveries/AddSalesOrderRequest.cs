using System;
using System.Collections.Generic;

namespace SmartDeliveries.SalesOrder
{
    public class AddSalesOrderRequest
    {
        public string OrderId { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<LineItem> LineItems { get; set; }

        public class LineItem
        {
            public int ProductId { get; set; }
            public decimal Price { get; set; }
        }
    }

    public class CloseExpiredOrdersRequest
    {
        public DateTime ExpiryDate { get; set; }
    }

    public class SalesOrderFulfillmentRequest
    {
        public string OrderId { get; set; }

        public string FulfillmentId { get; set; }

        public DateTime Date { get; set; }

        public List<FulfillmentLineItem> LineItems { get; set; }

        public class FulfillmentLineItem
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }
}
