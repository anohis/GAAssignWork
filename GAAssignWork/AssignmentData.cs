using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GAAssignWork
{
    public class AssignmentData
    {
        public string EmployeeName { get { return Employee.Name; } }
        public string WorkNames
        {
            get
            {
                string str = "";
                foreach (var w in Works)
                {
                    str += w.Name + ",";
                }
                return str;
            }
        }
        public float Value
        {
            get
            {
                float sum = 0;
                foreach (var w in Works)
                {
                    sum += w.Value;
                }
                return sum;
            }
        }

        public EmployeeData Employee;
        public List<WorkData> Works = new List<WorkData>();
    }
}
