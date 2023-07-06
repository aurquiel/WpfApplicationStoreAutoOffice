using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IniitalDataClassLibrary
{
    public class InitalData
    {
        private readonly string time_interval_checking_task = string.Empty;

        private DataTable usersData = new DataTable();
        private DataTable storesData = new DataTable();
        private DataTable storesEmployeeData = new DataTable();
        private readonly int userIdAudit;

        public string TIME_INTERVAL_CHECKING_TASK
        {
            get
            {
                return time_interval_checking_task;
            }
        }

       

        public DataTable StoresData
        {
            get
            {
                return storesData;
            }

            set
            {
                storesData = value;
            }
        }

        public DataTable StoresEmployeeData
        {
            get
            {
                return storesEmployeeData;
            }

            set
            {
                storesEmployeeData = value;
            }
        }

        public DataTable UsersData
        {
            get
            {
                return usersData;
            }

            set
            {
                usersData = value;
            }
        }

        public int UserIdAudit
        {
            get
            {
                return userIdAudit;
            }
        }

        public InitalData()
        {

        }

        public InitalData(string time_interval_checking_task, DataTable usersData, DataTable storesData, DataTable storesEmployeeData, int userIdAudit)
        {
            this.time_interval_checking_task = time_interval_checking_task;
            this.UsersData = usersData;
            this.StoresData = storesData;
            this.StoresEmployeeData = storesEmployeeData;
            this.userIdAudit = userIdAudit;
        }
    }
}
