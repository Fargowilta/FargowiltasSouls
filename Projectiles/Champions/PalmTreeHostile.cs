using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class PalmTreeHostile : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Minions/PalmTreeSentry";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Palm Tree");
        }

        public override void SetDefaults()
        {
            projectile.width = 70;
            projectile.height = 70;
            projectile.aiStyle = -1;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[0], ModContent.NPCType<NPCs.Champions.TimberChampion>());
            if (npc == null)
            {
                projectile.Kill();
                return;
            }
            Player player = Main.player[npc.target];

            projectile.timeLeft = 2;

            if (projectile.ai[1] == 0)
            {
                projectile.tileCollide = true;

                if (projectile.Distance(player.Center) > 1200)
                {
                    projectile.ai[1] = 1;
                    projectile.netUpdate = true;
                    return;
                }

                if (projectile.velocity.Y == 0 && --projectile.localAI[1] < 0)
                {
                    projectile.localAI[1] = 120f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        const float gravity = 0.2f;
                        float time = 90f;
                        Vector2 distance = player.Center - projectile.Center;
                        distance.X = distance.X / time;
                        distance.Y = distance.Y / time - 0.5f * gravity * time;
                        for (int i = 0; i < 10; i++)
                        {
                            Projectile.NewProjectile(projectile.Center, distance + Main.rand.NextVector2Square(-0.5f, 0.5f) * 2,
                                ModContent.ProjectileType<Acorn>(), projectile.damage, 0f, Main.myPlayer);
                        }
                    }
                }

                projectile.velocity.X *= 0.95f;
                projectile.velocity.Y = projectile.velocity.Y + 0.2f;
                if (projectile.velocity.Y > 16f)
                {
                    projectile.velocity.Y = 16f;
                }
            }
            else //chase to get back in range
            {
                projectile.tileCollide = false;
                projectile.localAI[1] = 0;

                if (projectile.Distance(player.Center) < 500)
                {
                    projectile.ai[1] = 0;
                    projectile.netUpdate = true;
                    return;
                }

                Vector2 targetPos = player.Center;
                const float speedModifier = 0.35f;
                const float cap = 20f;

                if (projectile.Center.X < targetPos.X)
                {
                    projectile.velocity.X += speedModifier;
                    if (projectile.velocity.X < 0)
                        projectile.velocity.X += speedModifier * 2;
                }
                else
                {
                    projectile.velocity.X -= speedModifier;
                    if (projectile.velocity.X > 0)
                        projectile.velocity.X -= speedModifier * 2;
                }
                if (projectile.Center.Y < targetPos.Y)
                {
                    projectile.velocity.Y += speedModifier;
                    if (projectile.velocity.Y < 0)
                        projectile.velocity.Y += speedModifier * 2;
                }
                else
                {
                    projectile.velocity.Y -= speedModifier;
                    if (projectile.velocity.Y > 0)
                        projectile.velocity.Y -= speedModifier * 2;
                }
                if (Math.Abs(projectile.velocity.X) > cap)
                    projectile.velocity.X = cap * Math.Sign(projectile.velocity.X);
                if (Math.Abs(projectile.velocity.Y) > cap)
                    projectile.velocity.Y = cap * Math.Sign(projectile.velocity.Y);
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = false;
            return true;
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
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
