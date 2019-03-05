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
            Program.Logger(string.Format("******************************"), TypeLog.DEFAULT);
            Program.Logger(string.Format(" Inicia proceso {0} ", DateTime.Now), TypeLog.DEFAULT);

            List<Activity> list = ctrlActivity.GetFromCSV(CurrentFilePath);

            int count = 0;
            foreach (var item in list)
            {
                Console.Clear();
                Console.WriteLine(string.Format("{0} de un total de {1} Actividades", count += 1, list.Count));
                ctrlActivity.Create(item);
            }


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
