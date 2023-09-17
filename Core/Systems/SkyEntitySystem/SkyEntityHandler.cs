using Cascade.Core.Configs;
using System.Collections.Generic;

namespace Cascade.Core.Systems.SkyEntitySystem
{
    public class SkyEntityHandler : ModSystem
    {
        private static List<SkyEntity> SkyEntities;

        private static List<SkyEntity> SkyEntitiesToBeKilled;

        public override void OnModLoad()
        {
            SkyEntities = new List<SkyEntity>();
            SkyEntitiesToBeKilled = new List<SkyEntity>();
        }

        public override void OnModUnload()
        {
            SkyEntities = null;
            SkyEntitiesToBeKilled = null;
        }

        public override void PostUpdateEverything()
        {
            foreach (SkyEntity skyEntity in SkyEntities)
            {
                if (skyEntity == null)
                    continue;

                skyEntity.Time++;
                skyEntity.Position += skyEntity.Velocity;
                skyEntity.Update();
            }

            SkyEntities.RemoveAll(skyEntity => (skyEntity.Time >= skyEntity.Lifespan && skyEntity.DieWithLifespan) || SkyEntitiesToBeKilled.Contains(skyEntity));
            SkyEntitiesToBeKilled.Clear();
        }

        public static void SpawnSkyEntity(SkyEntity skyEntity)
        {
            if (!Main.gamePaused && !Main.dedServ && SkyEntities != null && (SkyEntities.Count < GraphicalConfig.Instance.SkyEntityLimit || skyEntity.ShouldBypassLimit))
            {
                SkyEntities.Add(skyEntity);
            }
        }

        public static void RemoveSkyEntity(SkyEntity skyEntity)
        {
            SkyEntitiesToBeKilled.Add(skyEntity);
        }
    }
}
