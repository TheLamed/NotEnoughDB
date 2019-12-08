using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using NotEnoughDB.Exceptions;
using NotEnoughDB.Models;

namespace NotEnoughDB.Controllers
{
    public class MongoDBControl : IController
    {
        private event Action SUpdated, UUpdated, OUpdated;

        public event Action ServersUpdated
        {
            add
            {
                value?.Invoke();
                SUpdated += value;
            }
            remove => SUpdated -= value;
        }
        public event Action UsersUpdated
        {
            add
            {
                value?.Invoke();
                UUpdated += value;
            }
            remove => UUpdated -= value;
        }
        public event Action OrdersUpdated
        {
            add
            {
                value?.Invoke();
                OUpdated += value;
            }
            remove => OUpdated -= value;
        }

        private MongoClient client;
        private IMongoDatabase db;

        public MongoDBControl()
        {
            client = new MongoClient("mongodb://localhost:27017");
            db = client.GetDatabase("NotEnoughDB");
        }

        private bool IsNullOrEmpty(string str) => str == null || str == string.Empty;

        public void AddOrder(Order order)
        {
            if (order.UID == null)
                throw new RequiredFieldException("UID");
            if (order.SID == null)
                throw new RequiredFieldException("SID");

            var collection = db.GetCollection<Order>("Orders");
            var list = collection.AsQueryable().ToList();
            if (list.Count != 0) order.ID = list.Max(u => u.ID) + 1;
            else order.ID = 0;
            collection.InsertOne(order);
            OUpdated?.Invoke();
        }

        public void AddServer(Server server)
        {
            if (IsNullOrEmpty(server.Processor))
                throw new RequiredFieldException("Processor");
            if (server.RAM == null)
                throw new RequiredFieldException("RAM");
            if (server.SSD == null)
                throw new RequiredFieldException("SSD");

            var collection = db.GetCollection<Server>("Servers");
            var list = collection.AsQueryable().ToList();
            if (list.Count != 0) server.ID = list.Max(u => u.ID) + 1;
            else server.ID = 0;
            collection.InsertOne(server);
            SUpdated?.Invoke();
        }

        public void AddUser(User user)
        {
            if (IsNullOrEmpty(user.Surname))
                throw new RequiredFieldException("Surname");
            if (IsNullOrEmpty(user.Email))
                throw new RequiredFieldException("Email");

            var collection = db.GetCollection<User>("Users");
            var list = collection.AsQueryable().ToList();
            if (list.Count != 0) user.ID = list.Max(u => u.ID) + 1;
            else user.ID = 0;
            collection.InsertOne(user);
            UUpdated?.Invoke();
        }

        public void DeleteOrder(int id)
        {
            var collection = db.GetCollection<Order>("Orders");
            var filter = new BsonDocument("_id", new BsonDocument("$eq", id));
            collection.DeleteOne(filter);
            OUpdated?.Invoke();
        }

        public void DeleteServer(int id)
        {
            var collection = db.GetCollection<Server>("Servers");
            var filter = new BsonDocument("_id", new BsonDocument("$eq", id));
            collection.DeleteOne(filter);
            SUpdated?.Invoke();
        }

        public void DeleteUser(int id)
        {
            var collection = db.GetCollection<User>("Users");
            var filter = new BsonDocument("_id", new BsonDocument("$eq", id));
            collection.DeleteOne(filter);
            UUpdated?.Invoke();
        }

        public IEnumerable<Order> GetOrders(Order order)
        {
            var collection = db.GetCollection<Order>("Orders");
            if (order != null)
            {
                var filters = new Dictionary<string, BsonDocument>();
                if (order.UID != null)
                    filters.Add("UID", new BsonDocument("$eq", order.UID));
                if (order.SID != null)
                    filters.Add("SID", new BsonDocument("$eq", order.SID));

                var filter = new BsonDocument(filters);
                return collection.Find(filter).ToList();
            }
            return collection.AsQueryable().ToList();
        }

        public IEnumerable<Server> GetServers(Server server)
        {
            var collection = db.GetCollection<Server>("Servers");
            if (server != null)
            {
                var filters = new Dictionary<string, BsonDocument>();
                if (!IsNullOrEmpty(server.Processor))
                    filters.Add("Processor", new BsonDocument("$regex", $".*{server.Processor}.*"));
                if (!IsNullOrEmpty(server.Country))
                    filters.Add("Country", new BsonDocument("$regex", $".*{server.Country}.*"));
                if (server.RAM != null)
                    filters.Add("RAM", new BsonDocument("$eq", server.RAM));
                if (server.SSD != null)
                    filters.Add("SSD", new BsonDocument("$eq", server.SSD));

                var filter = new BsonDocument(filters);
                return collection.Find(filter).ToList();
            }
            return collection.AsQueryable().ToList();
        }

        public IEnumerable<User> GetUsers(User user)
        {
            var collection = db.GetCollection<User>("Users");
            if(user != null)
            {
                var filters = new Dictionary<string, BsonDocument>();
                if (!IsNullOrEmpty(user.Name))
                    filters.Add("Name", new BsonDocument("$regex", $".*{user.Name}.*"));
                if (!IsNullOrEmpty(user.Surname))
                    filters.Add("Surname", new BsonDocument("$regex", $".*{user.Surname}.*"));
                if (!IsNullOrEmpty(user.Company))
                    filters.Add("Company", new BsonDocument("$regex", $".*{user.Company}.*"));
                if (!IsNullOrEmpty(user.Email))
                    filters.Add("Email", new BsonDocument("$regex", $".*{user.Email}.*"));

                var filter = new BsonDocument(filters);
                return collection.Find(filter).ToList();
            }
            return collection.AsQueryable().ToList();
        }

        public void UpdateOrder(Order order)
        {
            if (order.UID == null)
                throw new RequiredFieldException("UID");
            if (order.SID == null)
                throw new RequiredFieldException("SID");

            var collection = db.GetCollection<Order>("Orders");
            var filter = new BsonDocument("_id", new BsonDocument("$eq", order.ID));
            collection.ReplaceOne(filter, order);

            OUpdated?.Invoke();
        }

        public void UpdateServer(Server server)
        {
            if (IsNullOrEmpty(server.Processor))
                throw new RequiredFieldException("Processor");
            if (server.RAM == null)
                throw new RequiredFieldException("RAM");
            if (server.SSD == null)
                throw new RequiredFieldException("SSD");

            var collection = db.GetCollection<Server>("Servers");
            var filter = new BsonDocument("_id", new BsonDocument("$eq", server.ID));
            collection.ReplaceOne(filter, server);

            SUpdated?.Invoke();
        }

        public void UpdateUser(User user)
        {
            if (IsNullOrEmpty(user.Surname))
                throw new RequiredFieldException("Surname");
            if (IsNullOrEmpty(user.Email))
                throw new RequiredFieldException("Email");

            var collection = db.GetCollection<User>("Users");
            var filter = new BsonDocument("_id", new BsonDocument("$eq", user.ID));
            collection.ReplaceOne(filter, user);

            UUpdated?.Invoke();
        }
    }
}
