using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kittingStatus.jabil.web.DataModel
{
    public class select2
    {
        private int _id;

        public int id
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _text;

        public string text
        {
            get { return _text; }
            set { _text = value; }
        }
    }
}