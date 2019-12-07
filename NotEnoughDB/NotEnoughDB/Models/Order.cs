using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotEnoughDB.Models
{
    public class Order
    {
        public int ID { get; set; }
        public int? UID { get; set; }
        public int? SID { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public override string ToString() => $"ID: {ID,4} User: {UID,4} Server: {SID,4}\nFrom: {DateFrom?.ToString("yyyy-MM-dd"),10} To: {DateTo?.ToString("yyyy-MM-dd"),10}";
    }
}
