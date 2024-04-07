using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class SparklingLoveHeart : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Masomode/FakeHeart";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Friend Heart");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.timeLeft = 300;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.aiStyle = -1;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.penetrate = 2;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            float rand = Main.rand.Next(90, 111) * 0.01f * (Main.essScale * 0.5f);
            Lighting.AddLight(Projectile.Center, 0.5f * rand, 0.1f * rand, 0.1f * rand);

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Projectile.ai[0] = -1;
            }

            if (Projectile.ai[0] >= 0 && Projectile.ai[0] < Main.maxNPCs)
            {
                int ai0 = (int)Projectile.ai[0];
                if (Main.npc[ai0].CanBeChasedBy() && Projectile.Distance(Main.npc[ai0].Center) > Math.Min(Main.npc[ai0].height, Main.npc[ai0].width) / 2)
                {
                    double num4 = (Main.npc[ai0].Center - Projectile.Center).ToRotation() - Projectile.velocity.ToRotation();
                    if (num4 > Math.PI)
                    {
                        num4 -= 2.0 * Math.PI;
                    }

                    if (num4 < -1.0 * Math.PI)
                    {
                        num4 += 2.0 * Math.PI;
                    }

                    Projectile.velocity = Projectile.velocity.RotatedBy(num4 * 0.3f);
                }
                else
                {
                    Projectile.ai[0] = -1f;
                    Projectile.ai[1] = 18f;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                if (--Projectile.ai[1] < 0f)
                {
                    Projectile.ai[1] = 18f;
                    Projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 1700f);
                    Projectile.netUpdate = true;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation() - (float)Math.PI / 2;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemAmethyst, 0f, 0f, 0, default, 2f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 8f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Lovestruck, 300);
            target.immune[Projectile.owner] = 6;

            /*if (Projectile.owner == Main.myPlayer)
            {
                int healAmount = 2;
                Main.player[Main.myPlayer].HealEffect(healAmount);
                Main.player[Main.myPlayer].statLife += healAmount;

                if (Main.player[Main.myPlayer].statLife > Main.player[Main.myPlayer].statLifeMax2)
                    Main.player[Main.myPlayer].statLife = Main.player[Main.myPlayer].statLifeMax2;
            }*/
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, lightColor.G, lightColor.B, lightColor.A);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
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
    }
}