namespace GauntletOfTheHero.Gameplay.Models
{
    [System.Serializable]
    public class MonsterModel
    {
        public long id;
        public string name;
        public int difficultyRank;
        public CharacterStatsModel stats;
    }
}