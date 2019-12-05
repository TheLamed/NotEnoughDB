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


        #endregion

        #region Constructor
        public Data()
        {

        }
        #endregion

        #region Methods

        public void Initialise(DataBases db)
            => Controller = DB.GetController(db);
        public bool IsController() => Controller != null;

        #endregion
    }
}
