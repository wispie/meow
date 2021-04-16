using Grimoire.Game.Data;
using Grimoire.Tools;
using Grimoire.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Grimoire.Game
{
    public class World
    {
        private Flash flash;
        private Player player;
        private LogForm logForm;
        public World(Flash newFlash, Player newPlayer, LogForm newLogForm)
        {
            logForm = newLogForm;
            flash = newFlash;
            player = newPlayer;
        }

        public void RefreshDictionary() => _players = JsonConvert.DeserializeObject<Dictionary<string, PlayerInfo>>(flash.Call("Players", new object[0]));

        private Dictionary<string, PlayerInfo> _players; 
        /// <summary>
        /// Gets a list of all players in the current map.
        /// </summary>
        public List<PlayerInfo> Players => _players.Values.ToList();

        public List<ShopInfo> LoadedShops;

        public DropStack DropStack;

        private readonly Dictionary<LockActions, string> LockedActions;

        public List<Monster> VisibleMonsters;

        public List<Monster> AvailableMonsters => flash.Call<List<Monster>>("GetMonstersInCell", new string[0]);

        public bool IsMapLoading => !flash.Call<bool>("MapLoadComplete", new string[0]);

        public List<string> PlayersInMap => flash.Call<List<string>>("PlayersInMap", new string[0]);

        public List<InventoryItem> ItemTree => flash.Call<List<InventoryItem>>("GetItemTree", new string[0]);

        public string[] Cells => flash.Call<string[]>("GetCells", new string[0]);

        public int RoomId => flash.Call<int>("RoomId", new string[0]);

        public int RoomNumber => flash.Call<int>("RoomNumber", new string[0]);

        public event Action<InventoryItem> ItemDropped;

        public event Action<ShopInfo> ShopLoaded;

        public void OnItemDropped(InventoryItem drop)
        {
            Action<InventoryItem> itemDropped = ItemDropped;
            if (itemDropped != null)
            {
                string text = $"{(player.Inventory.Items.Find((InventoryItem x) => x.Name == drop.Name) ?? new InventoryItem(this)).Quantity}";
                logForm.AppendDrops($"[Item Drop] {drop.Quantity} {drop.Name} at {DateTime.Now:hh:mm:ss tt} [{text}] \r\n");
                itemDropped(drop);
            }
        }

        public void OnShopLoaded(ShopInfo shopInfo)
        {
            ShopLoaded?.Invoke(shopInfo);
            LoadedShops.Add(shopInfo);
        }

        public bool IsActionAvailable(LockActions action) => flash.Call<bool>("IsActionAvailable", LockedActions[action]);

        public void SetSpawnPoint() => flash.Call("SetSpawnPoint", new string[0]);

        public bool IsMonsterAvailable(string name) => flash.Call<bool>("IsMonsterAvailable", new string[1]{name});

        World()
        {
            LoadedShops = new List<ShopInfo>();
            DropStack = new DropStack();
            LockedActions = new Dictionary<LockActions, string>(14)
            {
                {
                    LockActions.LoadShop,
                    "loadShop"
                },
                {
                    LockActions.LoadEnhShop,
                    "loadEnhShop"
                },
                {
                    LockActions.LoadHairShop,
                    "loadHairShop"
                },
                {
                    LockActions.EquipItem,
                    "equipItem"
                },
                {
                    LockActions.UnequipItem,
                    "unequipItem"
                },
                {
                    LockActions.BuyItem,
                    "buyItem"
                },
                {
                    LockActions.SellItem,
                    "sellItem"
                },
                {
                    LockActions.GetMapItem,
                    "getMapItem"
                },
                {
                    LockActions.TryQuestComplete,
                    "tryQuestComplete"
                },
                {
                    LockActions.AcceptQuest,
                    "acceptQuest"
                },
                {
                    LockActions.DoIA,
                    "doIA"
                },
                {
                    LockActions.Rest,
                    "rest"
                },
                {
                    LockActions.Who,
                    "who"
                },
                {
                    LockActions.Transfer,
                    "tfer"
                }
            };
            VisibleMonsters = flash.Call<List<Monster>>("GetVisibleMonstersInCell", new string[0]);
        }
    }
}