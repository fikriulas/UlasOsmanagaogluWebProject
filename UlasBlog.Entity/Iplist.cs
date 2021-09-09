using System;
using System.Collections.Generic;
using System.Text;

namespace UlasBlog.Entity
{
    public class Iplist
    {
        public int Id { get; set; }
        public string Ip { get; set; }
        public DateTime Date { get; set; }
        public bool Block { get; set; }
        public string Note { get; set; }
    }
}
