using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotEnoughDB.Exceptions;
using NotEnoughDB.Models;

namespace NotEnoughDB.Controllers
{
    public class XMLControl : IController
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

        private DataSet set;
        private DataTable users, orders, servers;
        private readonly string schemepath = "../../../../databases/XML/hosting.xsd";
        private readonly string dbpath = "../../../../databases/XML/hosting.xml";

        public XMLControl()
        {
            set = new DataSet();
            if (File.Exists(schemepath))
            {
                set.ReadXmlSchema(schemepath);

                users = set.Tables["Users"];
                servers = set.Tables["Servers"];
                orders = set.Tables["Orders"];
            }
            else
            {
                users = set.Tables.Add("Users");
                servers = set.Tables.Add("Servers");
                orders = set.Tables.Add("Orders");

                var uid = users.Columns.Add("ID", typeof(int));
                users.Columns.Add("Name", typeof(string));
                users.Columns.Add("Surname", typeof(string));
                users.Columns.Add("Email", typeof(string));
                users.Columns.Add("Company", typeof(string));

                var sid = servers.Columns.Add("ID", typeof(int));
                servers.Columns.Add("RAM", typeof(int));
                servers.Columns.Add("SSD", typeof(int));
                servers.Columns.Add("Processor", typeof(string));
                servers.Columns.Add("Country", typeof(string));

                var oid = orders.Columns.Add("ID", typeof(int));
                var osid = orders.Columns.Add("SID", typeof(int));
                var ouid = orders.Columns.Add("UID", typeof(int));
                orders.Columns.Add("DateFrom", typeof(DateTime));
                orders.Columns.Add("DateTo", typeof(DateTime));

                set.Relations.Add(uid, ouid);
                set.Relations.Add(sid, osid);

                uid.AutoIncrement = true;
                sid.AutoIncrement = true;
                oid.AutoIncrement = true;
                users.PrimaryKey = new[] { uid };
                servers.PrimaryKey = new[] { sid };
                orders.PrimaryKey = new[] { oid };

                set.WriteXmlSchema(schemepath);
            }

            if (File.Exists(dbpath))
                set.ReadXml(dbpath);

            Action write = () => set.WriteXml(dbpath);
            UUpdated += write;
            SUpdated += write;
            OUpdated += write;
        }

        private bool IsNullOrEmpty(string str) => str == null || str == string.Empty;

        public void AddOrder(Order order)
        {
            if (order.UID == null)
                throw new RequiredFieldException("UID");
            if (order.SID == null)
                throw new RequiredFieldException("SID");

            var row = orders.Rows.Add();
            row["UID"] = order.UID;
            row["SID"] = order.SID;

            if (order.DateFrom != null)
                row["DateFrom"] = order.DateFrom;
            if (order.DateTo != null)
                row["DateTo"] = order.DateTo;

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

            var row = servers.Rows.Add();
            row["Processor"] = server.Processor;
            row["RAM"] = server.RAM;
            row["SSD"] = server.SSD;

            if (!IsNullOrEmpty(server.Country))
                row["Country"] = server.Country;

            SUpdated?.Invoke();
        }

        public void AddUser(User user)
        {
            if (IsNullOrEmpty(user.Surname))
                throw new RequiredFieldException("Surname");
            if (IsNullOrEmpty(user.Email))
                throw new RequiredFieldException("Email");

            var row = users.Rows.Add();
            row["Surname"] = user.Surname;
            row["Email"] = user.Email;

            if(!IsNullOrEmpty(user.Name))
                row["Name"] = user.Name;
            if (!IsNullOrEmpty(user.Company))
                row["Company"] = user.Company;

            UUpdated?.Invoke();
        }

        public void DeleteOrder(Order order)
        {
            orders.Rows.Remove(orders.Rows.Find(order.ID));
            OUpdated?.Invoke();
        }

        public void DeleteServer(Server server)
        {
            servers.Rows.Remove(servers.Rows.Find(server.ID));
            SUpdated?.Invoke();
        }

        public void DeleteUser(User user)
        {
            users.Rows.Remove(users.Rows.Find(user.ID));
            UUpdated?.Invoke();
        }

        public IEnumerable<Order> GetOrders(Order order)
        {
            var list = new List<Order>();
            foreach (DataRow item in orders.Rows)
            {
                var o = new Order();
                o.ID = Convert.ToInt32(item["ID"]);
                o.UID = Convert.ToInt32(item["UID"]);
                o.SID = Convert.ToInt32(item["SID"]);
                if (item["DateFrom"] != DBNull.Value)
                    o.DateFrom = Convert.ToDateTime(item["DateFrom"]);
                else o.DateFrom = null;
                if (item["DateTo"] != DBNull.Value)
                    o.DateTo = Convert.ToDateTime(item["DateTo"]);
                else o.DateTo = null;
                list.Add(o);
            }
            if (order != null) list.RemoveAll(v =>
            {
                if (order.UID != null && (v.UID == null || v.UID != order.UID)) return true;
                if (order.SID != null && (v.SID == null || v.SID != order.SID)) return true;
                if (order.DateFrom != null && (v.DateFrom == null || v.DateFrom != order.DateFrom)) return true;
                if (order.DateTo != null && (v.DateTo == null || v.DateTo != order.DateTo)) return true;
                return false;
            });
            return list;
        }

        public IEnumerable<Server> GetServers(Server server)
        {
            var list = new List<Server>();
            foreach (DataRow item in servers.Rows)
            {
                var s = new Server();
                s.ID = Convert.ToInt32(item["ID"]);
                s.SSD = Convert.ToInt32(item["SSD"]);
                s.RAM = Convert.ToInt32(item["RAM"]);
                s.Processor = item["Processor"] != DBNull.Value ? item["Processor"].ToString() : null;
                s.Country = item["Country"] != DBNull.Value ? item["Country"].ToString() : null;
                list.Add(s);
            }
            if (server != null) list.RemoveAll(v =>
            {
                if (!IsNullOrEmpty(server.Processor) && (IsNullOrEmpty(v.Processor) || !v.Processor.Contains(server.Processor))) return true;
                if (!IsNullOrEmpty(server.Country) && (IsNullOrEmpty(v.Country) || !v.Country.Contains(server.Country))) return true;
                if (server.RAM != null && (v.RAM == null || v.RAM != server.RAM)) return true;
                if (server.SSD != null && (v.SSD == null || v.SSD != server.SSD)) return true;
                return false;
            });
            return list;
        }

        public IEnumerable<User> GetUsers(User user)
        {
            var list = new List<User>();
            foreach (DataRow item in users.Rows)
            {
                var u = new User();
                u.ID = Convert.ToInt32(item["ID"]);
                u.Name = item["Name"] != DBNull.Value ? item["Name"].ToString() : null; ;
                u.Surname = item["Surname"].ToString();
                u.Company = item["Company"] != DBNull.Value ? item["Company"].ToString() : null;
                u.Email = item["Email"].ToString();
                list.Add(u);
            }
            if (user != null) list.RemoveAll(v =>
            {
                if (!IsNullOrEmpty(user.Name) && (IsNullOrEmpty(v.Name) || !v.Name.Contains(user.Name))) return true;
                if (!IsNullOrEmpty(user.Surname) && (IsNullOrEmpty(v.Surname) || !v.Surname.Contains(user.Surname))) return true;
                if (!IsNullOrEmpty(user.Company) && (IsNullOrEmpty(v.Company) || !v.Company.Contains(user.Company))) return true;
                if (!IsNullOrEmpty(user.Email) && (IsNullOrEmpty(v.Email) || !v.Email.Contains(user.Email))) return true;
                return false;
            });
            return list;
        }

        public void UpdateOrder(Order order)
        {
            if (order.UID == null)
                throw new RequiredFieldException("UID");
            if (order.SID == null)
                throw new RequiredFieldException("SID");

            var row = orders.Rows.Find(order.ID);
            row["UID"] = order.UID;
            row["SID"] = order.SID;
            row["DateFrom"] = order.DateFrom;
            row["DateTo"] = order.DateTo;

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

            var row = servers.Rows.Find(server.ID);
            row["Processor"] = server.Processor;
            row["RAM"] = server.RAM;
            row["SSD"] = server.SSD;
            row["Country"] = server.Country;

            SUpdated?.Invoke();
        }

        public void UpdateUser(User user)
        {
            if (IsNullOrEmpty(user.Surname))
                throw new RequiredFieldException("Surname");
            if (IsNullOrEmpty(user.Email))
                throw new RequiredFieldException("Email");

            var row = users.Rows.Find(user.ID);
            row["Surname"] = user.Surname;
            row["Email"] = user.Email;
            row["Name"] = user.Name;
            row["Company"] = user.Company;

            UUpdated?.Invoke();
        }
    }
}
