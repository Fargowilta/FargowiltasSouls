using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class CosmosLightningOrb : TerraLightningOrb
    {
        public override string Texture => "Terraria/Projectile_465";

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.timeLeft = 600;
        }

        public override bool CanDamage()
        {
            return projectile.alpha == 0;
        }

        public override void AI()
        {
            if (projectile.timeLeft > 30)
            {
                projectile.alpha -= 10;
                if (projectile.alpha < 0)
                    projectile.alpha = 0;
            }
            else
            {
                projectile.alpha += 10;
                if (projectile.alpha > 255)
                {
                    projectile.alpha = 255;
                    projectile.Kill();
                }
            }

            if (++projectile.localAI[1] > 120 && projectile.localAI[1] < 240)
                projectile.velocity *= 1.05f;

            Lighting.AddLight(projectile.Center, 0.4f, 0.85f, 0.9f);
            projectile.frameCounter++;
            if (projectile.frameCounter > 3)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame > 3)
                    projectile.frame = 0;
            }

            /*if (Main.rand.Next(3) == 0)
            {
                float num11 = (float)(Main.rand.NextDouble() * 1.0 - 0.5); //vanilla dust :echbegone:
                if ((double)num11 < -0.5)
                    num11 = -0.5f;
                if ((double)num11 > 0.5)
                    num11 = 0.5f;
                Vector2 vector21 = new Vector2((float)-projectile.width * 0.2f * projectile.scale, 0.0f).RotatedBy((double)num11 * 6.28318548202515, new Vector2()).RotatedBy((double)projectile.velocity.ToRotation(), new Vector2());
                int index21 = Dust.NewDust(projectile.Center - Vector2.One * 5f, 10, 10, 226, (float)(-(double)projectile.velocity.X / 3.0), (float)(-(double)projectile.velocity.Y / 3.0), 150, Color.Transparent, 0.7f);
                Main.dust[index21].position = projectile.Center + vector21 * projectile.scale;
                Main.dust[index21].velocity = Vector2.Normalize(Main.dust[index21].position - projectile.Center) * 2f;
                Main.dust[index21].noGravity = true;
                float num1 = (float)(Main.rand.NextDouble() * 1.0 - 0.5);
                if ((double)num1 < -0.5)
                    num1 = -0.5f;
                if ((double)num1 > 0.5)
                    num1 = 0.5f;
                Vector2 vector2 = new Vector2((float)-projectile.width * 0.6f * projectile.scale, 0.0f).RotatedBy((double)num1 * 6.28318548202515, new Vector2()).RotatedBy((double)projectile.velocity.ToRotation(), new Vector2());
                int index2 = Dust.NewDust(projectile.Center - Vector2.One * 5f, 10, 10, 226, (float)(-(double)projectile.velocity.X / 3.0), (float)(-(double)projectile.velocity.Y / 3.0), 150, Color.Transparent, 0.7f);
                Main.dust[index2].velocity = Vector2.Zero;
                Main.dust[index2].position = projectile.Center + vector2 * projectile.scale;
                Main.dust[index2].noGravity = true;
            }*/
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 360);
            //if (FargoSoulsWorld.MasochistMode) target.AddBuff(ModContent.BuffType<LightningRod>(), 360);
        }
    }
}