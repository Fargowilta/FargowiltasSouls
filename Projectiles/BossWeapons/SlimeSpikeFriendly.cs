using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class SlimeSpikeFriendly : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_605";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Slime Spike");
        }

        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.height = 6;
            projectile.width = 6;
            projectile.aiStyle = 1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.timeLeft = 30;
            projectile.penetrate = 2;
        }

        public override void AI()
        {
            if (projectile.alpha == 0 && Main.rand.NextBool(3))
            {
                int num69 = Dust.NewDust(projectile.position - projectile.velocity * 3f, projectile.width, projectile.height, 4, 0f, 0f, 50, new Color(78, 136, 255, 150), 1.2f);
                Main.dust[num69].velocity *= 0.3f;
                Main.dust[num69].velocity += projectile.velocity * 0.3f;
                Main.dust[num69].noGravity = true;
            }
            projectile.alpha -= 50;
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                Main.PlaySound(SoundID.Item17, projectile.position);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Slimed, 150);
            projectile.timeLeft = 0;
        }
    }
}