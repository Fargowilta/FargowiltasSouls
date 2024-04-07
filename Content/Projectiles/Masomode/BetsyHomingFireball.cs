using FargowiltasSouls.Core;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Will
{
    public class BetsyHomingFireball : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_711";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fireball");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = 600;
            Projectile.alpha = 60;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            //CooldownSlot = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2;

            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item34, Projectile.position);
            }
            else if (Projectile.ai[1] == 1f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int num2 = -1;
                float num3 = 2000f;
                for (int k = 0; k < 255; k++)
                {
                    if (Main.player[k].active && !Main.player[k].dead)
                    {
                        Vector2 center = Main.player[k].Center;
                        float num4 = Vector2.Distance(center, Projectile.Center);
                        if ((num4 < num3 || num2 == -1) && Collision.CanHit(Projectile.Center, 1, 1, center, 1, 1))
                        {
                            num3 = num4;
                            num2 = k;
                        }
                    }
                }
                if (num3 < 20f)
                {
                    Projectile.Kill();
                    return;
                }
                if (num2 != -1)
                {
                    Projectile.ai[1] = 21f;
                    Projectile.ai[0] = num2;
                    Projectile.netUpdate = true;
                }
            }
            else if (Projectile.ai[1] > 20f && Projectile.ai[1] < 200f)
            {
                Projectile.ai[1] += 1f;
                int num5 = (int)Projectile.ai[0];
                if (!Main.player[num5].active || Main.player[num5].dead)
                {
                    Projectile.ai[1] = 1f;
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                else
                {
                    float num6 = Projectile.velocity.ToRotation();
                    Vector2 vector = Main.player[num5].Center - Projectile.Center;
                    if (vector.Length() < 20f)
                    {
                        Projectile.Kill();
                        return;
                    }
                    float targetAngle = vector.ToRotation();
                    if (vector == Vector2.Zero)
                    {
                        targetAngle = num6;
                    }
                    float num7 = num6.AngleLerp(targetAngle, 0.008f);
                    Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0f).RotatedBy(num7);
                }
            }
            if (Projectile.ai[1] >= 1f && Projectile.ai[1] < 20f)
            {
                Projectile.ai[1] += 1f;
                if (Projectile.ai[1] == 20f)
                {
                    Projectile.ai[1] = 1f;
                }
            }

            //Lighting.AddLight(Projectile.Center, 1.1f, 0.9f, 0.4f);
            bool recolor = WorldSavingSystem.EternityMode && SoulConfig.Instance.BossRecolors;
            int dustID = recolor ? DustID.Shadowflame : DustID.Torch;

            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] == 12f)
            {
                Projectile.localAI[0] = 0f;
                for (int l = 0; l < 12; l++)
                {
                    Vector2 spinningpoint = Vector2.UnitX * -Projectile.width / 2f;
                    spinningpoint += -Vector2.UnitY.RotatedBy((float)l * (float)Math.PI / 6f) * new Vector2(8f, 16f);
                    spinningpoint = spinningpoint.RotatedBy(Projectile.rotation - (float)Math.PI / 2f);
                    int num8 = Dust.NewDust(Projectile.Center, 0, 0, dustID, 0f, 0f, 160);
                    Main.dust[num8].scale = 1.1f;
                    Main.dust[num8].noGravity = true;
                    Main.dust[num8].position = Projectile.Center + spinningpoint;
                    Main.dust[num8].velocity = Projectile.velocity * 0.1f;
                    Main.dust[num8].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[num8].position) * 1.25f;
                }
            }
            if (Main.rand.NextBool(4))
            {
                for (int m = 0; m < 1; m++)
                {
                    Vector2 vector2 = -Vector2.UnitX.RotatedByRandom(0.19634954631328583).RotatedBy(Projectile.velocity.ToRotation());
                    int num9 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100);
                    Main.dust[num9].velocity *= 0.1f;
                    Main.dust[num9].position = Projectile.Center + vector2 * Projectile.width / 2f;
                    Main.dust[num9].fadeIn = 0.9f;
                }
            }
            if (Main.rand.NextBool(32))
            {
                for (int n = 0; n < 1; n++)
                {
                    Vector2 vector3 = -Vector2.UnitX.RotatedByRandom(0.39269909262657166).RotatedBy(Projectile.velocity.ToRotation());
                    int num10 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 155, default(Color), 0.8f);
                    Main.dust[num10].velocity *= 0.3f;
                    Main.dust[num10].position = Projectile.Center + vector3 * Projectile.width / 2f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num10].fadeIn = 1.4f;
                    }
                }
            }
            if (Main.rand.NextBool(2))
            {
                for (int num11 = 0; num11 < 2; num11++)
                {
                    Vector2 vector4 = -Vector2.UnitX.RotatedByRandom(0.7853981852531433).RotatedBy(Projectile.velocity.ToRotation());
                    int num12 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustID, 0f, 0f, 0, default(Color), 1.2f);
                    Main.dust[num12].velocity *= 0.3f;
                    Main.dust[num12].noGravity = true;
                    Main.dust[num12].position = Projectile.Center + vector4 * Projectile.width / 2f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num12].fadeIn = 1.4f;
                    }
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            bool recolor = WorldSavingSystem.EternityMode && SoulConfig.Instance.BossRecolors;
            int dustID = recolor ? DustID.Shadowflame : DustID.Torch;

            for (int i = 0; i < 30; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 3f);
                Main.dust[dust].velocity *= 1.4f;
            }

            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, dustID, 0f, 0f, 100, default, 3.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, dustID, 0f, 0f, 100, default, 1.5f);
                Main.dust[dust].velocity *= 3f;
            }

            float scaleFactor9 = 0.5f;
            for (int j = 0; j < 4; j++)
            {
                int gore = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                    default,
                    Main.rand.Next(61, 64));

                Main.gore[gore].velocity *= scaleFactor9;
                Main.gore[gore].velocity.X += 1f;
                Main.gore[gore].velocity.Y += 1f;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.betsyBoss, NPCID.DD2Betsy))
            {
                if (WorldSavingSystem.EternityMode)
                {
                    //target.AddBuff(BuffID.OnFire, 600);
                    //target.AddBuff(BuffID.Ichor, 600);
                    target.AddBuff(BuffID.WitheredArmor, Main.rand.Next(60, 300));
                    target.AddBuff(BuffID.WitheredWeapon, Main.rand.Next(60, 300));
                    target.AddBuff(BuffID.Burning, 300);
                }
            }
            Projectile.timeLeft = 0;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.betsyBoss, NPCID.DD2Betsy) && WorldSavingSystem.EternityMode && SoulConfig.Instance.BossRecolors)
            {
                texture2D13 = TextureAssets.Projectile[ProjectileID.DD2BetsyFireball].Value;
            }
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = Color.White;
            color26 = Projectile.GetAlpha(color26);
            color26.A = (byte)Projectile.alpha;

            SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                float lerpamount = 0;
                if (i > 3 && i < 5)
                    lerpamount = 0.6f;
                if (i >= 5)
                    lerpamount = 0.8f;

                Color color27 = Color.Lerp(Color.White, Color.Purple, lerpamount) * 0.75f * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                float scale = Projectile.scale * (ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}