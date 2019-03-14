using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skymigration
{
    public interface IActivity
    {
        List<Activity> GetFromCSV(string path);
        Activity Create(Activity activity);
        RootActivityBulk CreateBulk(RootActivity activities, int requestnumber);
    }
}
