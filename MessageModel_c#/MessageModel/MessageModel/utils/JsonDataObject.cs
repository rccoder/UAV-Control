using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageModel.utils
{
    class JsonDataObject
    {
        public string command { set; get; }
        public Dictionary<string, string> options { get; set; }
    }
}
