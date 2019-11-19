using System;
using System.Diagnostics;
using System.IO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace SmartDeliveries.SalesOrder
{
    class Program
    {
        const string ConnectionString = "mongodb+srv://demo_user:2O3ZqzhMeg7BByoW@digitalarkitekt1-jlacy.mongodb.net/test?retryWrites=true&w=majority";
        const string DatabaseName = "SmartDeliveries";
        const string CollectionName = "SalesOrders";

        private static DateTime ThreeMonthsAgo => DateTime.Now.AddMonths(3);

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var settings = MongoClientSettings.FromConnectionString(ConnectionString);

            IMongoClient client = new MongoClient(settings);

            client.DropDatabase(DatabaseName);

            // Set up MongoDB conventions
            var pack = new ConventionPack
            {
                new EnumRepresentationConvention(BsonType.String)
            };

            ConventionRegistry.Register("EnumStringConvention", pack, t => true);

            string json = File.ReadAllText("../SmartDeliveries/src/SmartDeliveries/Validation.js");

            var document = new BsonDocument();
            document.AddRange(BsonDocument.Parse(json));

            Debug.WriteLine(document.ToJson());
            
            var database = client.GetDatabase(DatabaseName);
            database.CreateCollection(CollectionName, new CreateCollectionOptions<BsonDocument> {
                ValidationAction = DocumentValidationAction.Error,
                ValidationLevel = DocumentValidationLevel.Strict,
                Validator = document
            });

            ISalesOrderService salesOrderService = new SalesOrderService(new MongoClient(settings));

            // PB1 Add a new sales order to the system.
            string orderId = Guid.NewGuid().ToString();

            salesOrderService.AddSalesOrder(new AddSalesOrderRequest {
                 OrderId = orderId,
                 CreatedAt = DateTime.Now
            });

            string fulfillmentId = Guid.NewGuid().ToString();

            // PB2 
            salesOrderService.AddFulFillment(new SalesOrderFulfillmentRequest {
                    OrderId = orderId,
                    FulfillmentId = fulfillmentId,
                    Date = DateTime.Now,
                    LineItems = new System.Collections.Generic.List<SalesOrderFulfillmentRequest.FulfillmentLineItem> {
                        new SalesOrderFulfillmentRequest.FulfillmentLineItem {
                            ProductId = 1,
                            Quantity = 5
                        }
                    }
            });

            salesOrderService.AddFulFillmentSafe(new SalesOrderFulfillmentRequest {
                    OrderId = orderId,
                    FulfillmentId = fulfillmentId,
                    Date = DateTime.Now,
                    LineItems = new System.Collections.Generic.List<SalesOrderFulfillmentRequest.FulfillmentLineItem> {
                        new SalesOrderFulfillmentRequest.FulfillmentLineItem {
                            ProductId = 1,
                            Quantity = 5
                        }
                    }
            });

            salesOrderService.CloseExpiredOrders(new CloseExpiredOrdersRequest {
                ExpiryDate = ThreeMonthsAgo
            });
        }
    }
}
