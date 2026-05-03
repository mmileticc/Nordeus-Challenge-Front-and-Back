using System.Collections.Generic;

namespace GauntletOfTheHero.Gameplay.Models
{
    [System.Serializable]
    public class HeroModel
    {
        public long id;
        public string name;
        public CharacterStatsModel stats;
        public List<MoveModel> moves = new List<MoveModel>();
    }
}