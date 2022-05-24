using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class StyxScythe : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_274";

        public int rotationDirection;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Styx Scythe");
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 420;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                rotationDirection = Math.Sign(Projectile.ai[1]);
                SoundEngine.PlaySound(SoundID.Item71, Projectile.Center);
            }

            if (Projectile.localAI[0] == 1) //extend out, locked to move around player
            {
                Projectile.ai[0] += Projectile.velocity.Length();
                Projectile.Center = Main.player[Projectile.owner].Center + Vector2.Normalize(Projectile.velocity) * Projectile.ai[0];

                if (Projectile.Distance(Main.player[Projectile.owner].Center) > Math.Abs(Projectile.ai[1]))
                {
                    Projectile.localAI[0]++;
                    Projectile.localAI[1] = Math.Sign(Projectile.ai[1]);
                    Projectile.ai[0] = Math.Abs(Projectile.ai[1]) - Projectile.velocity.Length();
                    Projectile.ai[1] = 0;
                    Projectile.netUpdate = true;
                }
            }
            else if (Projectile.localAI[0] == 2) //orbit player, please dont ask how this code works i dont know either
            {
                //Projectile.ai[0] += Projectile.velocity.Length();
                Projectile.Center = Main.player[Projectile.owner].Center + Vector2.Normalize(Projectile.velocity) * Projectile.ai[0];
                Projectile.Center += Projectile.velocity.RotatedBy(Math.PI / 2 * Projectile.localAI[1]);
                Projectile.velocity = Projectile.DirectionFrom(Main.player[Projectile.owner].Center) * Projectile.velocity.Length();

                if (++Projectile.ai[1] > 180)
                {
                    Projectile.localAI[0]++;
                    Projectile.localAI[1] = 0;
                    Projectile.ai[0] = 0;
                    Projectile.ai[1] = 0;
                    Projectile.netUpdate = true;
                }
            }
            else if (Projectile.timeLeft > 60) //now flying away, go into homing mode
            {
                if (Projectile.ai[0] >= 0 && Projectile.ai[0] < Main.maxNPCs)
                {
                    int ai0 = (int)Projectile.ai[0];
                    if (Main.npc[ai0].CanBeChasedBy())
                    {
                        double num4 = (Main.npc[ai0].Center - Projectile.Center).ToRotation() - Projectile.velocity.ToRotation();
                        if (num4 > Math.PI)
                            num4 -= 2.0 * Math.PI;
                        if (num4 < -1.0 * Math.PI)
                            num4 += 2.0 * Math.PI;
                        Projectile.velocity = Projectile.velocity.RotatedBy(num4 * 0.17f);
                    }
                    else
                    {
                        Projectile.ai[0] = -1f;
                        Projectile.netUpdate = true;
                    }
                }
                else
                {
                    if (++Projectile.localAI[1] > 12f)
                    {
                        Projectile.localAI[1] = 0f;
                        Projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 2000);
                        Projectile.netUpdate = true;
                    }
                }
            }

            Projectile.direction = Projectile.spriteDirection = rotationDirection;
            Projectile.rotation += Projectile.spriteDirection * 0.7f * rotationDirection;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, 0f, 0f, 0, default(Color), 2.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 4f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.ShadowFlame, 300);
            target.AddBuff(ModContent.BuffType<Buffs.Masomode.MutantNibble>(), 300);
            target.immune[Projectile.owner] = 6;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects spriteEffects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, spriteEffects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, spriteEffects, 0);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}