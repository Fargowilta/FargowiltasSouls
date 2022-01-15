using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class TimberTree : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Tree");
        }

        public override void SetDefaults()
        {
            projectile.width = 96;
            projectile.height = 304;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.timeLeft = 60;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.alpha = 255;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            projectile.velocity.Y += 1f;

            /*if (projectile.scale < 1f)
            {
                projectile.position.X += projectile.width / 2f;
                projectile.position.Y += projectile.height;

                projectile.width = (int)(projectile.width / projectile.scale);
                projectile.height = (int)(projectile.height / projectile.scale);

                projectile.scale += 0.01f;
                if (projectile.scale > 1f)
                    projectile.scale = 1f;

                projectile.width = (int)(projectile.width * projectile.scale);
                projectile.height = (int)(projectile.height * projectile.scale);

                projectile.position.X -= projectile.width / 2f;
                projectile.position.Y -= projectile.height;
            }*/

            if (projectile.alpha > 0)
            {
                projectile.alpha -= 10;
                if (projectile.alpha < 0)
                    projectile.alpha = 0;

                for (int i = 0; i < 5; i++)
                {
                    Dust.NewDust(projectile.position, projectile.width, projectile.height, 2);
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int p = Player.FindClosest(projectile.position, projectile.width, projectile.height);
                if (p != -1)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Vector2 spawnPos = projectile.position;
                        spawnPos.X += projectile.width / 2f + Main.rand.NextFloat(-40, 40);
                        spawnPos.Y += 40 + Main.rand.NextFloat(-40, 40);

                        const float gravity = 0.2f;
                        float time = 30f;
                        Vector2 distance = Main.player[p].Center - spawnPos;
                        distance.X = Main.rand.NextFloat(-2f, 2f);
                        distance.Y = distance.Y / time - 0.5f * gravity * time;
                        float minimumY = Main.rand.NextFloat(-12f, -9f);
                        if (distance.Y > minimumY)
                            distance.Y = minimumY;
                        distance += Main.rand.NextVector2Square(-0.5f, 0.5f) * 2;

                        if (Math.Abs(Main.player[(int)projectile.ai[0]].velocity.X) > 9f)
                            distance.X += Main.player[(int)projectile.ai[0]].velocity.X * 0.75f;

                        Projectile.NewProjectile(spawnPos, distance, ModContent.ProjectileType<Acorn>(), projectile.damage, projectile.knockBack, projectile.owner);
                    }
                }
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            SpriteEffects effects = projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}