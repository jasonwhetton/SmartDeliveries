using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace SmartDeliveries.SalesOrder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Smart Deliveries AB");

            var settings = MongoClientSettings
            .FromConnectionString("mongodb+srv://demo_user:2O3ZqzhMeg7BByoW@digitalarkitekt1-jlacy.mongodb.net/test?retryWrites=true&w=majority");
            IMongoClient client = new MongoClient(settings);

            var database = client.GetDatabase("SmartDeliveries-Jason");

            var collection = database.GetCollection<SalesOrderDocument>("Orders");


            var salesOrderDocument = new SalesOrderDocument {
                CustomerId = "ABC",
                CustomerName = "A.Customer",
                OrderId = "666666",
                LineItems = new List<SalesOrderDocument.LineItem> {
                    new SalesOrderDocument.LineItem {
                         Quantity = 1,
                         Price = 25m
                    }
                }
            };

            collection.InsertOne(salesOrderDocument);

            var productDocument = new BsonDocument {
                { "Name", "Value"},

            };

            Debug.WriteLine(productDocument.ToJson());

            // var orderDocument = new BsonDocument
            // {

            // };

            //Debug.WriteLine(orderDocument.ToJson());
        }

        public class SalesOrderDocument
        {
            public string CustomerId { get; set; }

            public string CustomerName { get; set; }

            [BsonId]
            public string OrderId {get;set;}

            public SalesOrderStatus Status { get; set; }

            public enum SalesOrderStatus
            {
                Open = 0,
                Closed = 1
            }

            [BsonRepresentation(MongoDB.Bson.BsonType.DateTime)]
            public DateTime CreatedAt { get; set; }

            public bool SubscribeToNewsletter { get; set; }

            public List<LineItem> LineItems { get; set; }

            public class LineItem
            {
                [BsonRepresentation(MongoDB.Bson.BsonType.Decimal128)]
                public decimal Price { get; set; }

                [BsonRepresentation(MongoDB.Bson.BsonType.Int32)]
                public int ProductId { get; set; }

                public int Quantity { get; set; }
            }
        }
    }
}
