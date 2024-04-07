using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles
{
    public class LightningVortex : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_578";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Vortex");
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1]);
            if (npc == null || !npc.CanBeChasedBy())
            {
                Projectile.ai[1] = FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 1000);
                Projectile.netUpdate = true;
            }

            Projectile.ai[0]++;
            if (Projectile.ai[0] <= 50)
            {
                if (Main.rand.NextBool(4))
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515);
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint * 30f, 0, 0, DustID.Vortex, 0.0f, 0.0f, 0, new Color(), 1f)];
                    dust.noGravity = true;
                    dust.position = Projectile.Center - spinningpoint * Main.rand.Next(10, 21);
                    dust.velocity = spinningpoint.RotatedBy((float)Math.PI / 2, new Vector2()) * 4f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                }
                if (Main.rand.NextBool(4))
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515);
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint * 30f, 0, 0, DustID.Granite, 0.0f, 0.0f, 0, new Color(), 1f)];
                    dust.noGravity = true;
                    dust.position = Projectile.Center - spinningpoint * 30f;
                    dust.velocity = spinningpoint.RotatedBy(-(float)Math.PI / 2, new Vector2()) * 2f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                }
            }
            else if (Projectile.ai[0] <= 90)
            {
                Projectile.scale = (Projectile.ai[0] - 50) / 40;
                Projectile.alpha = 255 - (int)(255 * Projectile.scale);
                Projectile.rotation = Projectile.rotation - 0.1570796f;
                if (Main.rand.NextBool())
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515);
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint * 30f, 0, 0, DustID.Vortex, 0.0f, 0.0f, 0, new Color(), 1f)];
                    dust.noGravity = true;
                    dust.position = Projectile.Center - spinningpoint * Main.rand.Next(10, 21);
                    dust.velocity = spinningpoint.RotatedBy((float)Math.PI / 2, new Vector2()) * 6f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                    dust.customData = Projectile.Center;
                }
                if (Main.rand.NextBool())
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515);
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint * 30f, 0, 0, DustID.Granite, 0.0f, 0.0f, 0, new Color(), 1f)];
                    dust.noGravity = true;
                    dust.position = Projectile.Center - spinningpoint * 30f;
                    dust.velocity = spinningpoint.RotatedBy(-(float)Math.PI / 2, new Vector2()) * 3f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                    dust.customData = Projectile.Center;
                }

                /*Vector2 rotationVector2 = Main.npc[ai1].Center - Projectile.Center;
                rotationVector2.Normalize(); //Projectile.ai[1].ToRotationVector2();
                Vector2 vector2_1 = rotationVector2.RotatedBy((float)Math.PI / 2, new Vector2()) * (Main.rand.NextBool()).ToDirectionInt() * (float)Main.rand.Next(10, 21);
                Vector2 vector2_2 = (rotationVector2 * Main.rand.Next(-80, 81) - vector2_1) / 10f;
                int Type = Utils.SelectRandom<int>(Main.rand, new int[2] { 229, 229 });
                Dust d = Main.dust[Dust.NewDust(Projectile.Center, 0, 0, Type, 0.0f, 0.0f, 0, new Color(), 1f)];
                d.noGravity = true;
                d.position = Projectile.Center + vector2_1;
                d.velocity = vector2_2;
                d.scale = 0.5f + Main.rand.NextFloat();
                d.fadeIn = 0.5f;*/
                if (Projectile.ai[0] == 90 && Projectile.ai[1] != -1 && FargoSoulsUtil.HostCheck && npc != null)
                {
                    Vector2 rotationVector2 = npc.Center - Projectile.Center;
                    rotationVector2.Normalize();

                    Vector2 vector2_3 = rotationVector2 * 8f;
                    float ai_1 = Main.rand.Next(80);

                    int p = Player.FindClosest(Projectile.Center, 0, 0);
                    if (p != -1 && Main.player[p].active && !Main.player[p].dead && !Main.player[p].ghost && Projectile.Distance(Main.player[p].Center) < 1000)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X - vector2_3.X, Projectile.Center.Y - vector2_3.Y, vector2_3.X, vector2_3.Y,
                          ModContent.ProjectileType<LightningArc>(), Projectile.damage, Projectile.knockBack, Projectile.owner,
                          rotationVector2.ToRotation(), ai_1);
                    }
                }
            }
            else if (Projectile.ai[0] <= 120)
            {
                Projectile.scale = 1f;
                Projectile.alpha = 0;
                Projectile.rotation = Projectile.rotation - (float)Math.PI / 60f;
                if (Main.rand.NextBool())
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515);
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint * 30f, 0, 0, DustID.Vortex, 0.0f, 0.0f, 0, new Color(), 1f)];
                    dust.noGravity = true;
                    dust.position = Projectile.Center - spinningpoint * Main.rand.Next(10, 21);
                    dust.velocity = spinningpoint.RotatedBy((float)Math.PI / 2, new Vector2()) * 6f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                    dust.customData = Projectile.Center;
                }
                else
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515);
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint * 30f, 0, 0, DustID.Granite, 0.0f, 0.0f, 0, new Color(), 1f)];
                    dust.noGravity = true;
                    dust.position = Projectile.Center - spinningpoint * 30f;
                    dust.velocity = spinningpoint.RotatedBy(-(float)Math.PI / 2, new Vector2()) * 3f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                    dust.customData = Projectile.Center;
                }
            }
            else
            {
                Projectile.scale = (float)(1.0 - (Projectile.ai[0] - 120.0) / 60.0);
                Projectile.alpha = 255 - (int)(255 * Projectile.scale);
                Projectile.rotation = Projectile.rotation - (float)Math.PI / 30f;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
                for (int index = 0; index < 2; ++index)
                {
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            Vector2 spinningpoint1 = Vector2.UnitY.RotatedByRandom(6.28318548202515) * Projectile.scale;
                            Dust dust1 = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint1 * 30f, 0, 0, DustID.Vortex, 0.0f, 0.0f, 0, new Color(), 1f)];
                            dust1.noGravity = true;
                            dust1.position = Projectile.Center - spinningpoint1 * Main.rand.Next(10, 21);
                            dust1.velocity = spinningpoint1.RotatedBy((float)Math.PI / 2, new Vector2()) * 6f;
                            dust1.scale = 0.5f + Main.rand.NextFloat();
                            dust1.fadeIn = 0.5f;
                            dust1.customData = Projectile.Center;
                            break;
                        case 1:
                            Vector2 spinningpoint2 = Vector2.UnitY.RotatedByRandom(6.28318548202515) * Projectile.scale;
                            Dust dust2 = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint2 * 30f, 0, 0, DustID.Granite, 0.0f, 0.0f, 0, new Color(), 1f)];
                            dust2.noGravity = true;
                            dust2.position = Projectile.Center - spinningpoint2 * 30f;
                            dust2.velocity = spinningpoint2.RotatedBy(-(float)Math.PI / 2, new Vector2()) * 3f;
                            dust2.scale = 0.5f + Main.rand.NextFloat();
                            dust2.fadeIn = 0.5f;
                            dust2.customData = Projectile.Center;
                            break;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}