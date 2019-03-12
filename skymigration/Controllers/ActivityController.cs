using System.Threading;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Globalization;

namespace skymigration
{
    public class ActivityController : IActivity
    {
        public string Authorization { get; set; } = ConfigurationManager.AppSettings["currentenviroment"].ToString();
        private string source { get; set; }
        private int attemps { get; set; } = 0;

        private void AnalyzingBulk(RootActivityBulk resultbulkactivity, RootActivity activities)
        {

            foreach (var item in resultbulkactivity.results)
            {
                try
                {
                    LayoutOutput layoutOutput = new LayoutOutput();
                    layoutOutput.activityId = item.activityKeys.activityId;
                    layoutOutput.apptNumber = item.activityKeys.apptNumber;
                    var tmpactivity = activities.activities.FirstOrDefault(x => x.apptNumber == item.activityKeys.apptNumber && x.customerNumber == item.activityKeys.customerNumber);
                    layoutOutput.resourceId = tmpactivity.resourceId;

                    if (string.IsNullOrEmpty(tmpactivity.date))
                        tmpactivity.date = tmpactivity.date;
                    else
                    {
                        DateTime tmpdateTime;
                        DateTime.TryParse(tmpactivity.date, out tmpdateTime);

                        if (tmpdateTime.Year != 1)
                            tmpactivity.date = tmpdateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    layoutOutput.date = tmpactivity.date;
                    layoutOutput.timeSlot = tmpactivity.timeSlot;
                    string horarioAgendado = string.IsNullOrEmpty(layoutOutput.timeSlot) ? string.Empty : Program.DictionaryTimeSlot[tmpactivity.timeSlot];
                    layoutOutput.horarioAgendado = horarioAgendado;
                    string resultInventory = string.Empty;

                    foreach (var itemInventory in tmpactivity.inventories.items)
                        resultInventory += itemInventory.inventoryType + ":" + itemInventory.status + "|";

                    layoutOutput.result = item.operationsPerformed == null ? "operationsFailed " + item.errors[0].errorDetail : item.operationsPerformed.FirstOrDefault();

                    // layoutOutput.result = item.operationsFailed
                    // (item.operationsFailed).Items[0]
                    // inventories
                    if (item.operationsFailed != null)
                    {
                        if (item.operationsFailed[0] == "updateInventories")
                            layoutOutput.result = layoutOutput.result + "|" + item.errors[0].errorDetail;
                    }


                    // layoutOutput.description = "Appointment id = " + layoutOutput.activityId;
                    string lineCSV = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};",
                                                   layoutOutput.activityId,
                                                   layoutOutput.apptNumber,
                                                   layoutOutput.typeCommand,
                                                   layoutOutput.resourceId,
                                                   layoutOutput.date,
                                                   layoutOutput.timeSlot,
                                                   layoutOutput.horarioAgendado,
                                                   layoutOutput.result,
                                                   layoutOutput.typeMessage,
                                                   layoutOutput.cod,
                                                   layoutOutput.description);

                    Program.LoggerCSV(lineCSV);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        public RootActivityBulk CreateBulk(RootActivity activities)
        {
            RootActivityBulk resultbulkactivity = new RootActivityBulk();
            string bulkactivity = JsonConvert.SerializeObject(activities, Formatting.None);

            ResponseOFSC result = UtilWebRequest.SendWayAsync("rest/ofscCore/v1/activities/custom-actions/bulkUpdate", enumMethod.POST, bulkactivity, Authorization);
            result.Content = result.Content.Replace("\n", string.Empty);

            // REQUEST OK
            if (result.statusCode >= 200 && result.statusCode < 300)
            {
                attemps += 0;
                resultbulkactivity = JsonConvert.DeserializeObject<RootActivityBulk>(result.Content);
                Program.Logger(string.Format("|{0}|statusCode:{1}|Content:{2}|ErrorMessage:{3}|activities:{4}|", DateTime.Now, result.statusCode, result.Content, result.ErrorMessage, JsonConvert.SerializeObject(activities.activities, Formatting.None)), TypeLog.OK_REST_ACTIVITY);

                // Analyzing object
                AnalyzingBulk(resultbulkactivity, activities);
            }

            // REQUEST NETWORK CONNECTION BEING DOWN 
            else if (result.statusCode == 0 && attemps < 3)
            {
                attemps += 1;
                Program.Logger(string.Format("|{0}|statusCode:{1}|Content:{2}|ErrorMessage:{3}|activities{4}|", DateTime.Now, result.statusCode, result.Content, result.ErrorMessage, bulkactivity), TypeLog.NSHTTPURLResponse);
                Thread.Sleep(5000);
                CreateBulk(activities);
            }

            // REQUEST BAD
            //else 
            //{
            //    Program.Logger(string.Format("|{0}|activities:{1}|Content:{2}|", DateTime.Now, bulkactivity, result.Content), TypeLog.BAD_REST_ACTIVITY);
            //}
            return resultbulkactivity;
        }
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



                        activity.apptNumber = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[0]);
                        activity.date = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[1]);

                        activity.resourceId = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[2]);
                        activity.resourceId = string.IsNullOrEmpty(activity.resourceId) ? activity.resourceId : ConfigurationManager.AppSettings["resourceiddefault"].ToString();

                        //activity.inventories = new Inventories();
                        //activity.inventories.items = new List<Item>();
                        // TODO 
                        // ERASE BELOW LINE
                        activity.resourceId = "ESTEBANTEC2018";
                        activity.customerNumber = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[3]);
                        activity.activityType = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[4]);
                        activity.timeSlot = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[5]);
                        activity.customerName = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[6]);
                        activity.customerPhone = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[7]);
                        activity.customerCell = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[8]);
                        activity.customerEmail = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[9]);
                        activity.streetAddress = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[10]);
                        activity.city = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[11]);
                        activity.country_code = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[12]);
                        activity.longitude = string.IsNullOrEmpty(tmpExtractInfoforActivity[13]) ? 0 : float.Parse(tmpExtractInfoforActivity[13]);
                        activity.latitude = string.IsNullOrEmpty(tmpExtractInfoforActivity[14]) ? 0 : float.Parse(tmpExtractInfoforActivity[14]);
                        activity.slaWindowStart = tmpExtractInfoforActivity[15];
                        activity.slaWindowEnd = tmpExtractInfoforActivity[16];
                        activity.postalCode = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[17]);
                        activity.stateProvince = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[18]);
                        int tmppoints;
                        if (int.TryParse(tmpExtractInfoforActivity[19], out tmppoints))
                            activity.points = tmppoints;
                        else
                            activity.points = null;

                        activity.XA_FechaAtencionProgramada = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[20]);
                        activity.XA_HoraAtencionProgramada = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[21]);
                        activity.XA_Prioridad = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[22]);
                        activity.XA_ClaveCapturaVenta = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[23]);
                        activity.XA_EquipoTerminal = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[24]);
                        activity.XA_FolioBt = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[25]);
                        activity.XA_MedioEntregaModem = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[26]);
                        activity.XA_Nis = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[27]);
                        activity.XA_NumAsignado = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[28]);
                        activity.XA_Paquete = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[29]);
                        activity.XA_Promocion = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[30]);
                        activity.XA_TipoIdentificación = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[31]);
                        activity.XA_NumeroIdentificacion = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[32]);
                        activity.XA_CanalVenta = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[33]);
                        activity.XA_TipoCuenta = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[34]);
                        activity.XA_SubtipoCliente = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[35]);
                        activity.XA_RFC = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[36]);
                        activity.XA_Contrato = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[37]);
                        activity.XA_TipoVenta = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[38]);
                        activity.XA_SubtipoVenta = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[39]);
                        activity.XA_NumeroExterior = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[40]);
                        activity.XA_NumeroInterior = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[41]);
                        activity.XA_Colonia = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[42]);
                        activity.XA_Municipio = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[43]);
                        activity.XA_EntreCalle1 = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[44]);
                        activity.XA_EntreCalle2 = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[45]);
                        activity.XA_ComentariosDireccion = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[46]);
                        activity.XA_CodAreaTel1 = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[47]);
                        activity.XA_CodAreaTel2 = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[48]);
                        activity.XA_Telefono2 = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[49]);
                        activity.XA_ExtTel2 = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[50]);
                        activity.XA_CodAreaTel3 = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[51]);
                        activity.XA_Telefono3 = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[52]);
                        activity.XA_ExtTel3 = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[53]);
                        activity.XA_AreaServicio = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[54]);
                        activity.XA_TipoServicio = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[55]);
                        activity.XA_SubtipoServicio = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[56]);
                        activity.XA_Zona = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[57]);
                        activity.XA_IDLlamada = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[58]);
                        activity.XA_LlamadaEspera = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[59]);
                        activity.XA_Tripartita = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[60]);
                        activity.XA_BuzonVoz = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[61]);
                        activity.XA_Sigueme = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[62]);
                        activity.XA_OrdenServicio = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[63]);
                        activity.XA_FolioSV = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[64]);
                        activity.XA_saldo_cuenta = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[65]);
                        activity.XA_saldo_adeudado = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[66]);
                        activity.XA_saldo_penalizado = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[67]);
                        activity.XA_tipo_de_plazo = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[68]);
                        // TODO
                        // ANALIZAR ESTA PROPIEDAD CON CHRISTIAN
                        // activity.XA_tipo_instalacion = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[69]);
                        activity.XA_id_instalador = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[70]);
                        activity.XA_mov_tipo_eq = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[71]);
                        activity.XA_red = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[72]);

                        // 
                        Activity getexistactivity = new Activity();
                        if (tmpExtractInfoforActivity.Length >= 80)
                        {
                            getexistactivity = listActivity.FirstOrDefault(x => x.apptNumber == activity.apptNumber);
                            activity.inventories = new Inventories();
                            activity.inventories.items = new List<Item>();

                            Item item = new Item();
                            item.inventoryType = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[73]);
                            item.serialNumber = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[74]);
                            item.XI_TI = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[75]);
                            item.XI_jerarquia = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[76]);
                            item.XI_estatus = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[77]);
                            item.quantity = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[78]) == null ? 0 : int.Parse(tmpExtractInfoforActivity[78]);
                            item.XI_Marca = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[79]);
                            item.XI_Modelo = UtilWebRequest.IsNullOrEmpty(tmpExtractInfoforActivity[80]);

                            if (getexistactivity != null)
                                getexistactivity.inventories.items.Add(item);
                            else
                                activity.inventories.items.Add(item);
                        }

                        if (getexistactivity == null)
                            listActivity.Add(activity);

                        else
                        {
                            var itemToRemove = listActivity.FirstOrDefault(x => x.apptNumber == activity.apptNumber);
                            listActivity.Remove(itemToRemove);
                            listActivity.Add(getexistactivity);
                        }

                    }
                    catch (Exception ex)
                    {
                        Program.Logger(string.Format("|{0}|*Error al llenar el objecto Actividad,{1}|{2}|", DateTime.Now, ex.Message, listCSVLines[i]), TypeLog.BAD_IO_ACTIVITY);
                    }
                }
            }
            // TODO
            // pruebas de stress
            //for (int i = 0; i < 1000000; i++)
            //{
            //    Activity activity = new Activity();
            //    activity.activityId = listActivity[0].activityId;
            //    activity.apptNumber = Guid.NewGuid().ToString();
            //    activity.date =  listActivity[0].date;
            //    activity.resourceId = listActivity[0].resourceId;
            //    activity.customerNumber = listActivity[0].customerNumber;
            //    activity.activityType = listActivity[0].activityType;
            //    activity.timeSlot = listActivity[0].timeSlot;
            //    activity.customerName = listActivity[0].customerName;
            //    activity.customerPhone = listActivity[0].customerPhone;
            //    activity.customerCell = listActivity[0].customerCell;
            //    activity.customerEmail = listActivity[0].customerEmail;
            //    activity.streetAddress = listActivity[0].streetAddress;
            //    activity.city = listActivity[0].city;
            //    activity.country_code = listActivity[0].country_code;
            //    activity.longitude = listActivity[0].longitude;
            //    activity.latitude = listActivity[0].latitude;
            //    activity.slaWindowStart = listActivity[0].slaWindowStart;
            //    activity.slaWindowEnd = listActivity[0].slaWindowEnd;
            //    activity.postalCode = listActivity[0].postalCode;
            //    activity.stateProvince = listActivity[0].stateProvince;
            //    activity.points = listActivity[0].points;
            //    listActivity.Add(activity);
            //}

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

            using (var reader = new StreamReader(this.source, Encoding.UTF8, true))
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
