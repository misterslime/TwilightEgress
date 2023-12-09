using Cascade.Content.NPCs.CosmostoneShowers;

namespace Cascade.Core.Players
{
    public class GravityPlanetoidDetour : ModSystem
    {
        public override void OnModLoad() => On_Player.DryCollision += UpdateVelocityOnCollision;

        public override void OnModUnload() => On_Player.DryCollision -= UpdateVelocityOnCollision;

        private void UpdateVelocityOnCollision(On_Player.orig_DryCollision orig, Player self, bool fallThrough, bool ignorePlats)
        {
            GravityPlanetoidPlayer player = self.GetModPlayer<GravityPlanetoidPlayer>();
            if (player.Planetoid is not null && player.Planetoid.NPC.active)
                self.velocity.Y = 0f;

            orig.Invoke(self, fallThrough, ignorePlats);
        }
    }

    public class GravityPlanetoidPlayer : ModPlayer
    {
        public GravityPlanetoid Planetoid = null;

        public float PlayerAngle = 0f;

        public float AngleSwitchTimer = 0f;

        public float AttractionSpeed = 0f;

        public float AttractionCooldown = 0f;

        private const int MaxAttractionCooldown = 45;

        private const int MaxAngleSwitchTimer = 60;

        public override void PostUpdate()
        {
            // Properly resets rotation.
            if (AngleSwitchTimer < 1f)
                Player.fullRotation = 0f;

            if (Planetoid is not null && Planetoid.NPC.active)
            {
                if (AttractionSpeed < 0.0085f)
                    AttractionSpeed += 0.0001f;
                if (AttractionSpeed > 0.0085f)
                    AttractionSpeed = 0.0085f;

                float radiusBetweenSurfaceAndAtmosphere = Planetoid.MaximumAttractionRadius - Planetoid.WalkableRadius;
                Vector2 maximumRadiusPosition = Planetoid.NPC.Center + Vector2.UnitX.RotatedBy(PlayerAngle) * (Planetoid.MaximumAttractionRadius + Planetoid.WalkableRadius);
                Vector2 walkablePlanetoidArea = Planetoid.NPC.Center + Vector2.UnitX.RotatedBy(PlayerAngle) * (radiusBetweenSurfaceAndAtmosphere - Player.height);
                Player.MountedCenter = Vector2.Lerp(Player.MountedCenter, walkablePlanetoidArea, AttractionSpeed / 0.0085f);

                PlayerAngle += Player.velocity.X / Planetoid.WalkableRadius;
                PlayerAngle %= Tau;

                // Eject the player from the Planetoid once they jump.
                if (Player.justJumped && AngleSwitchTimer >= MaxAngleSwitchTimer)
                {
                    Planetoid = null;
                    Player.velocity = Vector2.UnitX.RotatedBy(PlayerAngle) * 12f;
                    AttractionCooldown = MaxAttractionCooldown;
                }

                if (AngleSwitchTimer < MaxAngleSwitchTimer)
                    AngleSwitchTimer += 4f;
                if (AngleSwitchTimer > MaxAngleSwitchTimer)
                    AngleSwitchTimer = MaxAngleSwitchTimer;
            }
            else
            {
                // Reset any necessary variables when not in a zone of gravity.
                AngleSwitchTimer = Clamp(AngleSwitchTimer - 3f, 0f, MaxAngleSwitchTimer);
                AttractionCooldown = Clamp(AttractionCooldown - 1f, 0f, MaxAttractionCooldown);
                AttractionSpeed = 0f;
                Planetoid = null;
            }            
        }

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            if (Planetoid is not null && Planetoid.NPC.active || AngleSwitchTimer > 1f)
            {
                // Ensure the player rotates towards the Planetoid properly.
                drawInfo.drawPlayer.fullRotationOrigin = drawInfo.drawPlayer.Size / 2f;

                float realAngle = PlayerAngle * (AngleSwitchTimer / 59f);
                drawInfo.drawPlayer.fullRotation = realAngle + PiOver2 * (AngleSwitchTimer / 59f);
            }            
            
        }
    }
}
