using Terraria.GameContent.Events;
using Terraria.ModLoader.IO;

namespace Cascade.Core.Systems
{
    public class AmbientEventHandler : ModSystem
    {
        public static bool CometNight = false;

        public override void OnWorldLoad()
        {
            CometNight = false;
        }

        public override void OnWorldUnload()
        {
            CometNight = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag.Add("CometNight", CometNight);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            CometNight = tag.ContainsKey("CometNight");
        }

        public override void NetSend(BinaryWriter writer)
        {
            BitsByte environment = new BitsByte(CometNight);
            writer.Write(environment);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte environment = reader.ReadByte();
            CometNight = environment[0];
        }

        public override void PostUpdateWorld()
        {
            HandleEventOccurancesAndDisappearances();
        }

        public void HandleEventOccurancesAndDisappearances()
        {
            // Comet Night.
            bool viableToStartEvent = (LanternNight.LanternsUp || Star.starfallBoost > 1f);
            if (Utilities.JustTurnedToNight && !CometNight)
            {
                Main.NewText("The night sky glimmers with cosmic energy...", Color.DeepSkyBlue);
                CometNight = true;
            }

            if (Main.dayTime && CometNight)
            {
                CometNight = false;
            }
        }
    }
}
