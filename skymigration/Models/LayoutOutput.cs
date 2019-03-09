using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skymigration
{
    public class LayoutOutput
    {
        public int activityId { get; set; }
        public string apptNumber { get; set; }
        public string typeCommand { get; set; } = "Bulk Update";
        public string resourceId { get; set; }
        public string date { get; set; }
        public string timeSlot { get; set; }
        public string horarioAgendado { get; set; }
        public string result{ get; set; }
        public string typeMessage { get; set; } = "insert";
        public int cod { get; set; } = 0;
        public string description { get; set; }
    }
}
