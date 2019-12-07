using NotEnoughDB.Exceptions;
using NotEnoughDB.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace NotEnoughDB
{
    public enum Entities { User, Server, Order }
    public partial class Data : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion

        #region Properties
        private IController Controller { get; set; }
        private MainWindow Parent { get; set; }

        //UI Properties are in DataProperties

        #endregion

        #region Constructor
        public Data(MainWindow parrent)
        {
            Parent = parrent;


            Servers = new ObservableCollection<Server>();
            Users = new ObservableCollection<User>();
            Orders = new ObservableCollection<Order>();

            CurrentEntity = Entities.User;

            FindCmd     = new Command(_FindCmd);
            AddCmd      = new Command(_AddCmd);
            UpdateCmd   = new Command(_UpdateCmd);
            DeleteCmd   = new Command(_DeleteCmd);
        }
        #endregion

        #region Methods

        public void Initialise(DataBases db)
        {
            Controller = DB.GetController(db);

            Controller.ServersUpdated += _ServersUpdated;
            Controller.UsersUpdated += _UsersUpdated;
            Controller.OrdersUpdated += _OrdersUpdated;
        }
        public bool IsController() => Controller != null;

        public void ChangeEntity(Entities entity)
        {
            switch (entity)
            {
                case Entities.User:
                    Parent.UserGrid.Visibility = System.Windows.Visibility.Visible;
                    Parent.ServerGrid.Visibility = System.Windows.Visibility.Collapsed;
                    Parent.OrderGrid.Visibility = System.Windows.Visibility.Collapsed;
                    CurrentEntity = Entities.User;
                    break;
                case Entities.Server:
                    Parent.UserGrid.Visibility = System.Windows.Visibility.Collapsed;
                    Parent.ServerGrid.Visibility = System.Windows.Visibility.Visible;
                    Parent.OrderGrid.Visibility = System.Windows.Visibility.Collapsed;
                    CurrentEntity = Entities.Server;
                    break;
                case Entities.Order:
                    Parent.UserGrid.Visibility = System.Windows.Visibility.Collapsed;
                    Parent.ServerGrid.Visibility = System.Windows.Visibility.Collapsed;
                    Parent.OrderGrid.Visibility = System.Windows.Visibility.Visible;
                    CurrentEntity = Entities.Order;
                    break;
                default:
                    break;
            }
        }

        private void _ServersUpdated()
        {
            Servers.Clear();
            foreach (var item in Controller.GetServers(null))
                Servers.Add(item);
        }
        private void _UsersUpdated()
        {
            Users.Clear();
            foreach (var item in Controller.GetUsers(null))
                Users.Add(item);
        }
        private void _OrdersUpdated()
        {
            Orders.Clear();
            foreach (var item in Controller.GetOrders(null))
                Orders.Add(item);
        }



        private void _FindCmd()
        {
            switch (CurrentEntity)
            {
                case Entities.User:
                    FindUsers();
                    break;
                case Entities.Server:
                    FindServers();
                    break;
                case Entities.Order:
                    FindOrders();
                    break;
                default:
                    break;
            }
        }
        private void FindUsers()
        {
            User u = GetUser();
            if (u == null) return;

            Users.Clear();
            foreach (var item in Controller.GetUsers(u))
                Users.Add(item);
        }
        private void FindOrders()
        {
            Order o = GetOrder();
            if (o == null) return;

            Orders.Clear();
            foreach (var item in Controller.GetOrders(o))
                Orders.Add(item);
        }
        private void FindServers()
        {
            Server s = GetServer();
            if (s == null) return;

            Servers.Clear();
            foreach (var item in Controller.GetServers(s))
                Servers.Add(item);
        }

        private void _AddCmd()
        {
            switch (CurrentEntity)
            {
                case Entities.User:
                    AddUser();
                    break;
                case Entities.Server:
                    AddServer();
                    break;
                case Entities.Order:
                    AddOrder();
                    break;
                default:
                    break;
            }
        }
        private void AddUser()
        {
            User u = GetUser();
            if (u == null) return;

            if (u.Email != null && !new Regex(@"[a-zA-Z0-9_]+@[a-zA-Z0-9_]+\.[a-zA-Z0-9_]+").IsMatch(u.Email))
            {
                ShowErrorMessage("Email is not valid!");
                return;
            }

            try
            {
                Controller.AddUser(u);
            }
            catch (RequiredFieldException e)
            {
                ShowErrorMessage(e.Field + " is required!");
            }
        }
        private void AddServer()
        {
            Server s = GetServer();
            if (s == null) return;

            try
            {
                Controller.AddServer(s);
            }
            catch (RequiredFieldException e)
            {
                ShowErrorMessage(e.Field + " is required!");
            }
        }
        private void AddOrder()
        {
            Order o = GetOrder();
            if (o == null) return;

            try
            {
                Controller.AddOrder(o);
            }
            catch (RequiredFieldException e)
            {
                ShowErrorMessage(e.Field + " is required!");
            }
        }

        private void _UpdateCmd()
        {
            switch (CurrentEntity)
            {
                case Entities.User:
                    UpdateUser();
                    break;
                case Entities.Server:
                    UpdateServer();
                    break;
                case Entities.Order:
                    UpdateOrder();
                    break;
                default:
                    break;
            }
        }
        private void UpdateUser()
        {
            if(SelectedUser == null)
            {
                ShowErrorMessage("Please select User!");
                return;
            }

            User u = GetUser();
            if (u == null) return;
            u.ID = SelectedUser.ID;

            if (u.Email != null && !new Regex(@"[a-zA-Z0-9_]+@[a-zA-Z0-9_]+\.[a-zA-Z0-9_]+").IsMatch(u.Email))
            {
                ShowErrorMessage("Email is not valid!");
                return;
            }

            try
            {
                Controller.UpdateUser(u);
            }
            catch (RequiredFieldException e)
            {
                ShowErrorMessage(e.Field + " is required!");
            }
        }
        private void UpdateServer()
        {
            if (SelectedServer == null)
            {
                ShowErrorMessage("Please select Server!");
                return;
            }
            Server s = GetServer();
            if (s == null) return;
            s.ID = SelectedServer.ID;

            try
            {
                Controller.UpdateServer(s);
            }
            catch (RequiredFieldException e)
            {
                ShowErrorMessage(e.Field + " is required!");
            }
        }
        private void UpdateOrder()
        {
            if (SelectedOrder == null)
            {
                ShowErrorMessage("Please select Order!");
                return;
            }
            Order o = GetOrder();
            if (o == null) return;
            o.ID = SelectedOrder.ID;

            try
            {
                Controller.UpdateOrder(o);
            }
            catch (RequiredFieldException e)
            {
                ShowErrorMessage(e.Field + " is required!");
            }
        }

        private void _DeleteCmd()
        {
            switch (CurrentEntity)
            {
                case Entities.User:
                    DeleteUser();
                    break;
                case Entities.Server:
                    DeleteServer();
                    break;
                case Entities.Order:
                    DeleteOrder();
                    break;
                default:
                    break;
            }
        }
        private void DeleteUser()
        {
            if (SelectedUser == null)
            {
                ShowErrorMessage("Please select User!");
                return;
            }
            try
            {
                Controller.DeleteUser(SelectedUser.ID);
            }
            catch (RequiredFieldException e)
            {
                ShowErrorMessage(e.Field + " is required!");
            }
        }
        private void DeleteServer()
        {
            if (SelectedServer == null)
            {
                ShowErrorMessage("Please select User!");
                return;
            }
            try
            {
                Controller.DeleteServer(SelectedServer.ID);
            }
            catch (RequiredFieldException e)
            {
                ShowErrorMessage(e.Field + " is required!");
            }
        }
        private void DeleteOrder()
        {
            if (SelectedOrder == null)
            {
                ShowErrorMessage("Please select User!");
                return;
            }
            try
            {
                Controller.DeleteOrder(SelectedOrder.ID);
            }
            catch (RequiredFieldException e)
            {
                ShowErrorMessage(e.Field + " is required!");
            }
        }

        private bool IsNullOrEmpty(string str) => str == null || str == string.Empty;

        public void ShowErrorMessage(string text) 
            => MessageBox.Show(text, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);

        public User GetUser()
        {
            User u = new User()
            {
                Name = UserName,
                Surname = UserSurname,
                Company = UserCompany,
                Email = UserEmail
            };
            return u;
        }
        public Server GetServer()
        {

            Server s = new Server()
            {
                Processor = ServerProcessor,
                Country = ServerCountry
            };
            if (!IsNullOrEmpty(ServerRAM))
            {
                try
                {
                    s.RAM = Convert.ToInt32(ServerRAM);
                }
                catch (Exception)
                {
                    ShowErrorMessage("RAM is not valid!");
                    return null;
                }
            }
            else s.RAM = null;
            if (!IsNullOrEmpty(ServerSSD))
            {
                try
                {
                    s.SSD = Convert.ToInt32(ServerSSD);
                }
                catch (Exception)
                {
                    ShowErrorMessage("SSD is not valid!");
                    return null;
                }
            }
            else s.SSD = null;
            return s;
        }
        public Order GetOrder()
        {
            Order o = new Order();

            if (!IsNullOrEmpty(OrderUser))
            {
                try
                {
                    o.UID = Convert.ToInt32(OrderUser);
                }
                catch (Exception)
                {
                    ShowErrorMessage("User ID is not valid!");
                    return null;
                }
            }
            else o.UID = null;
            if (!IsNullOrEmpty(OrderServer))
            {
                try
                {
                    o.SID = Convert.ToInt32(OrderServer);
                }
                catch (Exception)
                {
                    ShowErrorMessage("Server ID is not valid!");
                    return null;
                }
            }
            else o.SID = null;

            if (IsOrderFromSelected)
                o.DateFrom = OrderFrom;
            else o.DateFrom = null;

            if (IsOrderToSelected)
                o.DateTo = OrderTo;
            else o.DateTo = null;

            return o;
        }

        #endregion
    }
}
