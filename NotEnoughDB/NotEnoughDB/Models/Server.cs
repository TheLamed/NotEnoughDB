﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotEnoughDB.Models
{
    public class Server
    {
        public int ID { get; set; }
        public string Processor { get; set; }
        public int RAM { get; set; }
        public int SSD { get; set; }
        public string Country { get; set; }
    }
}