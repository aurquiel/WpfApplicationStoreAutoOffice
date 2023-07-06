using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IniitalDataClassLibrary
{
    public class User
    {
        private string id;
        private string alias;

        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public string Alias
        {
            get
            {
                return alias;
            }

            set
            {
                alias = value;
            }
        }

        public User(string id, string alias)
        {
            this.Id = id;
            this.Alias = alias;
        }
    }
}
