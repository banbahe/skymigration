using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Configuration;
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
    }
    class Program
    {
        static IActivity ctrlActivity = new ActivityController();
        static string CurrentFilePath = ConfigurationManager.AppSettings["currentfile"].ToString();


        static void Main(string[] args)
        {
          
            List<Activity> list = ctrlActivity.GetFromCSV(CurrentFilePath);

            foreach (var item in list)
                ctrlActivity.Create(item);
            
            
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

    }
}
