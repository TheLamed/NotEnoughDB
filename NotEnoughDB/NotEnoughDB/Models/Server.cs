﻿using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotEnoughDB.Models
{
    public class Server
    {
        [BsonId]//Only for MongoDB
        public int ID { get; set; }
        public long? ID_pos { get; set; }//Only for OrientDB
        public string Processor { get; set; }
        public int? RAM { get; set; }
        public int? SSD { get; set; }
        public string Country { get; set; }

        public override string ToString() => $"ID: {ID,4}{(ID_pos != null ? ":" + ID_pos : "")} Processor: {Processor,10}\nRAM: {RAM,4} SSD: {SSD,5}";
    }
}
