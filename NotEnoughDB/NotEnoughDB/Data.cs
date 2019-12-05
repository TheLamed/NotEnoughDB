using System;
using System.Collections.Generic;
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



        public Command FindCmd { get; set; }
        public Command AddCmd { get; set; }
        public Command UpdateCmd { get; set; }
        public Command DeleteCmd { get; set; }
        #endregion

        #region Constructor
        public Data(MainWindow parrent)
        {
            Parent = parrent;


            FindCmd     = new Command(_FindCmd);
            AddCmd      = new Command(_AddCmd);
            UpdateCmd   = new Command(_UpdateCmd);
            DeleteCmd   = new Command(_DeleteCmd);
        }
        #endregion

        #region Methods

        public void Initialise(DataBases db)=> Controller = DB.GetController(db);
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
