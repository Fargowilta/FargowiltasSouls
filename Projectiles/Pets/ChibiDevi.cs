using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Pets
{
    public class ChibiDevi : ModProjectile
    {
        private Vector2 oldMouse;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Chibi Devi");
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.LightPet[base.projectile.type] = true;
            Main.projPet[base.projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 44;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            projectile.netImportant = true;
            projectile.friendly = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.rotation);
            writer.Write(projectile.direction);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.rotation = reader.ReadSingle();
            projectile.direction = reader.ReadInt32();
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            if (player.dead)
            {
                modPlayer.ChibiDevi = false;
            }
            if (modPlayer.ChibiDevi)
            {
                projectile.timeLeft = 2;
            }

            DelegateMethods.v3_1 = new Vector3(1f, 0.5f, 0.9f) * 0.75f;
            Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * 6f, 20f, new Utils.PerLinePoint(DelegateMethods.CastLightOpen));
            Utils.PlotTileLine(projectile.Left, projectile.Right, 20f, new Utils.PerLinePoint(DelegateMethods.CastLightOpen));

            if (projectile.ai[0] == 1)
            {
                projectile.tileCollide = true;
                projectile.ignoreWater = false;

                projectile.frameCounter = 0;
                projectile.frame = projectile.velocity.Y == 0 ? 5 : 4;

                projectile.velocity.X *= 0.95f;
                projectile.velocity.Y += 0.3f;

                if (projectile.owner == Main.myPlayer && projectile.Distance(Main.MouseWorld) > 180)
                {
                    projectile.ai[0] = 0;
                }
            }
            else
            {
                if (projectile.owner == Main.myPlayer)
                {
                    projectile.tileCollide = false;
                    projectile.ignoreWater = true;

                    projectile.direction = projectile.Center.X < Main.MouseWorld.X ? 1 : -1;

                    float distance = 2500;
                    float possibleDist = Main.player[projectile.owner].Distance(Main.MouseWorld) / 2 + 100;
                    if (distance < possibleDist)
                        distance = possibleDist;
                    if (projectile.Distance(Main.player[projectile.owner].Center) > distance && projectile.Distance(Main.MouseWorld) > distance)
                    {
                        projectile.Center = player.Center;
                        projectile.velocity = Vector2.Zero;
                    }

                    if (projectile.Distance(Main.MouseWorld) > 30)
                        Movement(Main.MouseWorld, 0.15f, 32f);

                    if (oldMouse == Main.MouseWorld)
                    {
                        projectile.ai[1]++;
                        if (projectile.ai[1] > 600)
                        {
                            bool okToRest = !Collision.SolidCollision(projectile.position, projectile.width, projectile.height);

                            if (okToRest)
                            {
                                okToRest = false;

                                Vector2 targetPos = new Vector2(projectile.Center.X, projectile.position.Y + projectile.height);
                                for (int i = 0; i < 10; i++) //collision check below self
                                {
                                    targetPos.Y += 16;
                                    Tile tile = Framing.GetTileSafely(targetPos); //if solid, ok
                                    if (tile.active() && !tile.inActive() && Main.tileSolid[tile.type])
                                    {
                                        okToRest = true;
                                        break;
                                    }
                                }
                            }

                            if (okToRest) //not in solid tiles, but found tiles within a short distance below
                            {
                                projectile.ai[0] = 1;
                                projectile.ai[1] = 0;
                            }
                            else //try again in a bit
                            {
                                projectile.ai[1] = 540;
                            }
                        }
                    }
                    else
                    {
                        projectile.ai[1] = 0;
                        oldMouse = Main.MouseWorld;
                    }
                }

                if (++projectile.frameCounter > 6)
                {
                    projectile.frameCounter = 0;
                    if (++projectile.frame >= 4)
                        projectile.frame = 0;
                }
            }

            projectile.spriteDirection = projectile.direction;
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

        private void Movement(Vector2 targetPos, float speedModifier, float cap = 12f)
        {
            if (projectile.Center.X < targetPos.X)
            {
                projectile.velocity.X += speedModifier;
                if (projectile.velocity.X < 0)
                    projectile.velocity.X *= 0.95f;
            }
            else
            {
                projectile.velocity.X -= speedModifier;
                if (projectile.velocity.X > 0)
                    projectile.velocity.X *= 0.95f;
            }
            if (projectile.Center.Y < targetPos.Y)
            {
                projectile.velocity.Y += speedModifier;
                if (projectile.velocity.Y < 0)
                    projectile.velocity.Y *= 0.95f;
            }
            else
            {
                projectile.velocity.Y -= speedModifier;
                if (projectile.velocity.Y > 0)
                    projectile.velocity.Y *= 0.95f;
            }
            if (Math.Abs(projectile.velocity.X) > cap)
                projectile.velocity.X = cap * Math.Sign(projectile.velocity.X);
            if (Math.Abs(projectile.velocity.Y) > cap)
                projectile.velocity.Y = cap * Math.Sign(projectile.velocity.Y);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects spriteEffects = projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, spriteEffects, 0f);
            return false;
        }
    }
}