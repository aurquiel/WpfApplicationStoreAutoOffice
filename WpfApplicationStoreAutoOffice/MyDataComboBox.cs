using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplicationStoreAutoOffice
{
    internal class MyDataComboBox
    {
        public MyDataComboBox(String value, String displayValue)
        {
            Value = value;
            DisplayValue = displayValue;
        }

        public String Value
        { get; set; }

        public String DisplayValue
        { get; set; }
    }
}
