using System;
using MongoDB.Driver;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SmartDeliveries.SalesOrder
{
    public class SalesOrderService : ISalesOrderService
    {
        const string DatabaseName = "SmartDeliveries";
        const string CollectionName = "SalesOrders";

        readonly IMongoClient client;

        public SalesOrderService(IMongoClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public void AddFulFillment(SalesOrderFulfillmentRequest request)
        {
            var database = client.GetDatabase(DatabaseName);
            var collection = database.GetCollection<SalesOrderDocument>(CollectionName);

            var query = collection.AsQueryable();

            var order = query.Single(s => s.OrderId == request.OrderId);

            order.Fulfillments.Add(new SalesOrderDocument.Fulfillment
            {
                FulfillmentId = request.FulfillmentId,
                FulfillmentDate = request.Date,
                LineItems = request.LineItems.Select(s => new SalesOrderDocument.Fulfillment.FulfillmentLineItem
                {
                    ProductId = s.ProductId,
                    Quantity = s.Quantity
                }).ToList()
            });

            var fb = Builders<SalesOrderDocument>.Filter;
            var filter = fb.Eq(s => s.OrderId, request.OrderId);

            var result = collection.ReplaceOne(filter, order);

            Debug.WriteLine(JsonConvert.SerializeObject(result));
        }

        public void AddFulFillmentSafe(SalesOrderFulfillmentRequest request)
        {
            var database = client.GetDatabase(DatabaseName);
            var collection = database.GetCollection<SalesOrderDocument>(CollectionName);

            var fb = Builders<SalesOrderDocument>.Filter;
            var lineItemFb = Builders<SalesOrderDocument.Fulfillment>.Filter;

            var filter = fb.Eq(s => s.OrderId, request.OrderId) &
                    fb.ElemMatch(s => s.Fulfillments, lineItemFb.Ne(s => s.FulfillmentId, request.FulfillmentId));

            var updateDefinition = Builders<SalesOrderDocument>.Update;

            var updateItems = new List<UpdateDefinition<SalesOrderDocument>> {
                updateDefinition.Push(s => s.Fulfillments, new SalesOrderDocument.Fulfillment {
                FulfillmentId = request.FulfillmentId,
                FulfillmentDate = request.Date,
                LineItems = request.LineItems.Select(s =>  new SalesOrderDocument.Fulfillment.FulfillmentLineItem {
                    ProductId = s.ProductId,
                    Quantity = s.Quantity
                }).ToList()
            })};

            var result = collection.UpdateOne(filter, updateDefinition.Combine(updateItems));

            Debug.WriteLine(JsonConvert.SerializeObject(result));
        }

        public void AddSalesOrder(AddSalesOrderRequest request)
        {
            var database = client.GetDatabase(DatabaseName);
            var collection = database.GetCollection<SalesOrderDocument>(CollectionName);

            var document = new SalesOrderDocument
            {
               // Status = SalesOrderDocument.OrderStatus.Open,
                OrderId = request.OrderId,
                CreatedAt = request.CreatedAt
            };

            collection.InsertOne(document);
        }

        public void CloseExpiredOrders(CloseExpiredOrdersRequest request)
        {
            var database = client.GetDatabase(DatabaseName);
            var collection = database.GetCollection<SalesOrderDocument>(CollectionName);

            var fb = Builders<SalesOrderDocument>.Filter;
            var filter = fb.Lt(s => s.CreatedAt, request.ExpiryDate);

            var update = Builders<SalesOrderDocument>.Update;

            var updateItems = new List<UpdateDefinition<SalesOrderDocument>> {
                update.Set(s => s.Status, SalesOrderDocument.OrderStatus.Closed)
            };

            collection.UpdateMany(filter, update.Combine(updateItems));
        }

        public List<string> OrdersWithOpenDeliveries()
        {
            var database = client.GetDatabase(DatabaseName);
            var collection = database.GetCollection<SalesOrderDocument>(CollectionName);

            var fb = Builders<SalesOrderDocument>.Filter;
            var filter = fb.ElemMatch();

            collection.Find(filter);
        }
    }
}
