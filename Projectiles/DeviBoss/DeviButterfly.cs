using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.DeviBoss
{
    public class DeviButterfly : ModProjectile
    {
        public override string Texture => "Terraria/NPC_205";

        public bool drawLoaded;
        public int drawBase;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Moth");
            Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.Moth];//24;
        }

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.timeLeft = 420;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;

            //projectile.scale = 2f;
            projectile.hide = true;

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 1;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[0], ModContent.NPCType<NPCs.DeviBoss.DeviBoss>());
            if (npc == null)
            {
                projectile.Kill();
                return;
            }

            if (!drawLoaded)
            {
                drawLoaded = true;
                drawBase = Main.rand.Next(8);
                projectile.hide = false;
            }

            Vector2 target;
            target.X = npc.Center.X;
            target.Y = Main.player[npc.target].Center.Y;

            target.X += 1100 * (float)Math.Sin(2 * Math.PI / 600 * projectile.ai[1]++);
            target.Y -= 400;

            Vector2 distance = target - projectile.Center;
            float length = distance.Length();
            if (length > 25f)
            {
                distance /= 8f;
                projectile.velocity = (projectile.velocity * 23f + distance) / 24f;
            }
            else
            {
                if (projectile.velocity.Length() < 12f)
                    projectile.velocity *= 1.05f;
            }
            
            if (++projectile.localAI[0] > 90) //spray shards
            {
                if (projectile.localAI[0] > (npc.localAI[3] > 1 ? 120 : 105))
                {
                    projectile.localAI[0] = npc.localAI[3] > 1 ? 30 : 45;
                }

                if (++projectile.localAI[1] > 3)
                {
                    projectile.localAI[1] = 0;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(projectile.Center, Vector2.UnitY * 3, mod.ProjectileType("DeviLightBall2"),
                            projectile.damage, projectile.knockBack, projectile.owner);
                    }
                }

                /*Main.PlaySound(SoundID.Item27, projectile.position);
                projectile.localAI[1] = 0f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (Math.Abs(npc.Center.X - projectile.Center.X) > (npc.localAI[3] > 1 ? 300 : 450))
                    {
                        Vector2 speed = new Vector2(Main.rand.Next(-1000, 1001), Main.rand.Next(-1000, 1001));
                        speed.Normalize();
                        speed *= 8f;
                        Projectile.NewProjectile(projectile.Center + speed * 4f, speed, mod.ProjectileType("AbomFrostShard"), projectile.damage, projectile.knockBack, projectile.owner);
                        Projectile.NewProjectile(projectile.Center + Vector2.UnitY * 8f, Vector2.UnitY * 8f, mod.ProjectileType("AbomFrostShard"), projectile.damage, projectile.knockBack, projectile.owner);
                    }
                    if (Main.player[npc.target].active && !Main.player[npc.target].dead && Main.player[npc.target].Center.Y < projectile.Center.Y)
                    {
                        Main.PlaySound(SoundID.Item120, projectile.position);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 vel = projectile.DirectionTo(Main.player[npc.target].Center + new Vector2(Main.rand.Next(-200, 201), Main.rand.Next(-200, 201))) * 12f;
                            Projectile.NewProjectile(projectile.Center, vel, mod.ProjectileType("AbomFrostWave"), projectile.damage, projectile.knockBack, projectile.owner);
                        }
                    }
                }*/
            }

            projectile.direction = projectile.spriteDirection = Math.Sign(projectile.velocity.X);

            projectile.frameCounter++;
            if (projectile.frameCounter < 4)
                projectile.frame = 0;
            else if (projectile.frameCounter < 8)
                projectile.frame = 1;
            else if (projectile.frameCounter < 12)
                projectile.frame = 2;
            else if (projectile.frameCounter < 16)
                projectile.frame = 1;
            else
                projectile.frameCounter = 0;

            /*if (projectile.frame < drawBase)
                projectile.frame = drawBase;

            if (++projectile.frameCounter > 6)
            {
                projectile.frameCounter = 0;

                if (++projectile.frame >= drawBase + 3)
                    projectile.frame = drawBase;
            }*/
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 86, 0f, 0f, 0, default(Color), 2.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 8f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects effects = projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}