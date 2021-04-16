using Grimoire.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Grimoire.Game.Data
{
    public class Bank
    {
        private Flash flash;
        public Bank(Flash newFlash)
        {
            flash = newFlash;
        }

        public List<InventoryItem> Items => flash.Call<List<InventoryItem>>("GetBankItems", new string[0]);

        public int AvailableSlots => TotalSlots - UsedSlots;

        public int UsedSlots => flash.Call<int>("UsedBankSlots", new string[0]);

        public int TotalSlots => flash.Call<int>("BankSlots", new string[0]);

        public void TransferToBank(string itemName) => flash.Call("TransferToBank", itemName);

        public void TransferFromBank(string itemName) => flash.Call("TransferToInventory", itemName);

        public void Swap(string invItemName, string bankItemName) => flash.Call("BankSwap", invItemName, bankItemName);

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

        public void Show()
        {
            flash.Call("ShowBank", new string[0]);
        }

        public void LoadItems()
        {
            flash.Call("LoadBankItems", new string[0]);
        }
    }
}