namespace Cascade.Core.Players
{
    public class HoneyCombPlayer : ModPlayer
    {
        public int beeFlight = 0;
        public override void UpdateEquips()
        {
            
            switch(beeFlight){
                case 1:
                    Player.wingTimeMax = (int)(Player.wingTimeMax * 1.1);
                    break;
                case 2:
                    Player.wingTimeMax = (int)(Player.wingTimeMax * 1.12);
                    break;
                case 3:
                    Player.wingTimeMax = (int)(Player.wingTimeMax * 1.07);
                    break;
                default:
                    beeFlight = 0;
                    break;
            }
            
        }

        public override void ResetEffects()
        {
            beeFlight = 0;
        }

        public override void UpdateDead()
        {
            beeFlight = 0;
        }

    }

}