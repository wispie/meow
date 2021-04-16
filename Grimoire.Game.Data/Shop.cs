using Grimoire.Tools;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Grimoire.Game.Data
{
    public class Shop
    {
        private Flash flash;
        public Shop(Flash newFlash)
        {
            flash = newFlash;
        }

        [JsonProperty("sName")]
        public string Name
        {
            get;
            set;
        }

        [JsonProperty("ShopID")]
        public int Id
        {
            get;
            set;
        }

        [JsonProperty("items")]
        public List<InventoryItem> Items
        {
            get;
            set;
        }

        public string Location
        {
            get;
            set;
        }

        public bool IsShopLoaded => flash.Call<bool>("IsShopLoaded", new string[0]);

        public void BuyItem(string name)
        {
            flash.Call("BuyItem", name);
        }

        public void ResetShopInfo()
        {
            flash.Call("ResetShopInfo", new string[0]);
        }

        public void Load(int id)
        {
            flash.Call("LoadShop", id.ToString());
        }

        public void SellItem(string name)
        {
            flash.Call("SellItem", name);
        }

        public void LoadHairShop(string id)
        {
            flash.Call("LoadHairShop", id);
        }

        public void LoadHairShop(int id)
        {
            flash.Call("LoadHairShop", id.ToString());
        }

        public void LoadArmorCustomizer()
        {
            flash.Call("LoadArmorCustomizer", new string[0]);
        }
    }
}