using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.AbomBoss
{
    public class AbomRocket : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_448";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rocket");
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.alpha = 0;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.scale = 2.5f;
            CooldownSlot = 1;
        }

        public override void AI()
        {
            if (Projectile.ai[1] > 0) //when first spawned just move straight
            {
                Projectile.timeLeft++; //don't expire while counting down

                if (--Projectile.ai[1] == 0) //do for one tick right before homing
                {
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity) * (Projectile.velocity.Length() + 6f);
                    Projectile.netUpdate = true;
                    for (int index1 = 0; index1 < 36; ++index1)
                    {
                        Vector2 vector2 = (Vector2.UnitX * -8f + -Vector2.UnitY.RotatedBy(index1 * 3.14159274101257 / 36 * 2, new Vector2()) * new Vector2(2f, 8f)).RotatedBy(Projectile.rotation - (float)Math.PI / 2, new Vector2());
                        int index2 = Dust.NewDust(Projectile.Center, 0, 0, DustID.GoldFlame, 0.0f, 0.0f, 0, new Color(), 1f);
                        Main.dust[index2].scale = 1f;
                        Main.dust[index2].noGravity = true;
                        Main.dust[index2].position = Projectile.Center + vector2 * 6f;
                        Main.dust[index2].velocity = Projectile.velocity * 0.0f;
                    }
                }
            }
            else //start homing
            {
                if (Projectile.ai[0] >= 0 && Projectile.ai[0] < Main.maxPlayers) //have target
                {
                    if (--Projectile.ai[1] > -45) //only home for a bit
                    {
                        double num4 = (Main.player[(int)Projectile.ai[0]].Center - Projectile.Center).ToRotation() - (double)Projectile.velocity.ToRotation();
                        if (num4 > Math.PI)
                            num4 -= 2.0 * Math.PI;
                        if (num4 < -1.0 * Math.PI)
                            num4 += 2.0 * Math.PI;
                        Projectile.velocity = Projectile.velocity.RotatedBy(num4 * 0.05f, new Vector2());
                    }
                }
                else //retarget
                {
                    Projectile.ai[0] = Player.FindClosest(Projectile.Center, 0, 0);
                }

                Projectile.tileCollide = true;
                if (++Projectile.localAI[0] > 10)
                {
                    Projectile.localAI[0] = 0f;
                    for (int index1 = 0; index1 < 36; ++index1)
                    {
                        Vector2 vector2 = (Vector2.UnitX * -8f + -Vector2.UnitY.RotatedBy(index1 * 3.14159274101257 / 36 * 2, new Vector2()) * new Vector2(2f, 4f)).RotatedBy(Projectile.rotation - (float)Math.PI / 2, new Vector2());
                        int index2 = Dust.NewDust(Projectile.Center, 0, 0, DustID.GoldFlame, 0.0f, 0.0f, 0, new Color(), 1f);
                        Main.dust[index2].scale = 1f;
                        Main.dust[index2].noGravity = true;
                        Main.dust[index2].position = Projectile.Center + vector2 * 6f;
                        Main.dust[index2].velocity = Projectile.velocity * 0.0f;
                    }
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2;

            Vector2 vector21 = Vector2.UnitY.RotatedBy(Projectile.rotation, new Vector2()) * 8f * 2;
            int index21 = Dust.NewDust(Projectile.Center, 0, 0, DustID.GoldFlame, 0.0f, 0.0f, 0, new Color(), 1f);
            Main.dust[index21].position = Projectile.Center + vector21;
            Main.dust[index21].scale = 1f;
            Main.dust[index21].noGravity = true;

            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                    Projectile.frame = 0;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<Buffs.Boss.AbomFangBuff>(), 300);
                //target.AddBuff(ModContent.BuffType<Defenseless>(), 300);
                //target.AddBuff(BuffID.Confused, 180);
            }
            target.AddBuff(BuffID.BrokenArmor, 300);
            Projectile.timeLeft = 0;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 112;
            Projectile.position.X -= Projectile.width / 2;
            Projectile.position.Y -= Projectile.height / 2;
            for (int index = 0; index < 4; ++index)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0.0f, 0.0f, 100, new Color(), 1.5f);
            for (int index1 = 0; index1 < 40; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame, 0.0f, 0.0f, 0, new Color(), 2.5f);
                Main.dust[index2].noGravity = true;
                Dust dust1 = Main.dust[index2];
                dust1.velocity *= 3f;
                int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Dust dust2 = Main.dust[index3];
                dust2.velocity *= 2f;
                Main.dust[index3].noGravity = true;
            }
            for (int index1 = 0; index1 < 1; ++index1)
            {
                int index2 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position + new Vector2(Projectile.width * Main.rand.Next(100) / 100f, Projectile.height * Main.rand.Next(100) / 100f) - Vector2.One * 10f, new Vector2(), Main.rand.Next(61, 64), 1f);
                Gore gore = Main.gore[index2];
                gore.velocity *= 0.3f;
                Main.gore[index2].velocity.X += Main.rand.Next(-10, 11) * 0.05f;
                Main.gore[index2].velocity.Y += Main.rand.Next(-10, 11) * 0.05f;
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
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}