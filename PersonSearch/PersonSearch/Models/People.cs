using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonSearch.Models
{
    public class People
    {
        public Int64 ID { get; set; }
        public string NAME { get; set; }
        public string GENDER { get; set; }
        public string BIRTHPLACE { get; set; }
        public Int64 FATHER_ID { get; set; }
        public Int64 MOTHER_ID { get; set; }
        public Int32 LEVEL { get; set; }
    }

}
