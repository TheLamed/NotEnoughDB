using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotEnoughDB.Exceptions;
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
        private SQLiteDataReader reader { get; set; }

        public SQLiteControl()
        {
            connection = new SQLiteConnection("Data Source=../../../../databases/SQLite/sqlite_database.db"); ;
        }

        private void Close()
        {
            if (!reader.IsClosed)
                reader.Close();
            connection.Close();
        }

        private bool IsNullOrEmpty(string str) => str == null || str == string.Empty;

        public void AddOrder(Order order)
        {
            SQLiteCommand cmd = new SQLiteCommand(connection);
            try
            {
                cmd.CommandText = "INSERT INTO \"Order\" ";
                string columns = "(", values = " VALUES(";

                if (order.UID == null)
                    throw new RequiredFieldException("UID");
                if (order.SID == null)
                    throw new RequiredFieldException("SID");

                columns += "UID, SID";
                values += $"{order.UID}, {order.SID}";

                if (order.DateFrom != null)
                {
                    columns += ", datefrom";
                    values += $", '{order.DateFrom?.ToString("yyyy-MM-dd")}'";
                }
                if (order.DateTo != null)
                {
                    columns += ", dateto";
                    values += $", '{order.DateTo?.ToString("yyyy-MM-dd")}'";
                }

                columns += ") ";
                values += ") ";

                cmd.CommandText += columns + values;

                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Close();
                OUpdated?.Invoke();
            }
        }

        public void AddServer(Server server)
        {
            SQLiteCommand cmd = new SQLiteCommand(connection);
            try
            {
                cmd.CommandText = "INSERT INTO \"Server\" ";
                string columns = "(", values = " VALUES(";

                if (IsNullOrEmpty(server.Processor))
                    throw new RequiredFieldException("Processor");
                if (server.RAM == null)
                    throw new RequiredFieldException("RAM");
                if (server.SSD == null)
                    throw new RequiredFieldException("SSD");

                columns += "processor, RAM, SSD";
                values += $"'{server.Processor}', {server.RAM}, {server.SSD}";

                if (!IsNullOrEmpty(server.Country))
                {
                    columns += ", country";
                    values += $", '{server.Country}'";
                }

                columns += ") ";
                values += ") ";

                cmd.CommandText += columns + values;

                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Close();
                SUpdated?.Invoke();
            }
        }

        public void AddUser(User user)
        {
            SQLiteCommand cmd = new SQLiteCommand(connection);
            try
            {
                cmd.CommandText = "INSERT INTO \"User\" ";
                string columns = "(", values = " VALUES(";

                if (IsNullOrEmpty(user.Surname))
                    throw new RequiredFieldException("Surname");
                if (IsNullOrEmpty(user.Email))
                    throw new RequiredFieldException("Email");

                columns += "surname, email";
                values += $"'{user.Surname}', '{user.Email}'";

                if (!IsNullOrEmpty(user.Name))
                {
                    columns += ", name";
                    values += $", '{user.Name}'";
                }
                if (!IsNullOrEmpty(user.Company))
                {
                    columns += ", company";
                    values += $", '{user.Company}'";
                }

                columns += ") ";
                values += ") ";

                cmd.CommandText += columns + values;

                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Close();
                UUpdated?.Invoke();
            }
        }

        public void DeleteOrder(int id)
        {
            SQLiteCommand cmd = new SQLiteCommand(connection);
            try
            {
                cmd.CommandText = $"DELETE FROM \"Order\" WHERE ID = '{id}'";
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Close();
                OUpdated?.Invoke();
            }
        }

        public void DeleteServer(int id)
        {
            SQLiteCommand cmd = new SQLiteCommand(connection);
            try
            {
                cmd.CommandText = $"DELETE FROM \"Server\" WHERE ID = '{id}'";
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Close();
                SUpdated?.Invoke();
            }
        }

        public void DeleteUser(int id)
        {
            SQLiteCommand cmd = new SQLiteCommand(connection);
            try
            {
                cmd.CommandText = $"DELETE FROM \"User\" WHERE ID = '{id}'";
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Close();
                UUpdated?.Invoke();
            }
        }

        public IEnumerable<Order> GetOrders(Order order)
        {
            List<Order> orders = new List<Order>();
            SQLiteCommand cmd = new SQLiteCommand(connection);
            try
            {
                cmd.CommandText = "SELECT * FROM \"Order\" ";

                if (order != null)
                {
                    string where = "";
                    if (order.UID != null)
                    {
                        if (where != "") where += " AND ";
                        where += $"UID = {order.UID}";
                    }
                    if (order.SID != null)
                    {
                        if (where != "") where += " AND ";
                        where += $"SID = {order.SID}";
                    }
                    if (where != "")
                        cmd.CommandText += " WHERE " + where;
                }
                connection.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Order o = new Order();
                    o.ID = Convert.ToInt32(reader["ID"]);
                    o.UID = Convert.ToInt32(reader["UID"]);
                    o.SID = Convert.ToInt32(reader["SID"]);
                    if (reader["datefrom"] != DBNull.Value)
                    {
                        var tmpdate = reader["datefrom"].ToString().Split("-/".ToCharArray())
                            .Select(v => Convert.ToInt32(v)).ToArray();
                        o.DateFrom = new DateTime(tmpdate[0], tmpdate[1], tmpdate[2]);
                    }
                    else o.DateFrom = null;
                    if (reader["dateto"] != DBNull.Value)
                    {
                        var tmpdate = reader["dateto"].ToString().Split("-/".ToCharArray()).Select(v => Convert.ToInt32(v)).ToArray();
                        o.DateTo = new DateTime(tmpdate[0], tmpdate[1], tmpdate[2]);
                    }
                    else o.DateTo = null;
                    orders.Add(o);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Close();
            }
            return orders;
        }

        public IEnumerable<Server> GetServers(Server server)
        {
            List<Server> servers = new List<Server>();
            SQLiteCommand cmd = new SQLiteCommand(connection);
            try
            {
                cmd.CommandText = "SELECT * FROM \"Server\" ";
                
                if(server != null)
                {
                    string where = "";
                    if (!IsNullOrEmpty(server.Processor))
                    {
                        if (where != "") where += " AND ";
                        where += $"processor LIKE '%{server.Processor}%'";
                    }
                    if (!IsNullOrEmpty(server.Country))
                    {
                        if (where != "") where += " AND ";
                        where += $"country LIKE '%{server.Country}%'";
                    }
                    if (server.RAM != null)
                    {
                        if (where != "") where += " AND ";
                        where += $"RAM = {server.RAM}";
                    }
                    if (server.SSD != null)
                    {
                        if (where != "") where += " AND ";
                        where += $"SSD = {server.SSD}";
                    }
                    if(where != "")
                        cmd.CommandText += " WHERE " + where;
                }
                connection.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Server s = new Server();
                    s.ID = Convert.ToInt32(reader["ID"]);
                    s.SSD = Convert.ToInt32(reader["SSD"]);
                    s.RAM = Convert.ToInt32(reader["RAM"]);
                    s.Processor = reader["processor"] != DBNull.Value ? reader["processor"].ToString() : null;
                    s.Country = reader["country"] != DBNull.Value ? reader["country"].ToString() : null;
                    servers.Add(s);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Close();
            }
            return servers;
        }

        public IEnumerable<User> GetUsers(User user)
        {
            List<User> users = new List<User>();
            SQLiteCommand cmd = new SQLiteCommand(connection);
            try
            {
                cmd.CommandText = "SELECT * FROM \"User\" ";

                if (user != null)
                {
                    string where = "";
                    if (!IsNullOrEmpty(user.Name))
                    {
                        if (where != "") where += " AND ";
                        where += $"name LIKE '%{user.Name}%'";
                    }
                    if (!IsNullOrEmpty(user.Surname))
                    {
                        if (where != "") where += " AND ";
                        where += $"surname LIKE '%{user.Surname}%'";
                    }
                    if (!IsNullOrEmpty(user.Company))
                    {
                        if (where != "") where += " AND ";
                        where += $"company LIKE '%{user.Company}%'";
                    }
                    if (!IsNullOrEmpty(user.Email))
                    {
                        if (where != "") where += " AND ";
                        where += $"email LIKE '%{user.Email}%'";
                    }
                    if (where != "")
                        cmd.CommandText += " WHERE " + where;
                }
                connection.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    User u = new User();
                    u.ID = Convert.ToInt32(reader["ID"]);
                    u.Name = reader["name"] != DBNull.Value ? reader["name"].ToString() : null;
                    u.Surname = reader["surname"] != DBNull.Value ? reader["surname"].ToString() : null;
                    u.Company = reader["company"] != DBNull.Value ? reader["company"].ToString() : null;
                    u.Email = reader["email"] != DBNull.Value ? reader["email"].ToString() : null;
                    users.Add(u);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Close();
            }
            return users;
        }

        public void UpdateOrder(Order order)
        {
            SQLiteCommand cmd = new SQLiteCommand(connection);
            try
            {
                cmd.CommandText = "UPDATE \"Order\" SET ";

                if (order.UID == null)
                    throw new RequiredFieldException("UID");
                if (order.SID == null)
                    throw new RequiredFieldException("SID");

                cmd.CommandText += $"UID = {order.UID}, SID = {order.SID}, datefrom = '{order.DateFrom?.ToString("yyy-MM-dd")}', dateto = '{order.DateTo?.ToString("yyy-MM-dd")}'";
                cmd.CommandText += $" WHERE ID = {order.ID}";

                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Close();
                OUpdated?.Invoke();
            }
        }

        public void UpdateServer(Server server)
        {
            SQLiteCommand cmd = new SQLiteCommand(connection);
            try
            {
                cmd.CommandText = "UPDATE \"Server\" SET ";

                if (IsNullOrEmpty(server.Processor))
                    throw new RequiredFieldException("Processor");
                if (server.RAM == null)
                    throw new RequiredFieldException("RAM");
                if (server.SSD == null)
                    throw new RequiredFieldException("SSD");

                cmd.CommandText += $"processor = '{server.Processor}', country = '{server.Country}', RAM = {server.RAM}, SSD = {server.SSD}";
                cmd.CommandText += $" WHERE ID = {server.ID}";

                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Close();
                SUpdated?.Invoke();
            }
        }

        public void UpdateUser(User user)
        {
            SQLiteCommand cmd = new SQLiteCommand(connection);
            try
            {
                cmd.CommandText = "UPDATE \"User\" SET ";
                
                if (IsNullOrEmpty(user.Surname))
                    throw new RequiredFieldException("Surname");
                if (IsNullOrEmpty(user.Email))
                    throw new RequiredFieldException("Email");

                cmd.CommandText += $"name = '{user.Name}', surname = '{user.Surname}', company = '{user.Company}', email = '{user.Email}'";
                cmd.CommandText += $" WHERE ID = {user.ID}";

                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Close();
                UUpdated?.Invoke();
            }
        }
    }
}
