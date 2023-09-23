using Terraria.ModLoader.IO;

namespace Cascade.Core.Systems
{
    public class WorldSavingSystem : ModSystem
    {
        public static bool LightCosmostoneShower { get; set; }

        public static bool CosmostoneShower { get; set; }

        public override void SaveWorldData(TagCompound tag)
        {
            // Events.
            tag.Add("CosmostoneShower", CosmostoneShower);
            tag.Add("LightCosmostoneShower", LightCosmostoneShower);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            CosmostoneShower = tag.ContainsKey("CosmostoneShower");
            LightCosmostoneShower = tag.ContainsKey("LightCosmostoneShower");
        }
    }
}
