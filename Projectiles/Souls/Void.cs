using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class Void : ModProjectile
    {
        private int timer = 0;

        public override string Texture => "Terraria/Images/Projectile_578";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Void");
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = 0;
            Projectile.scale = 1f;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            AIType = ProjectileID.Bullet;

        }

        public override void AI()
        {
            const int maxScale = 3;

            Projectile.ai[0]++;

            //spawning in
            if (Projectile.ai[0] <= 40)
            {
                Projectile.scale = (Projectile.ai[0]) / 40 * maxScale;
                Projectile.alpha = 255 - (int)(255 * Projectile.scale / maxScale);
                Projectile.rotation = Projectile.rotation - 0.1570796f;
                if (Main.rand.NextBool())
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515) * Projectile.scale;
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint * 30f, 0, 0, 229, 0.0f, 0.0f, 0, new Color(), 1f)];
                    dust.noGravity = true;
                    dust.position = Projectile.Center - spinningpoint * Main.rand.Next(10, 21);
                    dust.velocity = spinningpoint.RotatedBy(1.57079637050629, new Vector2()) * 6f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                    dust.customData = Projectile.Center;
                }
                if (Main.rand.NextBool())
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515) * Projectile.scale;
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint * 30f, 0, 0, 240, 0.0f, 0.0f, 0, new Color(), 1f)];
                    dust.noGravity = true;
                    dust.position = Projectile.Center - spinningpoint * 30f;
                    dust.velocity = spinningpoint.RotatedBy(-1.57079637050629, new Vector2()) * 3f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                    dust.customData = Projectile.Center;
                }

                int p = Player.FindClosest(Projectile.Center, 0, 0);
                if (p != -1)
                {
                    Projectile.localAI[1] =
                        Projectile.Center == Main.player[p].Center ? 0 : Projectile.DirectionTo(Main.player[p].Center).ToRotation();
                    Projectile.localAI[1] += (float)Math.PI * 2 / 3 / 2;
                }
            }
            else //if (Projectile.ai[0] <= 40 + time)
            {
                Projectile.scale = maxScale;
                Projectile.alpha = 0;
                Projectile.rotation = Projectile.rotation - (float)Math.PI / 60f;
                if (Main.rand.NextBool())
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515) * Projectile.scale;
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint * 30f, 0, 0, 229, 0.0f, 0.0f, 0, new Color(), 1f)];
                    dust.noGravity = true;
                    dust.position = Projectile.Center - spinningpoint * Main.rand.Next(10, 21);
                    dust.velocity = spinningpoint.RotatedBy(1.57079637050629, new Vector2()) * 6f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                    dust.customData = Projectile.Center;
                }
                else
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515) * Projectile.scale;
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint * 30f, 0, 0, 240, 0.0f, 0.0f, 0, new Color(), 1f)];
                    dust.noGravity = true;
                    dust.position = Projectile.Center - spinningpoint * 30f;
                    dust.velocity = spinningpoint.RotatedBy(-1.57079637050629, new Vector2()) * 3f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                    dust.customData = Projectile.Center;
                }


                //suck
                if (timer == 0)
                {
                    int minDist = 500;
                    Player player = Main.player[Projectile.owner];

                    /*for (int i = 0; i < 200; i++)
                    {
                        float distance = Projectile.Distance(Main.npc[i].Center);
                        NPC npc = Main.npc[i];

                        if (!npc.active || npc.townNPC || npc.type == NPCID.TargetDummy || npc.type == NPCID.DD2EterniaCrystal || npc.type == NPCID.DD2LanePortal || npc.boss ||
                            (distance > minDist))
                        {
                            continue;
                        }

                        Vector2 dir = Projectile.position - npc.Center;
                        npc.velocity = dir.SafeNormalize(Vector2.Zero) * 8;

                        if (Projectile.timeLeft <= 30 && distance < minDist / 4)
                        {
                            npc.active = false;
                        }
                    }*/

                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        float distance = Projectile.Distance(Main.projectile[i].Center);
                        Projectile proj = Main.projectile[i];

                        if (!FargoSoulsUtil.CanDeleteProjectile(proj) || (distance > minDist) || proj.type == Projectile.type || (proj.owner == Projectile.owner && player.GetModPlayer<FargoSoulsPlayer>().VortexStealth))
                        {
                            continue;
                        }

                        Vector2 dir = Projectile.position - proj.Center;
                        proj.velocity = dir.SafeNormalize(Vector2.Zero) * 8;

                        if (Projectile.timeLeft <= 30 && distance < minDist / 4 && proj.minionSlots == 0)
                        {
                            proj.active = false;
                        }
                    }

                    timer = 10;
                }

                timer--;

            }
            /*else
            {
                Projectile.scale = (float)(1.0 - (Projectile.ai[0] - time) / 60.0) * maxScale;
                Projectile.alpha = 255 - (int)(255 * Projectile.scale / maxScale);
                Projectile.rotation = Projectile.rotation - (float)Math.PI / 30f;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
                for (int index = 0; index < 2; ++index)
                {
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            Vector2 spinningpoint1 = Vector2.UnitY.RotatedByRandom(6.28318548202515) * Projectile.scale;
                            Dust dust1 = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint1 * 30f, 0, 0, 229, 0.0f, 0.0f, 0, new Color(), 1f)];
                            dust1.noGravity = true;
                            dust1.position = Projectile.Center - spinningpoint1 * Main.rand.Next(10, 21);
                            dust1.velocity = spinningpoint1.RotatedBy(1.57079637050629, new Vector2()) * 6f;
                            dust1.scale = 0.5f + Main.rand.NextFloat();
                            dust1.fadeIn = 0.5f;
                            dust1.customData = Projectile.Center;
                            break;
                        case 1:
                            Vector2 spinningpoint2 = Vector2.UnitY.RotatedByRandom(6.28318548202515) * Projectile.scale;
                            Dust dust2 = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint2 * 30f, 0, 0, 240, 0.0f, 0.0f, 0, new Color(), 1f)];
                            dust2.noGravity = true;
                            dust2.position = Projectile.Center - spinningpoint2 * 30f;
                            dust2.velocity = spinningpoint2.RotatedBy(-1.57079637050629, new Vector2()) * 3f;
                            dust2.scale = 0.5f + Main.rand.NextFloat();
                            dust2.fadeIn = 0.5f;
                            dust2.customData = Projectile.Center;
                            break;
                    }
                }
            }*/

            Dust dust3 = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 229, 0f, 0f, 0, new Color(), 1f)];
            dust3.velocity *= 5f;
            dust3.fadeIn = 1f;
            dust3.scale = 1f + Main.rand.NextFloat() + Main.rand.Next(4) * 0.3f;
            dust3.noGravity = true;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);
            int type = 229;
            for (int index = 0; index < 80; ++index)
            {
                Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, type, 0.0f, 0.0f, 0, new Color(), 1f)];
                dust.velocity *= 10f;
                dust.fadeIn = 1f;
                dust.scale = 1 + Main.rand.NextFloat() + Main.rand.Next(4) * 0.3f;
                if (Main.rand.Next(3) != 0)
                {
                    dust.noGravity = true;
                    dust.velocity *= 3f;
                    dust.scale *= 2f;
                }
            }

            if (Projectile.localAI[0] == 0 && Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Projectile.Distance(Main.npc[i].Center) < 850) //strip nearby enemies of iframes
                        Main.npc[i].immune[Projectile.owner] = 0;
                }

                Projectile.localAI[0] = 1;
                Projectile.position = Projectile.Center;
                Projectile.width = 600;
                Projectile.height = 600;
                Projectile.Center = Projectile.position;

                Projectile.damage *= 10;
                Projectile.knockBack *= 10;
                Projectile.Damage();
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.Black * Projectile.Opacity, -Projectile.rotation, origin2, Projectile.scale * 1.25f, SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}