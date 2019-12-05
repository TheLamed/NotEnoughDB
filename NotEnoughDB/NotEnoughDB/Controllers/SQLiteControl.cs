using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotEnoughDB.Models;

namespace NotEnoughDB.Controllers
{
    public class SQLiteControl : IController
    {
        public event Action ServersUpdated;
        public event Action UsersUpdated;
        public event Action OrdersUpdated;

        public void AddOrder(Order order)
        {
            throw new NotImplementedException();
        }

        public void AddServer(Server server)
        {
            throw new NotImplementedException();
        }

        public void AddUser(User user)
        {
            throw new NotImplementedException();
        }

        public void DeleteOrder(int id)
        {
            throw new NotImplementedException();
        }

        public void DeleteServer(int id)
        {
            throw new NotImplementedException();
        }

        public void DeleteUser(int id)
        {
            throw new NotImplementedException();
        }

        public Order GetOrderByID(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Order> GetOrders(Order order)
        {
            throw new NotImplementedException();
        }

        public Server GetServerByID(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Server> GetServers(Server server)
        {
            throw new NotImplementedException();
        }

        public User GetUserByID(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetUsers(User user)
        {
            throw new NotImplementedException();
        }

        public void UpdateOrder(Order order)
        {
            throw new NotImplementedException();
        }

        public void UpdateServer(Server server)
        {
            throw new NotImplementedException();
        }

        public void UpdateUser(User user)
        {
            throw new NotImplementedException();
        }
    }
}
