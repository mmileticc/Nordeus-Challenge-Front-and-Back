using GauntletOfTheHero.Backend.Models;

namespace GauntletOfTheHero.Gameplay.Models
{
    [System.Serializable]
    public class MoveModel
    {
        public long id;
        public string name;
        public MoveType moveType;
        public EffectType effectType;
        public int power;
        public BuffAttribute? buffAttribute;
        public int buffAmount;
        public int buffDuration;
    }
}