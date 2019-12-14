using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotEnoughDB.Models
{
    public class Order
    {
        [BsonId]//Only for MongoDB
        public int ID { get; set; }
        public long? ID_pos { get; set; }//Only for OrientDB
        public int? UID { get; set; }
        public long? UID_pos { get; set; }//Only for OrientDB
        public int? SID { get; set; }
        public long? SID_pos { get; set; }//Only for OrientDB
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public override string ToString()
        {
            if((ID_pos ?? UID_pos ?? SID_pos) == null)
                return $"ID: {ID,4} User: {UID,4} Server: {SID,4}\nFrom: {DateFrom?.ToString("yyyy-MM-dd"),10} To: {DateTo?.ToString("yyyy-MM-dd"),10}";
            return $"ID: {ID,4}{(ID_pos != null ? ":" + ID_pos : "")}" + //Only for OrientDB
                $" User: {UID,4}{(UID_pos != null ? ":" + UID_pos : "")}" +
                $" Server: {SID,4}{(SID_pos != null ? ":" + SID_pos : "")}" +
                $"\nFrom: {DateFrom?.ToString("yyyy-MM-dd"),10} To: {DateTo?.ToString("yyyy-MM-dd"),10}";
        }
    }
}
