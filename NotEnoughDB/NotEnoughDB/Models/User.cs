using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotEnoughDB.Models
{
    public class User
    {
        [BsonId]//Only for MongoDB
        public int ID { get; set; }
        public long? ID_pos { get; set; }//Only for OrientDB
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }

        public override string ToString() => $"ID: {ID,4}{(ID_pos != null ? ":" + ID_pos : "")} Name: {Name} {Surname}\nCompany: {Company}\nEmail: {Email}";
    }
}
