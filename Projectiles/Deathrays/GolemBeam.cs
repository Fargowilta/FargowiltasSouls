using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode;
using FargowiltasSouls.EternityMode.Content.Boss.HM;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Deathrays
{
    public class GolemBeam : BaseDeathray
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Deathrays/GolemBeam";
        public GolemBeam() : base(300) { }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Blazing Deathray");
        }

        public override void AI()
        {
            Projectile.alpha = 0;

            Vector2? vector78 = null;
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], NPCID.GolemHeadFree);
            if (npc != null && npc.GetEModeNPCMod<GolemHead>().DoAttack)
            {
                Projectile.Center = npc.Center;
            }
            else
            {
                Projectile.Kill();
                return;
            }
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            if (Projectile.localAI[0] == 0f)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Sounds/GolemBeam"), Projectile.Center);
            }
            float num801 = 1.3f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= maxTime)
            {
                Projectile.Kill();
                return;
            }
            Projectile.scale = num801;
            float num804 = Projectile.velocity.ToRotation();
            //num804 += Projectile.ai[0];
            Projectile.rotation = num804 - 1.57079637f;
            Projectile.velocity = num804.ToRotationVector2();
            float num805 = 3f;
            float num806 = (float)Projectile.width;
            Vector2 samplingPoint = Projectile.Center;
            if (vector78.HasValue)
            {
                samplingPoint = vector78.Value;
            }
            float[] array3 = new float[(int)num805];
            Collision.LaserScan(samplingPoint, Projectile.velocity, num806 * Projectile.scale, 2400f, array3);
            float num807 = 0f;
            int num3;
            for (int num808 = 0; num808 < array3.Length; num808 = num3 + 1)
            {
                num807 += array3[num808];
                num3 = num808;
            }
            num807 /= num805;

            Vector2 tip = Projectile.Center + Projectile.velocity * Projectile.localAI[1];

            if (Projectile.localAI[0] <= descendTime)
            {
                float targetLength = Math.Max(num807, 250 + 70);
                Projectile.localAI[1] = MathHelper.Lerp(0, targetLength, Projectile.localAI[0] / descendTime);

                if (++Projectile.frameCounter > 3)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= Main.projFrames[Projectile.type])
                        Projectile.frame = 0;
                }

                if (Projectile.localAI[0] == descendTime && !Main.dedServ && Main.LocalPlayer.active)
                {
                    Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 60;
                    for (int i = -1; i <= 1; i += 2)
                    {
                        for (int j = 0; j < 50; j++)
                        {
                            int d = Dust.NewDust(tip, 0, 0, DustID.Smoke, i * 3f, 0f, 50, default, 3f);
                            Main.dust[d].noGravity = Main.rand.NextBool();
                            Main.dust[d].velocity *= Main.rand.NextFloat(3f);
                        }

                        for (int j = 0; j < 15; j++)
                        {
                            int gore = Gore.NewGore(Projectile.GetSource_FromThis(), tip, default(Vector2), Main.rand.Next(61, 64));
                            Main.gore[gore].velocity.X += j / 3f * i;
                            Main.gore[gore].velocity.Y += Main.rand.NextFloat(2f);
                        }
                    }
                }
            }

            for (int k = 0; k < 5; k++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, 0, 0f, 50, default, 4f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity += Projectile.velocity * 2f;
                Main.dust[d].velocity *= Main.rand.NextFloat(2f);
            }

            if (Collision.SolidTiles(tip + Projectile.velocity * 16f, 0, 0))
            {
                for (int i = 0; i < 2; i++)
                {
                    int d = Dust.NewDust(tip - new Vector2(Projectile.width, Projectile.height) / 2, Projectile.width, Projectile.height, DustID.Smoke, 0, 0f, 50, default, 4f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity -= Projectile.velocity * 3f;
                    Main.dust[d].velocity *= Main.rand.NextFloat(3f);
                }

                if (Main.rand.NextBool(3))
                {
                    int gore = Gore.NewGore(Projectile.GetSource_FromThis(), tip, default(Vector2), Main.rand.Next(61, 64), 0.5f);
                    Main.gore[gore].velocity -= Projectile.velocity * 3f;
                    Main.gore[gore].velocity *= Main.rand.NextFloat(2f);
                }
            }
        }

        const int descendTime = 30;

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Stunned>(), 120);

            target.AddBuff(BuffID.BrokenArmor, 600);
            target.AddBuff(ModContent.BuffType<Defenseless>(), 600);
            target.AddBuff(BuffID.WitheredArmor, 600);
        }

        Rectangle Frame(Texture2D tex)
        {
            int frameHeight = tex.Height / Main.projFrames[Projectile.type];
            return new Rectangle(0, frameHeight * Projectile.frame, tex.Width, frameHeight);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero)
            {
                return false;
            }

            SpriteEffects spriteEffects = SpriteEffects.None;

            Texture2D texture2D19 = ModContent.Request<Texture2D>(Texture, AssetRequestMode.ImmediateLoad).Value;
            Texture2D texture2D20 = ModContent.Request<Texture2D>($"{Texture}2", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texture2D21 = ModContent.Request<Texture2D>($"{Texture}3", AssetRequestMode.ImmediateLoad).Value;

            float rayLength = Projectile.localAI[1];
            Texture2D arg_ABD8_1 = texture2D19;
            Vector2 arg_ABD8_2 = Projectile.Center - Main.screenPosition;
            Rectangle sourceRectangle2 = texture2D19.Bounds;
            Main.EntitySpriteDraw(arg_ABD8_1, arg_ABD8_2, sourceRectangle2, Projectile.GetAlpha(lightColor), Projectile.rotation, sourceRectangle2.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            rayLength -= (texture2D19.Height / 2 + texture2D21.Height) * Projectile.scale;
            Vector2 value20 = Projectile.Center;
            value20 += Projectile.velocity * Projectile.scale * texture2D19.Height / 2f;
            if (rayLength > 0f)
            {
                float num224 = 0f;
                Rectangle rectangle7 = texture2D20.Bounds;
                while (num224 < rayLength)
                {
                    Main.EntitySpriteDraw(texture2D20, value20 - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(rectangle7), Projectile.GetAlpha(Lighting.GetColor((int)value20.X / 16, (int)value20.Y / 16)), Projectile.rotation, rectangle7.Size() / 2, Projectile.scale, spriteEffects, 0);
                    num224 += rectangle7.Height * Projectile.scale;
                    value20 += Projectile.velocity * rectangle7.Height * Projectile.scale;
                }
            }
            Texture2D arg_AE2D_1 = texture2D21;
            Vector2 arg_AE2D_2 = value20 - Main.screenPosition;
            sourceRectangle2 = texture2D21.Bounds;
            Main.EntitySpriteDraw(arg_AE2D_1, arg_AE2D_2, sourceRectangle2, Projectile.GetAlpha(Lighting.GetColor((int)value20.X / 16, (int)value20.Y / 16)), Projectile.rotation, sourceRectangle2.Size() / 2, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}