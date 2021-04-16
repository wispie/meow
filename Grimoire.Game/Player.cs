using Grimoire.Botting;
using Grimoire.Botting.Commands.Map;
using Grimoire.Game.Data;
using Grimoire.Tools;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Grimoire.Game
{
    public class Player
    {
        private Flash flash;
        public Player(Flash newFlash)
        {
            flash = newFlash;
        }

        public enum State
        {
            Dead,
            Idle,
            InCombat
        }

        public int UserID => flash.Call<int>("UserID", new object[0]);

        public Bank Bank
        {
            get;
        }

        public Inventory Inventory
        {
            get;
        }

        public TempInventory TempInventory
        {
            get;
        }

        public House House
        {
            get;
        }

        /// <summary>
        /// Gets an array containing all the names of the factions that the player has some reputation in.
        /// </summary>
        public List<Faction> Factions => JsonConvert.DeserializeObject<List<Faction>>(flash.Call("GetFactions", new object[0]));

        public Quests Quests
        {
            get;
        }

        public CmdTravel CreateJoinCommand(string map, string cell = "Enter", string pad = "Spawn")
        {
            return new CmdTravel
            {
                Map = map,
                Cell = cell,
                Pad = pad
            };
        }

        public async void ExecuteTravel(List<IBotCommand> cmds)
        {
            foreach (IBotCommand cmd in cmds)
            {
                await cmd.Execute(null);
                await Task.Delay(1000);
            }
        }

        private bool usernamecheck = true;
        /// <summary>
        /// Grabs Username string.
        /// </summary>
        public string Username
        {
            get
            {
                if (!IsLoggedIn)
                    return "";
                string usr = flash.Call<string>("GetUsername", new string[0]);
                if (usr == "null" && usernamecheck)
                {
                    MessageBox.Show(
                        $"Your Username is currently null meaning some bots that utilize your username (like packet bots)\r\n" +
                        $" will not work correctly, to refresh it try restarting grimoire"
                        );
                    usernamecheck = false;
                }
                return usr;
            }
        }

        /// <summary>
        /// Grabs Password string.
        /// </summary>
        public string Password => flash.Call<string>("GetPassword", new string[0]);

        /// <summary>
        /// Checks if Logged in.
        /// </summary>
        public bool IsLoggedIn => flash.Call<bool>("IsLoggedIn", new string[0]);

        /// <summary>
        /// Gets Cell.
        /// </summary>
        public string Cell => flash.Call<string>("Cell", new string[0]);

        /// <summary>
        /// Gets Pad.
        /// </summary>
        public string Pad => flash.Call<string>("Pad", new string[0]);

        /// <summary>
        /// Gets Current State (Dead, Idle or InCombat).
        /// </summary>
        public State CurrentState => (State)flash.Call<int>("State", new string[0]);

        /// <summary>
        /// Gets Current Health.
        /// </summary>
        public int Health => flash.Call<int>("Health", new string[0]);

        /// <summary>
        /// Gets Health max.
        /// </summary>
        public int HealthMax => flash.Call<int>("HealthMax", new string[0]);

        /// <summary>
        /// Checks if Health is above 0.
        /// </summary>
        public bool IsAlive => Health > 0;

        /// <summary>
        /// Gets Current Mana.
        /// </summary>
        public int Mana => flash.Call<int>("Mana", new string[0]);

        /// <summary>
        /// Gets Mana max.
        /// </summary>
        public int ManaMax => flash.Call<int>("ManaMax", new string[0]);

        /// <summary>
        /// Gets map string.
        /// </summary>
        public string Map => flash.Call<string>("Map", new string[0]);

        /// <summary>
        /// Gets level int.
        /// </summary>
        public int Level => flash.Call<int>("Level", new string[0]);

        /// <summary>
        /// Gets gold int.
        /// </summary>
        public int Gold => flash.Call<int>("Gold", new string[0]);

        /// <summary>
        /// Checks if the player has a target.
        /// </summary>
        public bool HasTarget => flash.Call<bool>("HasTarget", new string[0]);

        /// <summary>
        /// Checks if all skills are available or off cooldown.
        /// </summary>
        public int AllSkillsAvailable => flash.Call<int>("AllSkillsAvailable", new string[0]);

        /// <summary>
        /// Checks if the player is afk.
        /// </summary>
        public bool IsAfk => flash.Call<bool>("IsAfk", new string[0]);

        /// <summary>
        /// Finds player position (float).
        /// </summary>
        public float[] Position => flash.Call<float[]>("Position", new string[0]);

        /// <summary>
        /// Checks if the player is a member (upgrade).
        /// </summary>
        public bool IsMember => bool.Parse(flash.Call<string>("IsMember", new string[0]));
            //flash.Instance.GetGameObject<int>("world.myAvatar.objData.iUpgDays") >= 0;

        /// <summary>
        /// Checks if int skill is available (i think if its also off cooldown).
        /// </summary>
        /// <param name="index"></param>
        public int SkillAvailable(string index) => flash.Call<int>("SkillAvailable", new string[1]{index});

        /// <summary>
        ///  Toggles mute.
        /// </summary>
        public void ToggleMute(bool b) => flash.Call("MuteToggle", b);

        /// <summary>
        /// Change between AccessLevels (Non Member, Member, Moderator).
        /// </summary>
        /// <param name="level"></param>
        public void ChangeAccessLevel(string level) => flash.Call("ChangeAccessLevel", level);

        public void WalkToPoint(string x, string y) => flash.Call("WalkToPoint", x, y);

        /// <summary>
        /// Cancels Target
        /// </summary>
        public void CancelTarget() => flash.Call("CancelTarget", new string[0]);

        /// <summary>
        /// Cancels Target on Self
        /// </summary>
        public void CancelTargetSelf() => flash.Call("CancelTargetSelf", new string[0]);

        /// <summary>
        /// Haste set to 50%, Mana refreshes to 100
        /// </summary>
        public void SetBuff() => flash.Call("Buff", new string[0]);

        /// <summary>
        /// Attacks Monster
        /// </summary>
        /// <param name="name"></param>
        public void AttackMonster(string name) => flash.Call("AttackMonster", name);

        /// <summary>
        /// Sets Respawn Point to Current Cell Pad
        /// </summary>
        public void SetSpawnPoint() => flash.Call("SetSpawnPoint", new string[0]);

        public void MoveToCell(string cell, string pad = "Spawn") => flash.Call("Jump", cell, pad);

        public void Rest() => flash.Call("Rest", new string[0]);

        public void JoinMap(string map, string cell = "Enter", string pad = "Spawn") => flash.Call("Join", map, cell, pad);

        public void Equip(string id) => flash.Call("Equip", id);
        public void Equip(int id) => flash.Call("Equip", id.ToString());

        public void EquipPotion(int id, string desc, string file, string name) => flash.Call("EquipPotion", id.ToString(), desc, file, name);

        //public void GotoPlayer(string name) => flash.Call("GoTo", name);
        public void GoToPlayer(string name) => flash.Call("GoTo", name);

        public bool HasActiveBoost(string name) => flash.Call<bool>("HasActiveBoost", new string[1]{name});

        public void UseBoost(string id) => flash.Call("UseBoost", id);

        public void UseBoost(int id) => flash.Call("UseBoost", id.ToString());

        public void UseSkill(string index) => flash.Call("UseSkill", index);

        public void GetMapItem(string id) => flash.Call("GetMapItem", id);

        public void GetMapItem(int id) => flash.Call("GetMapItem", id.ToString());

        public void Logout() => flash.Call("Logout", new string[0]);

        public Player()
        {
            Bank = new Bank(flash);
            Inventory = new Inventory(flash);
            TempInventory = new TempInventory(flash);
            House = new House(flash);
            Quests = new Quests(flash);
        }
    }
}