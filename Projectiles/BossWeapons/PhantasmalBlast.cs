using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class PhantasmalBlast : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_645";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Phantasmal Blast");
            Main.projFrames[projectile.type] = Main.projFrames[ProjectileID.LunarFlare];
        }

        public override void SetDefaults()
        {
            projectile.width = 100;
            projectile.height = 100;
            projectile.aiStyle = -1;
            //aiType = ProjectileID.LunarFlare;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            //projectile.extraUpdates = 5;
            projectile.penetrate = -1;
            projectile.scale = 1f;
            projectile.alpha = 0;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override void AI()
        {
            if (projectile.position.HasNaNs())
            {
                projectile.Kill();
                return;
            }
            /*Dust dust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0f, 0f, 0, new Color(), 1f)];
            dust.position = projectile.Center;
            dust.velocity = Vector2.Zero;
            dust.noGravity = true;
            dust.noLight = true;*/

            if (++projectile.frameCounter >= 3)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame--;
                    projectile.Kill();
                }
            }
            //if (++projectile.ai[0] > Main.projFrames[projectile.type] * 3) projectile.Kill();

            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1f;
                Main.PlaySound(SoundID.Item88, projectile.Center);
                projectile.scale = Main.rand.NextFloat(1f, 3f);
                projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            }
        }

        public override bool CanDamage()
        {
            return projectile.frame < 4;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 600);
            target.immune[projectile.owner] = 1;
        }

        public override void Kill(int timeLeft)
        {
            //for (int i = 0; i < 4; ++i)
            //    Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, new Color(), 1.5f);
            /*for (int i = 0; i < 4; ++i)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0f, 0f, 0, new Color(), 2.5f);
                Main.dust[d].velocity *= 3f;
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
                d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0f, 0f, 100, new Color(), 1.5f);
                Main.dust[d].velocity *= 2f;
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
            }*/
            /*int i2 = Gore.NewGore(projectile.position + new Vector2(projectile.width * Main.rand.Next(100) / 100f, projectile.height * Main.rand.Next(100) / 100f) - Vector2.One * 10f, new Vector2(), Main.rand.Next(61, 64), 1f);
            Main.gore[i2].velocity *= 0.3f;
            Main.gore[i2].velocity.X += Main.rand.Next(-10, 11) * 0.05f;
            Main.gore[i2].velocity.Y += Main.rand.Next(-10, 11) * 0.05f;*/
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 127) * projectile.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}

