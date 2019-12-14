using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotEnoughDB.Exceptions;
using NotEnoughDB.Models;
using Orient.Client;

namespace NotEnoughDB.Controllers
{
    public class OrientDBControl : IController
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

        private ODatabase db;

        public OrientDBControl()
        {
            db = new ODatabase("localhost", 2424, "NotEnoughDB", ODatabaseType.Graph, "NotEnoughUser", "user");

        }

        private bool IsNullOrEmpty(string str) => str == null || str == string.Empty;

        public void AddOrder(Order order)
        {
            if (order.UID == null)
                throw new RequiredFieldException("UID");
            if (order.SID == null)
                throw new RequiredFieldException("SID");

            var q = db.Create.Edge("Order")
                .From(new ORID((short)order.UID, order.UID_pos ?? 0))
                .To(new ORID((short)order.SID, order.SID_pos ?? 0));

            if (order.DateFrom != null) q = q.Set("DateFrom", order.DateFrom?.ToString());
            if (order.DateTo != null) q = q.Set("DateTo", order.DateTo?.ToString());

            q.Run();

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

            var q = db.Insert().Into("Server")
                .Set("Processor", server.Processor)
                .Set("RAM", server.RAM)
                .Set("SSD", server.SSD);

            if (!IsNullOrEmpty(server.Country)) q = q.Set("Country", server.Country);

            q.Run();

            SUpdated?.Invoke();
        }

        public void AddUser(User user)
        {
            if (IsNullOrEmpty(user.Surname))
                throw new RequiredFieldException("Surname");
            if (IsNullOrEmpty(user.Email))
                throw new RequiredFieldException("Email");

            var q = db.Insert().Into("User")
                .Set("Surname", user.Surname)
                .Set("Email", user.Email);

            if (!IsNullOrEmpty(user.Name)) q = q.Set("Name", user.Name);
            if (!IsNullOrEmpty(user.Company)) q = q.Set("Company", user.Company);

            q.Run();

            UUpdated?.Invoke();
        }

        public void DeleteOrder(Order order)
        {
            db.Delete.Edge("Order").Where("@rid").Equals($"#{order.ID}:{order.ID_pos}").Run();
            OUpdated?.Invoke();
        }

        public void DeleteServer(Server server)
        {
            db.Delete.Vertex("Server").Where("@rid").Equals($"#{server.ID}:{server.ID_pos}").Run();
            SUpdated?.Invoke();
        }

        public void DeleteUser(User user)
        {
            db.Delete.Vertex("User").Where("@rid").Equals($"#{user.ID}:{user.ID_pos}").Run();
            UUpdated?.Invoke();
        }

        public IEnumerable<Order> GetOrders(Order order)
        {
            var q = db.Select().From("Order");

            if (order != null)
            {
                bool w = true;
                if (order.UID != null)
                {
                    if (w) q = q.Where("out");
                    else q = q.And("out");
                    
                    q = q.Equals($"#{order.UID}:{order.UID_pos ?? 0}");
                    w = false;
                }
                if (order.SID != null)
                {
                    if (w) q = q.Where("in");
                    else q = q.And("in");
                    q = q.Equals($"#{order.SID}:{order.SID_pos ?? 0}");
                    w = false;
                }
            }

            return q.ToList().Select(v =>
            {
                var u = new Order();
                if (v.Keys.Contains("DateFrom"))
                    u.DateFrom = Convert.ToDateTime(v["DateFrom"]);
                if (v.Keys.Contains("DateTo"))
                    u.DateTo = Convert.ToDateTime(v["DateTo"]);
                u.ID = v.ORID.ClusterId;
                u.ID_pos = v.ORID.ClusterPosition;
                var OUT = (v["out"] as ORID);
                u.UID = OUT.ClusterId;
                u.UID_pos = OUT.ClusterPosition;
                var IN = (v["in"] as ORID);
                u.SID = IN.ClusterId;
                u.SID_pos = IN.ClusterPosition;
                return u;
            });
        }

        public IEnumerable<Server> GetServers(Server server)
        {
            var q = db.Select().From("Server");

            if (server != null)
            {
                bool w = true;
                if (!IsNullOrEmpty(server.Processor))
                {
                    if (w) q = q.Where("Processor");
                    else q = q.And("Processor");
                    q = q.Like($"%{server.Processor}%");
                    w = false;
                }
                if (!IsNullOrEmpty(server.Country))
                {
                    if (w) q = q.Where("Country");
                    else q = q.And("Country");
                    q = q.Like($"%{server.Country}%");
                    w = false;
                }
                if (server.RAM != null)
                {
                    if (w) q = q.Where("RAM");
                    else q = q.And("RAM");
                    q = q.Equals(server.RAM);
                    w = false;
                }
                if (server.SSD != null)
                {
                    if (w) q = q.Where("SSD");
                    else q = q.And("SSD");
                    q = q.Equals(server.SSD);
                    w = false;
                }
            }

            return q.ToList().Select(v =>
            {
                var u = v.To<Server>();
                u.ID = v.ORID.ClusterId;
                u.ID_pos = v.ORID.ClusterPosition;
                return u;
            });
        }

        public IEnumerable<User> GetUsers(User user)
        {
            var q = db.Select().From("User");

            if(user != null)
            {
                bool w = true;
                if (!IsNullOrEmpty(user.Name))
                {
                    if (w) q = q.Where("Name");
                    else q = q.And("Name");
                    q = q.Like($"%{user.Name}%");
                    w = false;
                }
                if (!IsNullOrEmpty(user.Surname))
                {
                    if (w) q = q.Where("Surname");
                    else q = q.And("Surname");
                    q = q.Like($"%{user.Surname}%");
                    w = false;
                }
                if (!IsNullOrEmpty(user.Company))
                {
                    if (w) q = q.Where("Company");
                    else q = q.And("Company");
                    q = q.Like($"%{user.Company}%");
                    w = false;
                }
                if (!IsNullOrEmpty(user.Email))
                {
                    if (w) q = q.Where("Email");
                    else q = q.And("Email");
                    q = q.Like($"%{user.Email}%");
                    w = false;
                }
            }

            return q.ToList().Select(v =>
            {
                var u = v.To<User>();
                u.ID = v.ORID.ClusterId;
                u.ID_pos = v.ORID.ClusterPosition;
                return u;
            });
        }

        public void UpdateOrder(Order order)
        {
            if (order.UID == null)
                throw new RequiredFieldException("UID");
            if (order.SID == null)
                throw new RequiredFieldException("SID");

            var q = db.Update(new ORID((short)order.ID, order.ID_pos ?? 0))
                .Set("out", new ORID((short)order.UID, order.UID_pos ?? 0))
                .Set("in", new ORID((short)order.SID, order.SID_pos ?? 0))
                .Set("DateFrom", order.DateFrom?.ToString())
                .Set("DateTo", order.DateTo?.ToString());

            q.Run();

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

            var q = db.Update(new ORID((short)server.ID, server.ID_pos ?? 0))
                .Set("Processor", server.Processor)
                .Set("RAM", server.RAM)
                .Set("SSD", server.SSD)
                .Set("Country", server.Country);

            q.Run();

            SUpdated?.Invoke();
        }

        public void UpdateUser(User user)
        {
            if (IsNullOrEmpty(user.Surname))
                throw new RequiredFieldException("Surname");
            if (IsNullOrEmpty(user.Email))
                throw new RequiredFieldException("Email");

            var q = db.Update(new ORID((short)user.ID, user.ID_pos ?? 0))
                .Set("Surname", user.Surname)
                .Set("Email", user.Email)
                .Set("Name", user.Name)
                .Set("Company", user.Company);

            q.Run();
            UUpdated?.Invoke();
        }
    }
}
