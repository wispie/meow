using System.Collections.Generic;

namespace Grimoire.Game
{
    public class BotData
    {
        public enum State
        {
            Others,
            Transaction,
            Move,
            Combat,
            Rest,
            Quest
        }

        public List<string> DropList = new List<string>();

        public string BotMap
        {
            get;
            set;
        } = null;

        public string BotCell
        {
            get;
            set;
        } = null;

        public string BotPad
        {
            get;
            set;
        } = null;

        public State BotState
        {
            get;
            set;
        } = State.Others;

        public string BotSkill
        {
            get;
            set;
        } = null;

        public Dictionary<string, int> SkillSet
        {
            get;
            set;
        } = new Dictionary<string, int>();
    }
}