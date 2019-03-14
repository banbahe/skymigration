using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.IO;
using System.Text;

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
        public static Dictionary<string, string> DictionaryTimeSlot = new Dictionary<string, string>()
        {
           {"TS21","Horario 21:00 - 21:59"},
           {"TS20","Horario 20:00 - 20:59"},
           {"TS19","Horario 19:00 - 19:59"},
           {"TS18","Horario 18:00 - 18:59"},
           {"TS17","Horario 17:00 - 17:59"},
           {"TS16","Horario 16:00 - 16:59"},
           {"TS15","Horario 15:00 - 15:59"},
           {"TS14","Horario 14:00 - 14:59"},
           {"TS13","Horario 13:00 - 13:59"},
           {"TS12","Horario 12:00 - 12:59"},
           {"TS11","Horario 11:00 - 11:59"},
           {"TS10","Horario 10:00 - 10:59"},
           {"TS09","Horario 09:00 - 09:59"},
           {"TS08","Horario 08:00 - 08:59"},
           {"TS07","Horario 07:00 - 07:59"},
           {"TSMIX-01","Horario 08:00 a 20:00 hrs"},
           {"TSSIM-01","Horario 00:00 a 23:59 hrs"},
           {"RDBT-01","Horario 9:00 a 11:59 hrs."},
           {"INBT-02","Horario 16:00 a 20:00 hrs."},
           {"RDBT-02","Horario 12:00 a 20:00 hrs."},
           {"INBT-01","Horario 10:00 a 15:59 hrs."}
        };

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" Procesando actividades ");
            int countactivities = 0;
            Program.Logger(string.Format("************************************************************"), TypeLog.DEFAULT);
            Program.Logger(string.Format(" Inicia proceso {0} ", DateTime.Now), TypeLog.DEFAULT);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<Activity> list = ctrlActivity.GetFromCSV(CurrentFilePath);
            
            RootActivity activities = new RootActivity();
            activities.activities = new List<Activity>();
            activities.activities = list;

            activities.updateParameters = new UpdateParameters();
            activities.updateParameters.identifyActivityBy = "apptNumber";
            activities.updateParameters.ifInFinalStatusThen = "createNew";


            int maxcountrequest = 500;
            double numberrequest = Math.Floor((double)activities.activities.Count / maxcountrequest);
            numberrequest = numberrequest == 0 ? 1 : numberrequest;

            List<Activity> full = activities.activities.ToList<Activity>();
            for (int i = 0; i < numberrequest; i++)
            {
                List<Activity> tmpactiviti = full.Skip(i * maxcountrequest).Take(maxcountrequest).ToList<Activity>();
                countactivities += tmpactiviti.Count;
                // Activity
                RootActivity activities2 = new RootActivity();
                activities2.activities = new List<Activity>();
                activities2.activities = tmpactiviti;
                activities2.updateParameters = new UpdateParameters();
                activities2.updateParameters.identifyActivityBy = "apptNumber";
                activities2.updateParameters.ifInFinalStatusThen = "createNew";
                ctrlActivity.CreateBulk(activities2, i);
            }
            stopwatch.Stop();
            string sMessageEnd = string.Format(" Se procesaron {0} actividades en {1} segundos, el tiempo estimado por creación de actividad es de {2}", countactivities, stopwatch.Elapsed.TotalSeconds, (stopwatch.Elapsed.TotalSeconds / countactivities));
            Console.WriteLine(sMessageEnd);
            Program.Logger(sMessageEnd, TypeLog.DEFAULT);
            Program.Logger(string.Format(" Fin de proceso {0} ", DateTime.Now), TypeLog.DEFAULT);
            Program.Logger(string.Format("************************************************************"), TypeLog.DEFAULT);
            Console.ReadLine();
        }


        /// <summary>
        /// 1 log_ok -> activity create success
        /// <br></br> 2 log_not ->  activity not create 
        /// <br></br> 3 log_json -> activity bad fill
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="opcion"></param>
        public static void Logger(String lines, TypeLog typeLog, int requestnumber = 0)
        {
            string @sPath = System.IO.Path.GetDirectoryName(CurrentFilePath);

            try
            {
                string temppath = string.Empty;
                switch (typeLog)
                {
                    case TypeLog.NSHTTPURLResponse:
                        temppath = @sPath + "\\log_NSHTTPURLResponse.txt";
                        break;
                    case TypeLog.OK_REST_ACTIVITY:
                        temppath = @sPath + "\\log_request\\log_" + requestnumber + ".txt";
                        if (!Directory.Exists(Path.GetDirectoryName(temppath)))
                            Directory.CreateDirectory(Path.GetDirectoryName(temppath));

                        break;
                    case TypeLog.BAD_REST_ACTIVITY:
                        temppath = @sPath + "\\log_bad_request.txt";
                        break;
                    case TypeLog.BAD_IO_ACTIVITY:
                        temppath = @sPath + "\\log_error.txt";
                        break;
                    default:
                        temppath = @sPath + "\\log.txt";
                        break;
                }

                System.IO.StreamWriter file = new System.IO.StreamWriter(temppath, true, Encoding.GetEncoding("iso-8859-1"));
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
                System.IO.StreamWriter file = new System.IO.StreamWriter(temppath, true, Encoding.GetEncoding("iso-8859-1"));
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
