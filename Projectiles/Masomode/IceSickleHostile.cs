using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class IceSickleHostile : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_263";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Ice Sickle");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.IceSickle);
            aiType = ProjectileID.IceSickle;
            projectile.hostile = true;
            projectile.melee = false;
            projectile.friendly = false;
            projectile.light = 0f;
            projectile.coldDamage = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, .05f, .35f, .5f);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item27, projectile.Center);
            for (int i = 0; i < 15; ++i)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 92, projectile.velocity.X, projectile.velocity.Y, Main.rand.Next(0, 101), new Color(), 1 + Main.rand.Next(40) * 0.01f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 2f;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, projectile.timeLeft < 255 ? projectile.timeLeft : 255);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 240);
            target.AddBuff(BuffID.Chilled, 120);
        }
    }
}