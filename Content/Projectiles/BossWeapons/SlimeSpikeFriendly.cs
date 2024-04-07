using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class SlimeSpikeFriendly : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_605";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slime Spike");
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.height = 6;
            Projectile.width = 6;
            Projectile.aiStyle = 1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.timeLeft = 30;
            Projectile.penetrate = 2;
        }

        public override void AI()
        {
            if (Projectile.alpha == 0 && Main.rand.NextBool(3))
            {
                int num69 = Dust.NewDust(Projectile.position - Projectile.velocity * 3f, Projectile.width, Projectile.height, DustID.TintableDust, 0f, 0f, 50, new Color(78, 136, 255, 150), 1.2f);
                Main.dust[num69].velocity *= 0.3f;
                Main.dust[num69].velocity += Projectile.velocity * 0.3f;
                Main.dust[num69].noGravity = true;
            }
            Projectile.alpha -= 50;
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item17, Projectile.position);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Slimed, 150);
            Projectile.timeLeft = 0;
        }
    }
}