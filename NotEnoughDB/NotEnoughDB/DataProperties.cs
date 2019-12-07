using NotEnoughDB.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotEnoughDB
{
    public partial class Data
    {
        public ObservableCollection<Server> Servers { get; set; }
        public ObservableCollection<User> Users { get; set; }
        public ObservableCollection<Order> Orders { get; set; }

        private Server _SelectedServer;
        public Server SelectedServer
        {
            get => _SelectedServer;
            set
            {
                _SelectedServer = value;
                ServerProcessor = value?.Processor;
                ServerCountry = value?.Country;
                ServerRAM = value?.RAM.ToString();
                ServerSSD = value?.SSD.ToString();
                ChangeEntity(Entities.Server);
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
                UserName = value?.Name;
                UserSurname = value?.Surname;
                UserCompany = value?.Company;
                UserEmail = value?.Email;
                ChangeEntity(Entities.User);
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
                OrderUser = value?.UID.ToString();
                OrderServer = value?.SID.ToString();
                OrderFrom = value?.DateFrom;
                OrderTo = value?.DateTo;
                ChangeEntity(Entities.Order);
                NotifyPropertyChanged();
            }
        }

        private string _UserName, _UserSurname, _UserCompany, _UserEmail,
            _ServerProcessor, _ServerCountry, _ServerRAM, _ServerSSD,
            _OrderUser, _OrderServer;

        public string UserName
        {
            get => _UserName;
            set
            {
                _UserName = value;
                NotifyPropertyChanged();
            }
        }
        public string UserSurname
        {
            get => _UserSurname;
            set
            {
                _UserSurname = value;
                NotifyPropertyChanged();
            }
        }
        public string UserCompany
        {
            get => _UserCompany;
            set
            {
                _UserCompany = value;
                NotifyPropertyChanged();
            }
        }
        public string UserEmail
        {
            get => _UserEmail;
            set
            {
                _UserEmail = value;
                NotifyPropertyChanged();
            }
        }
        public string ServerProcessor
        {
            get => _ServerProcessor;
            set
            {
                _ServerProcessor = value;
                NotifyPropertyChanged();
            }
        }
        public string ServerCountry
        {
            get => _ServerCountry;
            set
            {
                _ServerCountry = value;
                NotifyPropertyChanged();
            }
        }
        public string ServerRAM
        {
            get => _ServerRAM;
            set
            {
                _ServerRAM = value;
                NotifyPropertyChanged();
            }
        }
        public string ServerSSD
        {
            get => _ServerSSD;
            set
            {
                _ServerSSD = value;
                NotifyPropertyChanged();
            }
        }
        public string OrderUser
        {
            get => _OrderUser;
            set
            {
                _OrderUser = value;
                NotifyPropertyChanged();
            }
        }
        public string OrderServer
        {
            get => _OrderServer;
            set
            {
                _OrderServer = value;
                NotifyPropertyChanged();
            }
        }

        private DateTime? _OrderFrom, _OrderTo;
        private bool IsOrderFromSelected = false, IsOrderToSelected = false;
        public DateTime? OrderFrom
        {
            get => _OrderFrom;
            set
            {
                _OrderFrom = value;
                IsOrderFromSelected = true;
                NotifyPropertyChanged();
            }
        }
        public DateTime? OrderTo
        {
            get => _OrderTo;
            set
            {
                _OrderTo = value;
                IsOrderToSelected = true;
                NotifyPropertyChanged();
            }
        }

        public Entities CurrentEntity { get; private set; }

        public Command FindCmd { get; set; }
        public Command AddCmd { get; set; }
        public Command UpdateCmd { get; set; }
        public Command DeleteCmd { get; set; }
    }
}
