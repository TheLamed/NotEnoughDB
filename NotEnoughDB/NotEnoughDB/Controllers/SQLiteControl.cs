using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotEnoughDB.Models;

namespace NotEnoughDB.Controllers
{
    public class SQLiteControl : IController
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

        private SQLiteConnection connection { get; set; }

        public SQLiteControl()
        {
            connection = new SQLiteConnection("Data Source=../../../../databases/SQLite/sqlite_database.db"); ;
        }

        private void CloseConnection()
        {
            connection.Close();
        }

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
            List<Server> servers = new List<Server>();

            try
            {
                connection.Open();
                SQLiteCommand cmd = new SQLiteCommand(connection);
                cmd.CommandText = "select * from Server";
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Server s = new Server();
                    s.ID = Convert.ToInt32(reader["ID"]);
                    servers.Add(s);
                }
                reader.Close();
            }
            catch (Exception)
            {

            }
            finally
            {
                CloseConnection();
            }
            return servers;
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
