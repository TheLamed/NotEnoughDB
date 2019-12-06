using NotEnoughDB.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NotEnoughDB
{
    enum Entities { User, Server, Order }
    class Data : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion

        #region Properties
        private IController Controller { get; set; }
        private MainWindow Parent { get; set; }

        public ObservableCollection<Server> Servers { get; set; }
        public ObservableCollection<Server> Users { get; set; }
        public ObservableCollection<Server> Orders { get; set; }

        private Server _SelectedServer;
        public Server SelectedServer
        {
            get => _SelectedServer;
            set
            {
                _SelectedServer = value;
                NotifyPropertyChanged();
            }
        }

        private User _SelectedUser;
        public User SelectedUser
        {
            get => _SelectedUser;
            set
            {
                _SelectedUser = value;
                NotifyPropertyChanged();
            }
        }

        private Order _SelectedOrder;
        public Order SelectedOrder
        {
            get => _SelectedOrder;
            set
            {
                _SelectedOrder = value;
                NotifyPropertyChanged();
            }
        }



        public Command FindCmd { get; set; }
        public Command AddCmd { get; set; }
        public Command UpdateCmd { get; set; }
        public Command DeleteCmd { get; set; }
        #endregion

        #region Constructor
        public Data(MainWindow parrent)
        {
            Parent = parrent;


            Servers = new ObservableCollection<Server>();
            Users = new ObservableCollection<Server>();
            Orders = new ObservableCollection<Server>();

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
                    break;
                case Entities.Server:
                    Parent.UserGrid.Visibility = System.Windows.Visibility.Collapsed;
                    Parent.ServerGrid.Visibility = System.Windows.Visibility.Visible;
                    Parent.OrderGrid.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case Entities.Order:
                    Parent.UserGrid.Visibility = System.Windows.Visibility.Collapsed;
                    Parent.ServerGrid.Visibility = System.Windows.Visibility.Collapsed;
                    Parent.OrderGrid.Visibility = System.Windows.Visibility.Visible;
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



        private void _FindCmd()
        {

        }

        private void _AddCmd()
        {

        }

        private void _UpdateCmd()
        {

        }

        private void _DeleteCmd()
        {

        }

        #endregion
    }
}
