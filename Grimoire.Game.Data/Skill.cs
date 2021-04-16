using Grimoire.Tools;

namespace Grimoire.Game.Data
{
    public class Skill
    {
        private Flash flash;
        public Skill(Flash newFlash)
        {
            flash = newFlash;
        }

        public enum SkillType
        {
            Normal,
            Safe,
            Label
        }

        public string Text
        {
            get;
            set;
        }

        public SkillType Type
        {
            get;
            set;
        }

        public string Index
        {
            get;
            set;
        }

        public int SafeHealth
        {
            get;
            set;
        }

        public bool SafeMp
        {
            get;
            set;
        }

        public string GetSkillName(string index)
        {
            return flash.Call<string>("GetSkillName", new string[1]
            {
                index
            });
        }
    }
}