using System;
using System.Collections.Generic;

namespace GauntletOfTheHero.Gameplay.Models
{
    [System.Serializable]
    public class BattleSnapshotModel
    {
        public Guid sessionId;
        public HeroModel hero;
        public MonsterModel monster;
        public List<ActiveBuffModel> activeBuffs = new List<ActiveBuffModel>();
        public List<string> logs = new List<string>();
        public string heroMoveName;
        public string monsterMoveName;
        public bool battleFinished;
        public string winner;
    }
}