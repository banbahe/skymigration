
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
namespace skymigration
{
    public class ActivityController : IActivity
    {
        public string Authorization { get; set; } = ConfigurationManager.AppSettings["currentenviroment"].ToString();
        private string source { get; set; }


        public Activity Create(Activity activity)
        {
            Program.Logger(string.Format("|{0}|apptNumber:{1}|resourceId:{2}|", DateTime.Now, activity.apptNumber, activity.resourceId), TypeLog.DEFAULT);
            Activity responseActivity = new Activity();
            string jsonActivy = JsonConvert.SerializeObject(activity, Formatting.None);

            ResponseOFSC result = UtilWebRequest.SendWayAsync("rest/ofscCore/v1/activities", enumMethod.POST, jsonActivy, Authorization);
            result.Content = result.Content.Replace("\n", string.Empty);

            if (result.statusCode >= 200 && result.statusCode < 300)
            {
                responseActivity = JsonConvert.DeserializeObject<Activity>(result.Content);
                Program.Logger(string.Format("|{0}|apptNumber:{1},activityId:{2},resource_id:{3}|{4}|", DateTime.Now, responseActivity.apptNumber, responseActivity.activityId, responseActivity.resourceId, result.Content), TypeLog.OK_REST_ACTIVITY);
            }

            else
                Program.Logger(string.Format("|{0}|apptNumber:{1},resource_id:{2}|{3}|", DateTime.Now, activity.apptNumber, activity.resourceId, result.Content), TypeLog.BAD_REST_ACTIVITY);

            return responseActivity;
        }

        public Activity Exist(Activity activity)
        {
            Program.Logger(string.Format("|{0}|apptNumber:{1}|resourceId:{2}|", DateTime.Now, activity.apptNumber, activity.resourceId), TypeLog.DEFAULT);
            Activity responseActivity = new Activity();
            string jsonActivy = JsonConvert.SerializeObject(activity, Formatting.None);

            ResponseOFSC result = UtilWebRequest.SendWayAsync("rest/ofscCore/v1/activities", enumMethod.POST, jsonActivy, Authorization);
            result.Content = result.Content.Replace("\n", string.Empty);

            if (result.statusCode >= 200 && result.statusCode < 300)
            {
                responseActivity = JsonConvert.DeserializeObject<Activity>(result.Content);
                Program.Logger(string.Format("|{0}|apptNumber:{1},activityId:{2},resource_id:{3}|{4}|", DateTime.Now, responseActivity.apptNumber, responseActivity.activityId, responseActivity.resourceId, result.Content), TypeLog.OK_REST_ACTIVITY);
            }

            else
                Program.Logger(string.Format("|{0}|apptNumber:{1},resource_id:{2}|{3}|", DateTime.Now, activity.apptNumber, activity.resourceId, result.Content), TypeLog.BAD_REST_ACTIVITY);

            return responseActivity;
        }
        public List<Activity> GetFromCSV(string path)
        {
            List<string> listCSVLines = ReadCSV(path);
            return FillList(listCSVLines);
        }

        private List<Activity> FillList(List<string> listCSVLines)
        {
            List<Activity> listActivity = new List<Activity>();

            for (int i = 0; i < listCSVLines.Count; i++)
            {
                if (i == 0)
                    continue;
                else
                {
                    try
                    {
                        string[] tmpExtractInfoforActivity = listCSVLines[i].Split(';');

                        //  For Activity required  minimun 72 array length

                        if (tmpExtractInfoforActivity.Length < 72)
                            throw new Exception("* La longitud minima es de 72 posiciones, linea en csv esta mal formada");

                        Activity activity = new Activity();
                        activity.apptNumber = tmpExtractInfoforActivity[0];
                        activity.date = tmpExtractInfoforActivity[1];
                        activity.resourceId = tmpExtractInfoforActivity[2];
                        activity.customerNumber = tmpExtractInfoforActivity[3];
                        activity.activityType = tmpExtractInfoforActivity[4];
                        activity.timeSlot = tmpExtractInfoforActivity[5];
                        activity.customerName = tmpExtractInfoforActivity[6];
                        activity.customerPhone = tmpExtractInfoforActivity[7];
                        activity.customerCell = tmpExtractInfoforActivity[8];
                        activity.customerEmail = tmpExtractInfoforActivity[9];
                        activity.streetAddress = tmpExtractInfoforActivity[10];
                        activity.city = tmpExtractInfoforActivity[11];
                        activity.country_code = tmpExtractInfoforActivity[12];
                        activity.longitude = string.IsNullOrEmpty(tmpExtractInfoforActivity[13]) ? 0 : float.Parse(tmpExtractInfoforActivity[13]);
                        activity.latitude = string.IsNullOrEmpty(tmpExtractInfoforActivity[14]) ? 0 : float.Parse(tmpExtractInfoforActivity[14]); ;
                        activity.slaWindowStart = string.IsNullOrEmpty(tmpExtractInfoforActivity[15]) ? string.Empty : tmpExtractInfoforActivity[15];
                        activity.slaWindowEnd = string.IsNullOrEmpty(tmpExtractInfoforActivity[16]) ? string.Empty : tmpExtractInfoforActivity[16];
                        activity.postalCode = tmpExtractInfoforActivity[17];
                        activity.stateProvince = tmpExtractInfoforActivity[18];
                        activity.points = string.IsNullOrEmpty(tmpExtractInfoforActivity[19]) ? 0 : int.Parse(tmpExtractInfoforActivity[19]);
                        activity.XA_FechaAtencionProgramada = tmpExtractInfoforActivity[20];
                        activity.XA_HoraAtencionProgramada = tmpExtractInfoforActivity[21];
                        activity.XA_Prioridad = tmpExtractInfoforActivity[22];
                        activity.XA_ClaveCapturaVenta = tmpExtractInfoforActivity[23];
                        activity.XA_EquipoTerminal = tmpExtractInfoforActivity[24];
                        activity.XA_FolioBt = tmpExtractInfoforActivity[25];
                        activity.XA_MedioEntregaModem = tmpExtractInfoforActivity[26];
                        activity.XA_Nis = tmpExtractInfoforActivity[27];
                        activity.XA_NumAsignado = tmpExtractInfoforActivity[28];
                        activity.XA_Paquete = tmpExtractInfoforActivity[29];
                        activity.XA_Promocion = tmpExtractInfoforActivity[30];
                        activity.XA_TipoIdentificación = tmpExtractInfoforActivity[31];
                        activity.XA_NumeroIdentificacion = tmpExtractInfoforActivity[32];
                        activity.XA_CanalVenta = tmpExtractInfoforActivity[33];
                        activity.XA_TipoCuenta = tmpExtractInfoforActivity[34];
                        activity.XA_SubtipoCliente = tmpExtractInfoforActivity[35];
                        activity.XA_RFC = tmpExtractInfoforActivity[36];
                        activity.XA_Contrato = tmpExtractInfoforActivity[37];
                        activity.XA_TipoVenta = tmpExtractInfoforActivity[38];
                        activity.XA_SubtipoVenta = tmpExtractInfoforActivity[39];
                        activity.XA_NumeroExterior = tmpExtractInfoforActivity[40];
                        activity.XA_NumeroInterior = tmpExtractInfoforActivity[41];
                        activity.XA_Colonia = tmpExtractInfoforActivity[42];
                        activity.XA_Municipio = tmpExtractInfoforActivity[43];
                        activity.XA_EntreCalle1 = tmpExtractInfoforActivity[44];
                        activity.XA_EntreCalle2 = tmpExtractInfoforActivity[45];
                        activity.XA_ComentariosDireccion = tmpExtractInfoforActivity[46];
                        activity.XA_CodAreaTel1 = tmpExtractInfoforActivity[47];
                        activity.XA_CodAreaTel2 = tmpExtractInfoforActivity[48];
                        activity.XA_Telefono2 = tmpExtractInfoforActivity[49];
                        activity.XA_ExtTel2 = tmpExtractInfoforActivity[50];
                        activity.XA_CodAreaTel3 = tmpExtractInfoforActivity[51];
                        activity.XA_Telefono3 = tmpExtractInfoforActivity[52];
                        activity.XA_ExtTel3 = tmpExtractInfoforActivity[53];
                        activity.XA_AreaServicio = tmpExtractInfoforActivity[54];
                        activity.XA_TipoServicio = tmpExtractInfoforActivity[55];
                        activity.XA_SubtipoServicio = tmpExtractInfoforActivity[56];
                        activity.XA_Zona = tmpExtractInfoforActivity[57];
                        activity.XA_IDLlamada = tmpExtractInfoforActivity[58];
                        activity.XA_LlamadaEspera = tmpExtractInfoforActivity[59];
                        activity.XA_Tripartita = tmpExtractInfoforActivity[60];
                        activity.XA_BuzonVoz = tmpExtractInfoforActivity[61];
                        activity.XA_Sigueme = tmpExtractInfoforActivity[62];
                        activity.XA_OrdenServicio = tmpExtractInfoforActivity[63];
                        activity.XA_FolioSV = tmpExtractInfoforActivity[64];
                        activity.XA_saldo_cuenta = tmpExtractInfoforActivity[65];
                        activity.XA_saldo_adeudado = tmpExtractInfoforActivity[66];
                        activity.XA_saldo_penalizado = tmpExtractInfoforActivity[67];
                        activity.XA_tipo_de_plazo = tmpExtractInfoforActivity[68];
                        // activity.XA_tipo_instalacion = tmpExtractInfoforActivity[69];
                        activity.XA_id_instalador = tmpExtractInfoforActivity[70];
                        activity.XA_mov_tipo_eq = tmpExtractInfoforActivity[71];
                        activity.XA_red = tmpExtractInfoforActivity[72];
                        listActivity.Add(activity);

                        // Check if has inventory
                        if (tmpExtractInfoforActivity.Length == 80)
                        {

                        }
                    }
                    catch (Exception ex)
                    {
                        Program.Logger(string.Format("|{0}|*Error al llenar el objecto Actividad,{1}|{2}|", DateTime.Now, ex.Message, listCSVLines[i]), TypeLog.BAD_IO_ACTIVITY);
                    }
                }
            }
            return listActivity;
        }

        private List<string> ReadCSV(string path)
        {
            this.source = path;
            List<string> list = new List<string>();
            var files = new FileInfo(path);
            Task<List<string>> task = this.LinesFileAsync();
            task.Wait();
            var result = task.Result;
            list.AddRange(result);
            return list;
        }

        private async Task<List<string>> LinesFileAsync()
        {
            List<string> listResult = new List<string>();

            using (var reader = new StreamReader(this.source, Encoding.Default, true))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    listResult.Add(line);
                }
            }
            return listResult;
        }
    }
}
