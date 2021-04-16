using Grimoire.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Grimoire.Game.Data
{
    public class Quests
    {
        private Flash flash;
        public Quests(Flash newFlash)
        {
            flash = newFlash;
        }

        public List<Quest> QuestTree => flash.Call<List<Quest>>("GetQuestTree", new string[0]);

        public List<Quest> AcceptedQuests => QuestTree.Where((Quest q) => q.IsInProgress).ToList();

        public List<Quest> UnacceptedQuests => QuestTree.Where((Quest q) => !q.IsInProgress).ToList();

        public List<Quest> CompletedQuests => QuestTree.Where((Quest q) => q.CanComplete).ToList();

        public event Action<List<Quest>> QuestsLoaded;

        public event Action<CompletedQuest> QuestCompleted;

        public void OnQuestsLoaded(List<Quest> quests) => this.QuestsLoaded?.Invoke(quests);

        public void OnQuestCompleted(CompletedQuest quest) => this.QuestCompleted?.Invoke(quest);

        public void Accept(int questId) => flash.Call("Accept", questId.ToString());

        public void Accept(string questId) => flash.Call("Accept", questId);

        public void Complete(int questId) => flash.Call("Complete", questId.ToString());

        public void Complete(string questId) => flash.Call("Complete", questId);

        public void Complete(string questId, string itemId) => flash.Call("Complete", itemId, bool.TrueString);

        public void Load(int id) => flash.Call("LoadQuest", id.ToString());

        public void Load(List<int> ids) => flash.Call("LoadQuests", string.Join(",", ids));
        
        public void Get(List<int> ids) => flash.Call("GetQuests", string.Join(",", ids.Select(delegate (int i) { return i.ToString(); })));

        public bool IsInProgress(int id) => flash.Call<bool>("IsInProgress", id.ToString());

        public bool IsInProgress(string id) => flash.Call<bool>("IsInProgress", id);

        public bool CanComplete(int id) => flash.Call<bool>("CanComplete", id.ToString());

        public bool CanComplete(string id) => flash.Call<bool>("CanComplete", id);

        public bool IsAvailable(int id) => flash.Call<bool>("IsAvailable", id.ToString());
    }
}