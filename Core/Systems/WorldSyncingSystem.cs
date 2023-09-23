using static Cascade.Core.Systems.WorldSavingSystem;

namespace Cascade.Core.Systems
{
    public class WorldSyncingSystem : ModSystem
    {
        public override void NetSend(BinaryWriter writer)
        {
            // Full-scale events.
            BitsByte events = new BitsByte();
            // Small ambient events, akin to Spirit's.
            BitsByte environment = new BitsByte();

            events[0] = CosmostoneShower;

            environment[0] = LightCosmostoneShower;

            writer.Write(events);
            writer.Write(environment);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte events = reader.ReadByte();
            BitsByte environment = reader.ReadByte();

            CosmostoneShower = events[0];

            LightCosmostoneShower = environment[0];
        }
    }
}
