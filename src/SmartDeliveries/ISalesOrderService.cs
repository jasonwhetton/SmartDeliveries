using System.Collections.Generic;

namespace SmartDeliveries.SalesOrder
{
    public interface ISalesOrderService
    {
        void AddSalesOrder(AddSalesOrderRequest request);

        void AddFulFillment(SalesOrderFulfillmentRequest request);

        void AddFulFillmentSafe(SalesOrderFulfillmentRequest request);

        void CloseExpiredOrders(CloseExpiredOrdersRequest request);

        List<string> OrdersWithOpenDeliveries();
    }
}