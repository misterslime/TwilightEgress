using System.Collections.Generic;

namespace Cascade
{
    public static partial class Utilities
    {
        public static SoundStyle GetRandomSoundFromList(List<SoundStyle> soundsToPickFrom) => soundsToPickFrom[Main.rand.Next(soundsToPickFrom.Count)];
    }
}
