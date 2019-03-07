using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skymigration
{

    public class ActivityKeys
    {
        public int activityId { get; set; }
        public string apptNumber { get; set; }
        public string customerNumber { get; set; }
    }

    public class Error
    {
        public string operation { get; set; }
        public string errorDetail { get; set; }
    }

    public class Result
    {
        public ActivityKeys activityKeys { get; set; }
        public List<string> operationsPerformed { get; set; }
        public List<Error> errors { get; set; }
        public List<string> operationsFailed { get; set; }
    }

    public class Link
    {
        public string rel { get; set; }
        public string href { get; set; }
    }

    public class RootActivityBulk
    {
        public List<Result> results { get; set; }
        public List<Link> links { get; set; }
    }

}
