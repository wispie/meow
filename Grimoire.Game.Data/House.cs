using Grimoire.Networking;
using Grimoire.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Grimoire.Game.Data
{
    public class House
    { 
        private Flash flash;
        public House(Flash newFlash)
        {
            flash = newFlash;
        }
        public List<InventoryItem> Items => flash.Call<List<InventoryItem>>("GetHouseItems", new string[0]);

        public int TotalSlots => flash.Call<int>("HouseSlots", new string[0]);

        public bool ContainsItem(string itemName, string quantity = "*")
        {
            InventoryItem inventoryItem = Items.FirstOrDefault((InventoryItem i) => i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));
            if (inventoryItem != null)
            {
                if (!(quantity == "*"))
                {
                    return inventoryItem.Quantity >= int.Parse(quantity);
                }
                return true;
            }
            return false;
        }
    }
}