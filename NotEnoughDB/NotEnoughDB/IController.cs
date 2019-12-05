using NotEnoughDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotEnoughDB
{
    public interface IController
    {
        event Action ServersUpdated;
        event Action UsersUpdated;
        event Action OrdersUpdated;

        IEnumerable<Server> GetServers(Server server);
        IEnumerable<User> GetUsers(User user);
        IEnumerable<Order> GetOrders(Order order);

        Server GetServerByID(int id);
        User GetUserByID(int id);
        Order GetOrderByID(int id);

        void AddServer(Server server);
        void AddUser(User user);
        void AddOrder(Order order);

        void DeleteServer(int id);
        void DeleteUser(int id);
        void DeleteOrder(int id);

        void UpdateServer(Server server);
        void UpdateUser(User user);
        void UpdateOrder(Order order);
    }
}
