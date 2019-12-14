using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using NotEnoughDB.Exceptions;
using NotEnoughDB.Models;

namespace NotEnoughDB.Controllers
{
    public class FireBaseControl : IController
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

        private IFirebaseConfig config;
        private IFirebaseClient client;

        public FireBaseControl()
        {
            config = new FirebaseConfig
            {
                AuthSecret = "09doRtKkY5MU7OsLpCuCO9NPhCbonO1Jxm2iwYY8",
                BasePath = "https://notenoughdb.firebaseio.com/"
            };
            client = new FirebaseClient(config);
        }

        private bool IsNullOrEmpty(string str) => str == null || str == string.Empty;

        public void AddOrder(Order order)
        {
            if (order.UID == null)
                throw new RequiredFieldException("UID");
            if (order.SID == null)
                throw new RequiredFieldException("SID");
            try
            {
                FirebaseResponse getresponse = client.Get("Orders");
                var orders = getresponse.ResultAs<List<Order>>();
                orders.RemoveAll(v => v == null);
                order.ID = orders.Last().ID + 1;
                FirebaseResponse response = client.Set("Orders/" + order.ID, order);
            }
            catch (Exception)
            {
                throw;
            }
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
            try
            {
                FirebaseResponse getresponse = client.Get("Servers");
                var servers = getresponse.ResultAs<List<Server>>();
                servers.RemoveAll(v => v == null);
                server.ID = servers.Last().ID + 1;
                FirebaseResponse response = client.Set("Servers/" + server.ID, server);
            }
            catch (Exception)
            {
                throw;
            }
            SUpdated?.Invoke();
        }

        public void AddUser(User user)
        {
            if (IsNullOrEmpty(user.Surname))
                throw new RequiredFieldException("Surname");
            if (IsNullOrEmpty(user.Email))
                throw new RequiredFieldException("Email");
            try
            {
                FirebaseResponse getresponse = client.Get("Users");
                var users = getresponse.ResultAs<List<User>>();
                users.RemoveAll(v => v == null);
                user.ID = users.Last().ID + 1;
                FirebaseResponse response = client.Set("Users/" + user.ID, user);
            }
            catch (Exception)
            {
                throw;
            }
            UUpdated?.Invoke();
        }

        public void DeleteOrder(Order order)
        {
            try
            {
                FirebaseResponse response = client.Delete("Orders/" + order.ID);
            }
            catch (Exception)
            {
                throw;
            }
            OUpdated?.Invoke();
        }

        public void DeleteServer(Server server)
        {
            try
            {
                FirebaseResponse response = client.Delete("Servers/" + server.ID);
            }
            catch (Exception)
            {
                throw;
            }
            SUpdated?.Invoke();
        }

        public void DeleteUser(User user)
        {
            try
            {
                FirebaseResponse response = client.Delete("Users/" + user.ID);
            }
            catch (Exception)
            {
                throw;
            }
            UUpdated?.Invoke();
        }

        public IEnumerable<Order> GetOrders(Order order)
        {
            List<Order> orders = new List<Order>();
            try
            {
                FirebaseResponse response = client.Get("Orders");
                orders = response.ResultAs<List<Order>>();
                orders.RemoveAll(v => v == null);
                if (order != null) orders.RemoveAll(v =>
                {
                    if (order.UID != null && (v.UID == null || v.UID != order.UID)) return true;
                    if (order.SID != null && (v.SID == null || v.SID != order.SID)) return true;
                    if (order.DateFrom != null && (v.DateFrom == null || v.DateFrom != order.DateFrom)) return true;
                    if (order.DateTo != null && (v.DateTo == null || v.DateTo != order.DateTo)) return true;

                    return false;
                });
            }
            catch (Exception)
            {
                throw;
            }
            return orders;
        }

        public IEnumerable<Server> GetServers(Server server)
        {
            List<Server> servers = new List<Server>();
            try
            {
                FirebaseResponse response = client.Get("Servers");
                servers = response.ResultAs<List<Server>>();
                servers.RemoveAll(v => v == null);
                if (server != null) servers.RemoveAll(v =>
                {
                    if (!IsNullOrEmpty(server.Processor) && (IsNullOrEmpty(v.Processor) || !v.Processor.Contains(server.Processor))) return true;
                    if (!IsNullOrEmpty(server.Country) && (IsNullOrEmpty(v.Country) || !v.Country.Contains(server.Country))) return true;
                    if (server.RAM != null && (v.RAM == null || v.RAM != server.RAM)) return true;
                    if (server.SSD != null && (v.SSD == null || v.SSD != server.SSD)) return true;

                    return false;
                });
            }
            catch (Exception)
            {
                throw;
            }
            return servers;
        }

        public IEnumerable<User> GetUsers(User user)
        {
            List<User> users = new List<User>();
            try
            {
                FirebaseResponse response = client.Get("Users");
                users = response.ResultAs<List<User>>();
                users.RemoveAll(v => v == null);
                if (user != null) users.RemoveAll(v =>
                {
                    if (!IsNullOrEmpty(user.Name) && (IsNullOrEmpty(v.Name) || !v.Name.Contains(user.Name))) return true;
                    if (!IsNullOrEmpty(user.Surname) && (IsNullOrEmpty(v.Surname) || !v.Surname.Contains(user.Surname))) return true;
                    if (!IsNullOrEmpty(user.Company) && (IsNullOrEmpty(v.Company) || !v.Company.Contains(user.Company))) return true;
                    if (!IsNullOrEmpty(user.Email) && (IsNullOrEmpty(v.Email) || !v.Email.Contains(user.Email))) return true;
                    return false;
                });
            }
            catch (Exception)
            {
                throw;
            }
            return users;
        }

        public void UpdateOrder(Order order)
        {
            if (order.UID == null)
                throw new RequiredFieldException("UID");
            if (order.SID == null)
                throw new RequiredFieldException("SID");
            try
            {
                FirebaseResponse response = client.Update("Orders/" + order.ID, order);
            }
            catch (Exception)
            {
                throw;
            }
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
            try
            {
                FirebaseResponse response = client.Update("Servers/" + server.ID, server);
            }
            catch (Exception)
            {
                throw;
            }
            SUpdated?.Invoke();
        }

        public void UpdateUser(User user)
        {
            if (IsNullOrEmpty(user.Surname))
                throw new RequiredFieldException("Surname");
            if (IsNullOrEmpty(user.Email))
                throw new RequiredFieldException("Email");
            try
            {
                FirebaseResponse response = client.Update("Users/" + user.ID, user);
            }
            catch (Exception)
            {
                throw;
            }
            UUpdated?.Invoke();
        }
    }
}
