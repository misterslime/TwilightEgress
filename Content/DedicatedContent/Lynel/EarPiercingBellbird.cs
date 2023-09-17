using Steamworks;

namespace Cascade.Content.DedicatedContent.Lynel
{
    public class EarPiercingBellbird : ModProjectile
    {
        public enum AIState
        {
            Flying,
            Perched,
            MyFuckingEarsAreBurning,
        }

        public Player Owner => Main.player[Projectile.owner];

        public ref float Timer => ref Projectile.ai[0];

        public override string Texture => "Terraria/Images/Extra_13";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Harmless Little Bellbird");
        }
    }
}
