using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cascade.Content.Items.Weapons.Ranged.SailorsSingularity
{
    public class SailorStar : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 3;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
        }
        public override void AI()
        {
            Projectile.rotation += 0.15f;
            Lighting.AddLight(Projectile.Center, new Vector3(0.22f, 0.32f, 0.81f));
        }
    }
}
