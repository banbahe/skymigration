using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Configuration;
using System.Threading.Tasks;
using System.Linq;

namespace skymigration
{
    enum TypeLog
    {
        DEFAULT,
        OK_REST_ACTIVITY,
        BAD_REST_ACTIVITY,
        OK_REST_INVENTORY,
        BAD_REST_INVENTORY,
        BAD_IO_ACTIVITY,
        BAD_IO_ENTITY,
        NSHTTPURLResponse,
    }
    class Program
    {
        static IActivity ctrlActivity = new ActivityController();
        static string CurrentFilePath = ConfigurationManager.AppSettings["currentfile"].ToString();

        static void Main(string[] args)
        {
            Program.Logger(string.Format("******************************"), TypeLog.DEFAULT);
            Program.Logger(string.Format(" Inicia proceso {0} ", DateTime.Now), TypeLog.DEFAULT);

            List<Activity> list = ctrlActivity.GetFromCSV(CurrentFilePath);

            // DISTINCT LIST FOR apptNumber

            var tmp = (from table in list
                       select table.apptNumber).Distinct();
            
            RootActivity activities = new RootActivity();
            activities.activities = new List<Activity>();
            //activities.activities = from tableresult in tmp join tableorign in list equals equals tableorign.apptNumber select tableresult;

            activities.updateParameters = new UpdateParameters();
            activities.updateParameters.identifyActivityBy = "apptNumber";
            activities.updateParameters.ifInFinalStatusThen = "createNew";

            ctrlActivity.CreateBulk(activities);
            //int count = 0;
            //int activity_ok = 0;
            //int activity_bad = 1;

            //Parallel.ForEach(list, item =>
            //{
            //    ctrlActivity.Create(item);
            //});

            //foreach (var item in list)
            //{
            //    Console.Clear();
            //    Console.WriteLine(string.Format("{0} de un total de {1} Actividades", count += 1, list.Count));
            //    var response = ctrlActivity.Create(item);
            //    if (response == null)
            //        activity_bad += 1;
            //    else
            //        activity_ok += 1;
            //}

            Program.Logger(string.Format(" Fin de proceso {0} ", DateTime.Now), TypeLog.DEFAULT);
            Program.Logger(string.Format("******************************"), TypeLog.DEFAULT);

        }


        /// <summary>
        /// 1 log_ok -> activity create success
        /// <br></br> 2 log_not ->  activity not create 
        /// <br></br> 3 log_json -> activity bad fill
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="opcion"></param>
        public static void Logger(String lines, TypeLog typeLog)
        {
            string @sPath = System.IO.Path.GetDirectoryName(CurrentFilePath);

            try
            {
                string temppath = string.Empty;
                switch (typeLog)
                {
                    case TypeLog.NSHTTPURLResponse:
                        temppath = @sPath + "\\log_activity_NSHTTPURLResponse.txt";
                        break;
                    case TypeLog.OK_REST_ACTIVITY:
                        temppath = @sPath + "\\log_activity_create.txt";
                        break;
                    case TypeLog.BAD_REST_ACTIVITY:
                        temppath = @sPath + "\\log_activity_bad_request.txt";
                        break;
                    case TypeLog.BAD_IO_ACTIVITY:
                        temppath = @sPath + "\\log_error.txt";
                        break;
                    default:
                        temppath = @sPath + "\\log.txt";
                        break;
                }
                System.IO.StreamWriter file = new System.IO.StreamWriter(temppath, true);
                file.WriteLine(lines);
                file.Close();
            }
            catch
            {
                // Console.WriteLine(ex.Message);
                Thread.Sleep(800);
                Logger(lines, typeLog);
            }
        }

        public static void LoggerCSV(String lines)
        {
            string @sPath = System.IO.Path.GetDirectoryName(CurrentFilePath);

            try
            {
                string temppath = string.Empty;
                string tmpDate = DateTime.Now.ToString("yyyy-MM-dd");
                temppath = @sPath + "\\layout_output_" + tmpDate + ".csv";
                System.IO.StreamWriter file = new System.IO.StreamWriter(temppath, true);
                file.WriteLine(lines);
                file.Close();
            }
            catch
            {
                // Console.WriteLine(ex.Message);
                Thread.Sleep(888);
                LoggerCSV(lines);
            }
        }
    }
}
