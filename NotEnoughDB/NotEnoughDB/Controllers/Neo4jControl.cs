using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4j.Driver.V1;
using NotEnoughDB.Exceptions;
using NotEnoughDB.Models;

namespace NotEnoughDB.Controllers
{
    public class Neo4jControl : IController
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

        private IDriver driver;

        public Neo4jControl()
        {
            driver = GraphDatabase.Driver(@"bolt://localhost:7687", AuthTokens.Basic("neo4j", "admin"));
        }

        private bool IsNullOrEmpty(string str) => str == null || str == string.Empty;

        public void AddOrder(Order order)
        {
            if (order.UID == null)
                throw new RequiredFieldException("UID");
            if (order.SID == null)
                throw new RequiredFieldException("SID");

            using (var session = driver.Session())
            {
                var result = session.WriteTransaction(transaction =>
                {
                    string tr = "MATCH (u:User), (s:Server) WHERE id(u) = $UID AND id(s) = $SID CREATE (u)-[o:Order{";
                    string values = "";
                    if (order.DateFrom != null)
                    {
                        if (values != "") values += ", ";
                        values += "datefrom: date($DateFrom)";
                    }
                    if (order.DateTo != null)
                    {
                        if (values != "") values += ", ";
                        values += "dateto: date($DateTo)";
                    }

                    tr += values + "}]->(s)";
                    var _result = transaction.Run(tr, order);
                    return _result;
                });
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

            using (var session = driver.Session())
            {
                var result = session.WriteTransaction(transaction =>
                {
                    string tr = "CREATE (s:Server {processor: $Processor, RAM: $RAM, SSD: $SSD";
                    if (!IsNullOrEmpty(server.Country))
                        tr += ", country: $Country";

                    tr += "})";
                    var _result = transaction.Run(tr, server);
                    return _result;
                });
            }
            SUpdated?.Invoke();
        }

        public void AddUser(User user)
        {
            if (IsNullOrEmpty(user.Surname))
                throw new RequiredFieldException("Surname");
            if (IsNullOrEmpty(user.Email))
                throw new RequiredFieldException("Email");

            using (var session = driver.Session())
            {
                var result = session.WriteTransaction(transaction =>
                {
                    string tr = "CREATE (u:User {surname: $Surname, email: $Email";
                    if (!IsNullOrEmpty(user.Name))
                        tr += $", name: $Name";
                    if (!IsNullOrEmpty(user.Company))
                        tr += $", company: $Company";

                    tr += "})";
                    var _result = transaction.Run(tr, user);
                    return _result;
                });
            }
            UUpdated?.Invoke();
        }

        public void DeleteOrder(int id)
        {
            using (var session = driver.Session())
            {
                var result = session.WriteTransaction(transaction =>
                {
                    string tr = "MATCH ()-[o:Order]->() WHERE id(o) = $ID DELETE o";
                    var _result = transaction.Run(tr, new { ID = id });
                    return _result;
                });
            }
            OUpdated?.Invoke();
        }

        public void DeleteServer(int id)
        {
            using (var session = driver.Session())
            {
                var result_relationships = session.WriteTransaction(transaction =>
                {
                    string tr = "MATCH ()-[o:Order]->(s:Server) WHERE id(s) = $ID DELETE o";
                    var _result = transaction.Run(tr, new { ID = id });
                    return _result;
                });
                var result_nodes = session.WriteTransaction(transaction =>
                {
                    string tr = "MATCH (s:Server) WHERE id(s) = $ID DELETE s";
                    var _result = transaction.Run(tr, new { ID = id });
                    return _result;
                });
            }
            SUpdated?.Invoke();
            OUpdated?.Invoke();
        }

        public void DeleteUser(int id)
        {
            using (var session = driver.Session())
            {
                var result_relationships = session.WriteTransaction(transaction =>
                {
                    string tr = "MATCH (u:User)-[o:Order]->() WHERE id(u) = $ID DELETE o";
                    var _result = transaction.Run(tr, new { ID = id });
                    return _result;
                });
                var result_nodes = session.WriteTransaction(transaction =>
                {
                    string tr = "MATCH (u:User) WHERE id(u) = $ID DELETE u";
                    var _result = transaction.Run(tr, new { ID = id });
                    return _result;
                });
            }
            UUpdated?.Invoke();
            OUpdated?.Invoke();
        }

        public IEnumerable<Order> GetOrders(Order order)
        {
            List<Order> orders = new List<Order>();
            using (var session = driver.Session())
            {
                var result = session.WriteTransaction(transaction =>
                {
                    string tr = "MATCH (u)-[o:Order]->(s) ";

                    if (order != null)
                    {
                        string where = "";
                        if (order.UID != null)
                        {
                            if (where != "") where += " AND ";
                            where += $"id(u) = $UID";
                        }
                        if (order.SID != null)
                        {
                            if (where != "") where += " AND ";
                            where += $"id(s) = $SID";
                        }
                        if (where != "")
                            tr += " WHERE " + where;
                    }

                    tr += " RETURN o";

                    var _result = transaction.Run(tr, order);
                    return _result;
                });
                foreach (IRecord record in result)
                {
                    foreach (var pair in record.Values)
                    {
                        Order o = new Order();
                        IRelationship relationship = pair.Value as IRelationship;

                        o.ID = Convert.ToInt32(relationship.Id);
                        o.UID = Convert.ToInt32(relationship.StartNodeId);
                        o.SID = Convert.ToInt32(relationship.EndNodeId);

                        if (relationship.Properties.Keys.Contains("datefrom"))
                            o.DateFrom = Convert.ToDateTime(relationship.Properties["datefrom"]);

                        if (relationship.Properties.Keys.Contains("dateto"))
                            o.DateTo = Convert.ToDateTime(relationship.Properties["dateto"]);

                        orders.Add(o);
                    }
                }
            }
            return orders;
        }

        public IEnumerable<Server> GetServers(Server server)
        {
            List<Server> servers = new List<Server>();
            using (var session = driver.Session())
            {
                var result = session.WriteTransaction(transaction =>
                {
                    string tr = "MATCH (s:Server) ";

                    if (server != null)
                    {
                        string where = "";
                        if (!IsNullOrEmpty(server.Processor))
                        {
                            if (where != "") where += " AND ";
                            where += $"s.processor CONTAINS $Processor";
                        }
                        if (!IsNullOrEmpty(server.Country))
                        {
                            if (where != "") where += " AND ";
                            where += $"s.country CONTAINS $Country";
                        }
                        if (server.RAM != null)
                        {
                            if (where != "") where += " AND ";
                            where += $"s.RAM = $RAM";
                        }
                        if (server.SSD != null)
                        {
                            if (where != "") where += " AND ";
                            where += $"s.SSD = $SSD";
                        }
                        if (where != "")
                            tr += " WHERE " + where;
                    }

                    tr += " RETURN s";

                    var _result = transaction.Run(tr, server);
                    return _result;
                });
                foreach (IRecord record in result)
                {
                    foreach (var pair in record.Values)
                    {
                        Server s = new Server();
                        INode node = pair.Value as INode;

                        s.ID = Convert.ToInt32(node.Id);

                        if (node.Properties.Keys.Contains("processor"))
                            s.Processor = node.Properties["processor"].ToString();

                        if (node.Properties.Keys.Contains("country"))
                            s.Country = node.Properties["country"].ToString();

                        if (node.Properties.Keys.Contains("RAM"))
                            s.RAM = Convert.ToInt32(node.Properties["RAM"]);

                        if (node.Properties.Keys.Contains("SSD"))
                            s.SSD = Convert.ToInt32(node.Properties["SSD"]);

                        servers.Add(s);
                    }
                }
            }
            return servers;
        }

        public IEnumerable<User> GetUsers(User user)
        {
            List<User> users = new List<User>();
            using (var session = driver.Session())
            {
                var result = session.WriteTransaction(transaction =>
                {
                    string tr = "MATCH (u:User) ";

                    if(user != null)
                    {
                        string where = "";
                        if (!IsNullOrEmpty(user.Name))
                        {
                            if (where != "") where += " AND ";
                            where += $"u.name CONTAINS $Name";
                        }
                        if (!IsNullOrEmpty(user.Surname))
                        {
                            if (where != "") where += " AND ";
                            where += $"u.surname CONTAINS $Surname";
                        }
                        if (!IsNullOrEmpty(user.Company))
                        {
                            if (where != "") where += " AND ";
                            where += $"u.company CONTAINS $Company";
                        }
                        if (!IsNullOrEmpty(user.Email))
                        {
                            if (where != "") where += " AND ";
                            where += $"u.email CONTAINS $Email";
                        }
                        if (where != "")
                            tr += " WHERE " + where;
                    }

                    tr += " RETURN u";

                    var _result = transaction.Run(tr, user);
                    return _result;
                });
                foreach (IRecord record in result)
                {
                    foreach (var pair in record.Values)
                    {
                        User u = new User();
                        INode node = pair.Value as INode;

                        u.ID = Convert.ToInt32(node.Id);

                        if (node.Properties.Keys.Contains("name"))
                            u.Name = node.Properties["name"].ToString();

                        if (node.Properties.Keys.Contains("surname"))
                            u.Surname = node.Properties["surname"].ToString();

                        if (node.Properties.Keys.Contains("company"))
                            u.Company = node.Properties["company"].ToString();

                        if (node.Properties.Keys.Contains("email"))
                            u.Email = node.Properties["email"].ToString();

                        users.Add(u);
                    }
                }
            }
            return users;
        }

        public void UpdateOrder(Order order)
        {
            if (order.UID == null)
                throw new RequiredFieldException("UID");
            if (order.SID == null)
                throw new RequiredFieldException("SID");

            using (var session = driver.Session())
            {
                var result = session.WriteTransaction(transaction =>
                {
                    string tr = "MATCH ()-[o:Order]->() WHERE id(o) = $ID " +
                        " MATCH (s: Server) WHERE id(s) = $SID" +
                        " MATCH (u: User) WHERE id(u) = $UID" +
                        " MERGE (u)-[onew: Order]->(s)" +
                        " SET onew = o";
                    string values = "";
                    if (order.DateFrom != null)
                        values += ", onew.datefrom = $DateFrom";
                    if (order.DateTo != null)
                        values += ", onew.dateto = $DateTo";

                    tr += values + " DELETE o";
                    var _result = transaction.Run(tr, order);
                    return _result;
                });
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

            using (var session = driver.Session())
            {
                var result = session.WriteTransaction(transaction =>
                {
                    string tr = "MATCH (s:Server) WHERE id(s) = $ID SET s.processor = $Processor, s.RAM = $RAM, s.SSD = $SSD";
                    if (!IsNullOrEmpty(server.Country))
                        tr += ", s.country = $Country";

                    var _result = transaction.Run(tr, server);
                    return _result;
                });
            }
            SUpdated?.Invoke();
        }

        public void UpdateUser(User user)
        {
            if (IsNullOrEmpty(user.Surname))
                throw new RequiredFieldException("Surname");
            if (IsNullOrEmpty(user.Email))
                throw new RequiredFieldException("Email");

            using (var session = driver.Session())
            {
                var result = session.WriteTransaction(transaction =>
                {
                    string tr = "MATCH (u:User) WHERE id(u) = $ID SET u.surname = $Surname, u.email = $Email";
                    if (!IsNullOrEmpty(user.Name))
                        tr += ", u.name = $Name";
                    if (!IsNullOrEmpty(user.Company))
                        tr += ", u.company = $Company";

                    var _result = transaction.Run(tr, user);
                    return _result;
                });
            }
            UUpdated?.Invoke();
        }
    }
}
