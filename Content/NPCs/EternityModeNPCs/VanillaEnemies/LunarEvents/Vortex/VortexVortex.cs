using FargowiltasSouls.Content.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.LunarEvents.Vortex
{
    public class VortexVortex : ModProjectile
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
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60 * 60 * 60;
            Projectile.FargoSouls().DeletionImmuneRank = 2;
        }

        public override bool? CanDamage()
        {
            return Projectile.scale >= 2;
        }

        public override void AI()
        {
            if (!Main.npc.Any(n => n.active && n.type == NPCID.LunarTowerVortex))
                Projectile.Kill();

            int time = 360;
            int maxScale = 3;

            if (Projectile.ai[1] == 0)
                time = 30;

            void Suck()
            {
                Player player = Main.LocalPlayer;
                if (player.active && !player.dead && !player.ghost && Projectile.Center != player.Center && Projectile.Distance(player.Center) < 2000)
                {
                    float dragSpeed = Projectile.Distance(player.Center) / 60;
                    player.position += Projectile.DirectionFrom(player.Center) * dragSpeed;
                    player.AddBuff(ModContent.BuffType<LowGroundBuff>(), 2);
                    player.wingTime = 60;
                }
            };

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
                Projectile.scale = (Projectile.ai[0] - 50) / 40 * maxScale;
                Projectile.alpha = 255 - (int)(255 * Projectile.scale / maxScale);
                Projectile.rotation = Projectile.rotation - 0.1570796f;
                if (Main.rand.NextBool())
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515) * Projectile.scale;
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
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515) * Projectile.scale;
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint * 30f, 0, 0, DustID.Granite, 0.0f, 0.0f, 0, new Color(), 1f)];
                    dust.noGravity = true;
                    dust.position = Projectile.Center - spinningpoint * 30f;
                    dust.velocity = spinningpoint.RotatedBy(-(float)Math.PI / 2, new Vector2()) * 3f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                    dust.customData = Projectile.Center;
                }

                if (Projectile.ai[1] != 0)
                    Suck();

                int p = Player.FindClosest(Projectile.Center, 0, 0);
                if (p != -1)
                {
                    Projectile.localAI[1] =
                        Projectile.Center == Main.player[p].Center ? 0 : Projectile.SafeDirectionTo(Main.player[p].Center).ToRotation();
                    Projectile.localAI[1] += (float)Math.PI * 2 / 3 / 2;
                }
            }
            else
            {
                Projectile.scale = maxScale;
                Projectile.alpha = 0;
                Projectile.rotation = Projectile.rotation - (float)Math.PI / 60f;
                if (Main.rand.NextBool())
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515) * Projectile.scale;
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint * 30f, 0, 0, DustID.Vortex, 0.0f, 0.0f, 0, new Color(), 1f)];
                    dust.noGravity = true;
                    dust.position = Projectile.Center - spinningpoint * Main.rand.Next(10, 21);
                    dust.velocity = spinningpoint.RotatedBy((float)Math.PI / 2, new Vector2()) * 6f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                    dust.customData = Projectile.Center;
                }

                if (Projectile.ai[1] != 0)
                {
                    Suck();

                    int projTime = 4;
                    if (Projectile.localAI[0] % projTime == 0) //shoot lightning out, rotate 48 degrees per second by default
                    {
                        //Projectile.localAI[0] = 0;

                        SoundEngine.PlaySound(SoundID.Item82, Projectile.Center);

                        if (FargoSoulsUtil.HostCheck)
                        {
                            const int Distance = 1600;
                            const int max = 1;
                            for (int i = 0; i < max; i++)
                            {
                                Vector2 pos = Projectile.Center + (Vector2.UnitX * Distance).RotatedByRandom(MathHelper.TwoPi);
                                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), pos, Vector2.Zero, ModContent.ProjectileType<VortexVortexProjectile>(),
                                    Projectile.damage, 3f, Main.myPlayer, ai0: Projectile.whoAmI);
                            }
                        }

                        Projectile.localAI[1] += MathHelper.ToRadians(48f / 60f * projTime) * Projectile.ai[1];
                    }

                    Projectile.localAI[0]++;
                }
            }
            if (Projectile.localAI[0] % 30 == 0) //anti lag
            {
                if (NPC.CountNPCS(NPCID.LunarTowerVortex) <= 0)
                {
                    Projectile.scale = (float)(1.0 - (Projectile.ai[0] - time) / 60.0) * maxScale;
                    Projectile.alpha = 255 - (int)(255 * Projectile.scale / maxScale);
                    Projectile.rotation = Projectile.rotation - (float)Math.PI / 30f;
                    if (Projectile.alpha >= 255)
                        Projectile.Kill();

                    Vector2 spinningpoint1 = Vector2.UnitY.RotatedByRandom(6.28318548202515) * Projectile.scale;
                    Dust dust1 = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint1 * 30f, 0, 0, DustID.Vortex, 0.0f, 0.0f, 0, new Color(), 1f)];
                    dust1.noGravity = true;
                    dust1.position = Projectile.Center - spinningpoint1 * Main.rand.Next(10, 21);
                    dust1.velocity = spinningpoint1.RotatedBy((float)Math.PI / 2, new Vector2()) * 6f;
                    dust1.scale = 0.5f + Main.rand.NextFloat();
                    dust1.fadeIn = 0.5f;
                    dust1.customData = Projectile.Center;
                }
            }

            if (Main.rand.NextBool())
            {
                Dust dust3 = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, 0f, 0f, 0, new Color(), 1f)];
                dust3.velocity *= 5f;
                dust3.fadeIn = 1f;
                dust3.scale = 1f + Main.rand.NextFloat() + Main.rand.Next(4) * 0.3f;
                dust3.noGravity = true;
            }

            float num1 = 0.5f;
            for (int i = 0; i < 5; ++i)
            {
                if (Main.rand.NextFloat() >= num1)
                {
                    float f = Main.rand.NextFloat() * 6.283185f;
                    float num2 = Main.rand.NextFloat();
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + f.ToRotationVector2() * (110 + 600 * num2), 229, (f - 3.141593f).ToRotationVector2() * (14 + 8 * num2), 0, default, 1f);
                    dust.scale = 0.9f;
                    dust.fadeIn = 1.15f + num2 * 0.3f;
                    //dust.color = new Color(1f, 1f, 1f, num1) * (1f - num1);
                    dust.noGravity = true;
                    //dust.noLight = true;
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Electrified, 360);
            //if (WorldSavingSystem.MasochistMode) target.AddBuff(ModContent.BuffType<Buffs.Masomode.LightningRod>(), 360);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);
            int type = 229;
            for (int index = 0; index < 80; ++index)
            {
                Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, type, 0.0f, 0.0f, 0, new Color(), 1f)];
                dust.velocity *= 10f;
                dust.fadeIn = 1f;
                dust.scale = 1 + Main.rand.NextFloat() + Main.rand.Next(4) * 0.3f;
                if (!Main.rand.NextBool(3))
                {
                    dust.noGravity = true;
                    dust.velocity *= 3f;
                    dust.scale *= 2f;
                }
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
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.Black * Projectile.Opacity, -Projectile.rotation, origin2, Projectile.scale * 1.25f, SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}