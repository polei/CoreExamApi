using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.ViewModel
{
    public class mJsonResult
    {
        public mJsonResult()
        {
            success = false;
        }
        public bool success { get; set; }

        public string msg { get; set; }

        public object obj { get; set; }

        public string data { get; set; }
    }
}
