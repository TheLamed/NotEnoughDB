using NotEnoughDB.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotEnoughDB
{
    public enum DataBases { SQLite, Neo4j, FireBase, MongoDB, OrientDB, XML }
    public static class DB
    {
        public static DataBases CurrentDB { get; private set; }
        public static IController GetController(DataBases db)
        {
            CurrentDB = db;
            switch (db)
            {
                case DataBases.SQLite:  return new SQLiteControl();
                case DataBases.Neo4j:   return new Neo4jControl();
                case DataBases.FireBase: return new FireBaseControl();
                case DataBases.MongoDB: return new MongoDBControl();
                case DataBases.OrientDB: return new OrientDBControl();
                case DataBases.XML: return new XMLControl();

                default: return null;
            }
        }
    }
}
