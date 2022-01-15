using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class MoonLordSunBlast : Champions.EarthChainBlast
    {
        public override string Texture => "Terraria/Projectile_687";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            //DisplayName.SetDefault("Sun Blast");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.width = 70;
            projectile.height = 70;
            cooldownSlot = 1;
        }

        public override bool CanDamage()
        {
            return projectile.frame == 3 || projectile.frame == 4;
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

            if (++projectile.frameCounter >= 2)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame--;
                    projectile.Kill();
                    return;
                }
                if (projectile.frame == 3)
                    projectile.GetGlobalProjectile<FargoGlobalProjectile>().GrazeCD = 0;
            }
            //if (++projectile.ai[0] > Main.projFrames[projectile.type] * 3) projectile.Kill();

            if (projectile.localAI[1] == 0)
            {
                Main.PlaySound(SoundID.Item88, projectile.Center);
                projectile.position = projectile.Center;
                projectile.scale = Main.rand.NextFloat(1.5f, 4f); //ensure no gaps
                projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                projectile.width = (int)(projectile.width * projectile.scale);
                projectile.height = (int)(projectile.height * projectile.scale);
                projectile.Center = projectile.position;
            }

            if (++projectile.localAI[1] == 6 && projectile.ai[1] > 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                projectile.ai[1]--;

                Vector2 baseDirection = projectile.ai[0].ToRotationVector2();
                float random = MathHelper.ToRadians(15);

                if (projectile.localAI[0] != 2f)
                {
                    //spawn stationary blasts
                    float stationaryPersistence = Math.Min(5, projectile.ai[1]); //stationaries always count down from this
                    int p = Projectile.NewProjectile(projectile.Center + Main.rand.NextVector2Circular(20, 20), Vector2.Zero, projectile.type,
                        projectile.damage, 0f, projectile.owner, projectile.ai[0], stationaryPersistence);
                    if (p != Main.maxProjectiles)
                        Main.projectile[p].localAI[0] = 1f; //only make more stationaries, don't propagate forward
                }

                //propagate forward
                if (projectile.localAI[0] != 1f)
                {
                    //10f / 7f is to compensate for shrunken hitbox
                    float length = projectile.width / projectile.scale * 10f / 7f;
                    Vector2 offset = length * baseDirection.RotatedBy(Main.rand.NextFloat(-random, random));
                    int p = Projectile.NewProjectile(projectile.Center + offset, Vector2.Zero, projectile.type,
                          projectile.damage, 0f, projectile.owner, projectile.ai[0], projectile.ai[1]);
                    if (p != Main.maxProjectiles)
                        Main.projectile[p].localAI[0] = projectile.localAI[0];
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Burning, 120);
            target.AddBuff(BuffID.OnFire, 300);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Color color = new Color(255, 255, 255, 200);
            //color = Color.Lerp(new Color(255, 95, 46, 50), new Color(150, 35, 0, 100), (4 - projectile.ai[1]) / 4);

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color,
                projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}

