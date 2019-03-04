using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skymigration
{
    public class Inventory
    {
        // 1 inventoryType
        // 1 serialNumber
        // 1 XI_TI
        // 1 XI_jerarquia
        // 1 XI_estatus
        // 1 quantity
        // XI_Marca
        // XI_Modelo
        public int inventoryId { get; set; }
        public string status { get; set; }
        public string inventoryType { get; set; }
        public string serialNumber { get; set; }
        public int quantity { get; set; }
        public int activityId { get; set; }
        public string XI_TI { get; set; }
        public string XI_jerarquia { get; set; }
        public string XI_estatus { get; set; }
        public string XI_Marca { get; set; }
        public string XI_Modelo { get; set; }

    }
}
