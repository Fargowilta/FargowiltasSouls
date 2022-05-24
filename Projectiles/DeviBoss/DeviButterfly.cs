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
        public override string Texture => "Terraria/Images/NPC_205";

        public bool drawLoaded;
        public int drawBase;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Moth");
            Main.projFrames[Projectile.type] = Main.npcFrameCount[NPCID.Moth];//24;
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.timeLeft = 420;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;

            //Projectile.scale = 2f;
            Projectile.hide = true;

            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 1;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], ModContent.NPCType<NPCs.DeviBoss.DeviBoss>());
            if (npc == null)
            {
                Projectile.Kill();
                return;
            }

            if (!drawLoaded)
            {
                drawLoaded = true;
                drawBase = Main.rand.Next(8);
                Projectile.hide = false;
            }

            Vector2 target;
            target.X = npc.Center.X;
            target.Y = Main.player[npc.target].Center.Y;

            target.X += 1100 * (float)Math.Sin(2 * Math.PI / 600 * Projectile.ai[1]++);
            target.Y -= 400;

            Vector2 distance = target - Projectile.Center;
            float length = distance.Length();
            if (length > 25f)
            {
                distance /= 8f;
                Projectile.velocity = (Projectile.velocity * 23f + distance) / 24f;
            }
            else
            {
                if (Projectile.velocity.Length() < 12f)
                    Projectile.velocity *= 1.05f;
            }

            if (++Projectile.localAI[0] > 90) //spray shards
            {
                if (Projectile.localAI[0] > (npc.localAI[3] > 1 ? 120 : 105))
                {
                    Projectile.localAI[0] = npc.localAI[3] > 1 ? 30 : 45;
                }

                if (++Projectile.localAI[1] > 3)
                {
                    Projectile.localAI[1] = 0;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.UnitY * 3, ModContent.ProjectileType<DeviLightBall2>(),
                            Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }

                /*SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
                Projectile.localAI[1] = 0f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (Math.Abs(npc.Center.X - Projectile.Center.X) > (npc.localAI[3] > 1 ? 300 : 450))
                    {
                        Vector2 speed = new Vector2(Main.rand.Next(-1000, 1001), Main.rand.Next(-1000, 1001));
                        speed.Normalize();
                        speed *= 8f;
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center + speed * 4f, speed, ModContent.ProjectileType<AbomFrostShard>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center + Vector2.UnitY * 8f, Vector2.UnitY * 8f, ModContent.ProjectileType<AbomFrostShard>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                    if (Main.player[npc.target].active && !Main.player[npc.target].dead && Main.player[npc.target].Center.Y < Projectile.Center.Y)
                    {
                        SoundEngine.PlaySound(SoundID.Item120, Projectile.position);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 vel = Projectile.DirectionTo(Main.player[npc.target].Center + new Vector2(Main.rand.Next(-200, 201), Main.rand.Next(-200, 201))) * 12f;
                            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, vel, ModContent.ProjectileType<AbomFrostWave>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                    }
                }*/
            }

            Projectile.direction = Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);

            Projectile.frameCounter++;
            if (Projectile.frameCounter < 4)
                Projectile.frame = 0;
            else if (Projectile.frameCounter < 8)
                Projectile.frame = 1;
            else if (Projectile.frameCounter < 12)
                Projectile.frame = 2;
            else if (Projectile.frameCounter < 16)
                Projectile.frame = 1;
            else
                Projectile.frameCounter = 0;

            /*if (Projectile.frame < drawBase)
                Projectile.frame = drawBase;

            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;

                if (++Projectile.frame >= drawBase + 3)
                    Projectile.frame = drawBase;
            }*/
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 86, 0f, 0f, 0, default(Color), 2.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 8f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}