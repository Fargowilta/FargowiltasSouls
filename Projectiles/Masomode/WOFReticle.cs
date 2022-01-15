using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class WOFReticle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Wall of Flesh Reticle");
        }

        public override void SetDefaults()
        {
            projectile.width = 110;
            projectile.height = 110;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.alpha = 255;
            projectile.timeLeft = 300;
            //cooldownSlot = 1;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
                projectile.localAI[0] = Main.rand.NextBool() ? 1 : -1;

            if (++projectile.ai[0] < 130)
            {
                projectile.alpha -= 2;
                if (projectile.alpha < 0) //fade in
                    projectile.alpha = 0;
            }
            else if (projectile.ai[0] < 145)
            {
                if (projectile.ai[0] == 130 && Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, -1, -13);

                if (projectile.ai[0] % 6 == 0)
                {
                    //Vector2 spawnPos = projectile.Center;
                    //spawnPos.X += Main.rand.NextFloat(-250, 250);
                    //spawnPos.Y += Main.rand.NextFloat(700, 800) * projectile.localAI[0];
                    float angle = MathHelper.ToRadians(15);
                    Vector2 spawnPos;
                    spawnPos.Y = (projectile.localAI[0] > 0 ? Main.maxTilesY * 16 - 16 : Main.maxTilesY * 16 - 200 * 16) - projectile.Center.Y;
                    spawnPos.X = spawnPos.Y * (float)Math.Tan(Main.rand.NextFloat(-angle, angle));
                    spawnPos += projectile.Center;
                    
                    Vector2 vel = Main.rand.NextFloat(0.8f, 1.2f) * (projectile.Center - spawnPos) / 90;
                    if (vel.Length() < 10f)
                        vel = Vector2.Normalize(vel) * Main.rand.NextFloat(10f, 15f);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(spawnPos, vel, ModContent.ProjectileType<WOFChain>(), projectile.damage, 0f, Main.myPlayer);

                    Main.PlaySound(4, (int)projectile.Center.X, (int)projectile.Center.Y, 13, volumeScale: 0.5f);

                    projectile.localAI[0] *= -1;
                }
            }
            else
            {
                projectile.alpha += 8;
                if (projectile.alpha > 255) //fade out
                {
                    projectile.alpha = 255;
                    projectile.Kill();
                }
            }

            int modifier = Math.Min(110, (int)projectile.ai[0]);

            projectile.scale = 4f - 3f / 110 * modifier; //start big, shrink down

            /*projectile.Center = Main.npc[ai0].Center;
            projectile.velocity = Main.player[Main.npc[ai0].target].Center - projectile.Center;
            projectile.velocity = projectile.velocity / 60 * modifier; //move from npc to player*/
            projectile.rotation = (float)Math.PI * 2f / 55 * modifier * projectile.localAI[0];
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 128) * (1f - projectile.alpha / 255f);
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