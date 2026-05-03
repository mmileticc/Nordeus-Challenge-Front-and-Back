using GauntletOfTheHero.Backend.Models;

namespace GauntletOfTheHero.Gameplay.Models
{
    [System.Serializable]
    public class ActiveBuffModel
    {
        public CharacterSide targetSide;
        public BuffAttribute attribute;
        public int amount;
        public int remainingTurns;
    }
}