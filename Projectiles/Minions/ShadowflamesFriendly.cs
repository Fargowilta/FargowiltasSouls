using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class ShadowflamesFriendly : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_299";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Shadowflames");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.Shadowflames);
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.magic = false;
            projectile.minion = true;
            projectile.timeLeft = 180 * (projectile.extraUpdates + 1);
            projectile.tileCollide = true;

            projectile.penetrate = 2;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 10;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().noInteractionWithNPCImmunityFrames = true;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                Main.PlaySound(SoundID.Item8, projectile.position);
                for (int index1 = 0; index1 < 10; ++index1)
                {
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 181, 0.0f, 0.0f, 100, default, 1f);
                    Dust dust1 = Main.dust[index2];
                    dust1.velocity *= 3;
                    Dust dust2 = Main.dust[index2];
                    dust2.velocity += projectile.velocity * 0.5f;
                    Main.dust[index2].noGravity = true;
                }
            }

            for (int index1 = 0; index1 < 2; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 181, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default, 1f);
                Dust dust = Main.dust[index2];
                dust.velocity *= 0.6f;
                Main.dust[index2].noGravity = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item8, projectile.position);
            for (int index1 = 0; index1 < 30; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 181, 0.0f, 0.0f, 100, default, 1f);
                Dust dust1 = Main.dust[index2];
                dust1.velocity *= 3;
                Dust dust2 = Main.dust[index2];
                dust2.velocity += -projectile.velocity * 0.75f;
                Main.dust[index2].scale *= 1.2f;
                Main.dust[index2].noGravity = true;
            }
        }
    }
}