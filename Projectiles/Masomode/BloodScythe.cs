using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class BloodScythe : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Moon Sickle");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.DemonSickle);
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.magic = false;
            projectile.timeLeft = 300;
            projectile.tileCollide = false;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                Main.PlaySound(SoundID.Item8, projectile.Center);
            }
            projectile.rotation += 0.8f;
            if (++projectile.localAI[1] > 30 && projectile.localAI[1] < 120)
                projectile.velocity *= 1.03f;
            
            for (int i = 0; i < 3; i++)
            {
                Vector2 offset = new Vector2(0, -20).RotatedBy(projectile.rotation);
                offset = offset.RotatedByRandom(MathHelper.Pi / 6);
                int d = Dust.NewDust(projectile.Center, 0, 0, 229, 0f, 0f, 150);
                Main.dust[d].position += offset;
                float velrando = Main.rand.Next(20, 31) / 10;
                Main.dust[d].velocity = projectile.velocity / velrando;
                Main.dust[d].noGravity = true;
            }

            if (projectile.timeLeft < 180)
                projectile.tileCollide = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            //target.AddBuff(mod.BuffType("Shadowflame"), 300);
            //target.AddBuff(BuffID.Bleeding, 600);
            //target.AddBuff(BuffID.Obstructed, 15);
            target.AddBuff(mod.BuffType("Berserked"), 300);
            target.AddBuff(mod.BuffType("CurseoftheMoon"), 120);
        }
    }
}
