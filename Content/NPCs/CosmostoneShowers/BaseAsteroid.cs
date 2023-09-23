using System.Collections.Generic;

namespace Cascade.Content.NPCs.CosmostoneShowers
{
    public abstract class BaseAsteroid : ModNPC
    {
        public ref float RotationSpeedSpawnFactor => ref NPC.Cascade().ExtraAI[0];

        public ref float MaxTime => ref NPC.Cascade().ExtraAI[1];

        public ref float Timer => ref NPC.ai[0];

        public virtual void OnMeteorCrashKill() { }

        public virtual void SafeOnSpawn(IEntitySource source) { }

        public virtual void SafeAI() { }

        public sealed override void OnSpawn(IEntitySource source)
        {
            RotationSpeedSpawnFactor = Main.rand.NextFloat(75f, 480f) * Utils.SelectRandom(Main.rand, -1, 1);
            MaxTime = Main.rand.NextFloat(1200, 7200);
            SafeOnSpawn(source);
        }

        public sealed override void AI()
        {
            NPC.TargetClosest();

            // Fade in.
            if (Timer < MaxTime)
                NPC.Opacity = Clamp(NPC.Opacity + 0.02f, 0f, 1f);

            // Idly rotate.
            NPC.rotation += Pi / RotationSpeedSpawnFactor;
            NPC.rotation += NPC.velocity.X * 0.03f;

            // If an asteroid falls within a certain distance of Terraria's mesosphere, it
            // begins to be pulled in by the planet's gravity.

            // In simple terms, the Y-velocity is increased once the asteroid is pushed low enough.
            if (NPC.Bottom.Y >= Main.maxTilesY + 135f)
            {
                // Increase damage as Y-velocity begins to increase.
                NPC.damage = 150 * (int)Utils.GetLerpValue(0f, 1f, NPC.velocity.Y / 12f, true);
                NPC.velocity.Y = Clamp(NPC.velocity.Y + 0.03f, 0f, 18f);

                // Die upon tile collision, explode.
                if (Collision.SolidCollision(NPC.Center, NPC.width, NPC.height))
                {
                    NPC.life = 0;
                    NPC.checkDead();
                    OnMeteorCrashKill();
                    NPC.netUpdate = true;
                    return;
                }
            }
            else
            {
                // Kill the asteroid's damage if it is simply floating around.
                NPC.damage = 0;
                // Always decrease velocity so that they don't drift off into No Man's Land.
                NPC.velocity *= 0.99f;
            }

            NPC.ShowNameOnHover = false;

            // Resize the hitbox based on scale.
            int oldWidth = NPC.width;
            int idealWidth = (int)(NPC.scale * 36f);
            int idealHeight = (int)(NPC.scale * 36f);
            if (idealWidth != oldWidth)
            {
                NPC.position.X += NPC.width / 2;
                NPC.position.Y += NPC.height / 2;
                NPC.width = idealWidth;
                NPC.height = idealHeight;
                NPC.position.X -= NPC.width / 2;
                NPC.position.Y -= NPC.height / 2;
            }

            // Despawn after some time as to not clog the NPC limit.
            Timer++;
            if (Timer >= MaxTime)
            {
                NPC.Opacity = Clamp(NPC.Opacity - 0.02f, 0f, 1f);
                if (NPC.Opacity <= 0f)
                {
                    NPC.active = false;
                }
            }

            SafeAI();
        }
    }
}
