using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skymigration
{
    class InventoryController : IInventory
    {
        private string Authorization { get; set; } = ConfigurationManager.AppSettings["currentenviroment"].ToString();

        public Inventory CreateInventory(Inventory inventory)
        {
            Inventory responseInventory = new Inventory();

            string search = string.Format("rest/ofscCore/v1/activities/{0}/customerInventories", inventory.activityId);
            ResponseOFSC result = UtilWebRequest.SendWayAsync(search, enumMethod.POST,
                                                              JsonConvert.SerializeObject(inventory, Formatting.None),
                                                              Authorization);
            result.Content = result.Content.Replace("\n", string.Empty);

            if (result.statusCode >= 200 && result.statusCode < 300)
            {
                responseInventory = JsonConvert.DeserializeObject<Inventory>(result.Content);
                Program.Logger(string.Format("|{0}|activityId:{1},inventoryId|{2}|{3}", DateTime.Now, responseInventory.activityId, responseInventory.inventoryId, result.Content), TypeLog.OK_REST_ACTIVITY);
            }

            else
                Program.Logger(string.Format("|{0}|activityId:{1},inventoryType|{2}|{3}", DateTime.Now, inventory.activityId, inventory.inventoryType, result.Content), TypeLog.BAD_REST_INVENTORY);

            return responseInventory;
        }
    }
}
