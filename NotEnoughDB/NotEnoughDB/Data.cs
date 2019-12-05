using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NotEnoughDB
{
    class Data : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion

        #region Properties
        private IController Controller { get; set; }
        private MainWindow Parrent { get; set; }


        public Command FindCmd { get; set; }
        public Command AddCmd { get; set; }
        public Command UpdateCmd { get; set; }
        public Command DeleteCmd { get; set; }

        #endregion

        #region Constructor
        public Data(MainWindow parrent)
        {
            Parrent = parrent;


            FindCmd     = new Command(_FindCmd);
            AddCmd      = new Command(_AddCmd);
            UpdateCmd   = new Command(_UpdateCmd);
            DeleteCmd   = new Command(_DeleteCmd);
        }
        #endregion

        #region Methods

        public void Initialise(DataBases db)=> Controller = DB.GetController(db);
        public bool IsController() => Controller != null;




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
