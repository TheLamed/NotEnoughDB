using NotEnoughDB.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotEnoughDB
{
    public enum DataBases { SQLite, Neo4j, FireBase }
    public static class DB
    {
        public static IController GetController(DataBases db)
        {
            switch (db)
            {
                case DataBases.SQLite:  return new SQLiteControl();
                case DataBases.Neo4j:   return new Neo4jControl();
                case DataBases.FireBase: return new FireBaseControl();


                default: return null;
            }
        }
    }
}
