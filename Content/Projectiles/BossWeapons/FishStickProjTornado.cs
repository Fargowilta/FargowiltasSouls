using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class FishStickProjTornado : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.VanillaTextureProjectile(ProjectileID.Tempest);

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fish Stick");
            Main.projFrames[Type] = Main.projFrames[ProjectileID.Tempest];
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.aiStyle = -1;
            AIType = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;

            Projectile.FargoSouls().CanSplit = false;
        }
        public const int TravelTime = 30;
        ref float Timer => ref Projectile.ai[2];
        public override bool? CanDamage() => false;
        public override void AI()
        {
            if (++Projectile.frameCounter > 4)
                if (++Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 0;

            if (Timer >= TravelTime)
            {
                Projectile.velocity = Vector2.Zero;
            }
            foreach (Projectile tornado in Main.projectile.Where(p => p.active && p.type == ModContent.ProjectileType<FishStickProjTornado>() && p.owner == Projectile.owner && p.identity > Projectile.identity))
            {
                tornado.Kill();
            }
            Timer++;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.position -= oldVelocity;
            Projectile.velocity = Vector2.Zero;
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor);
            return false;
        }
    }
}