using NotEnoughDB.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotEnoughDB
{
    public enum DataBases { SQLite }
    public static class DB
    {
        public static IController GetController(DataBases db)
        {
            switch (db)
            {
                case DataBases.SQLite: return new SQLiteControl();





                default: return null;
            }
        }
    }
}
