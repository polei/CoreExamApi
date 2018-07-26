using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminWeb.ViewModel
{
    public class mJsonResult
    {
        public mJsonResult()
        {
            success = false;
        }
        public bool success { get; set; }

        public string msg { get; set; }

        public string data { get; set; }

        public object obj { set; get; }

        public int count { get; set; }

        public int status { get; set; }
    }
}