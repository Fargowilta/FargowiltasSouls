using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class AbominationnSickle : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_44";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Abominationn Sickle");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 48;
            projectile.alpha = 100;
            projectile.light = 0.2f;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.timeLeft = 180;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;

            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                Main.PlaySound(SoundID.Item8, projectile.Center);
            }
            projectile.rotation += 0.8f;
            if (++projectile.localAI[1] > 30 && projectile.localAI[1] < 100)
                projectile.velocity *= 1.06f;
            for (int i = 0; i < 2; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 27, 0f, 0f, 100);
                Main.dust[d].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(mod.BuffType("MutantNibble"), 600);
            target.AddBuff(BuffID.ShadowFlame, 600);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}
