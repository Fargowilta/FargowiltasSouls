using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class NatureIcicle : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Souls/FrostIcicle";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nature Icicle");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = 600;

            Projectile.scale = 1.5f;
            Projectile.hide = true;
            CooldownSlot = 1;
            Projectile.tileCollide = false;
            Projectile.coldDamage = true;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = Main.rand.NextBool() ? 1 : -1;
                Projectile.rotation = Main.rand.NextFloat(0, (float)Math.PI * 2);
                Projectile.hide = false;
            }

            if (--Projectile.ai[0] > 0)
            {
                Projectile.tileCollide = false;
                Projectile.rotation += Projectile.velocity.Length() * .1f * Projectile.localAI[0];
            }
            else if (Projectile.ai[0] == 0)
            {
                int p = Player.FindClosest(Projectile.Center, 0, 0);
                if (p != -1)
                {
                    Projectile.velocity = Projectile.DirectionTo(Main.player[p].Center) * 30;
                    Projectile.netUpdate = true;

                    if (Projectile.ai[1] > 0)
                    {
                        float rotation = MathHelper.ToRadians(20) + Main.rand.NextFloat(MathHelper.ToRadians(30));
                        if (Main.rand.NextBool())
                            rotation *= -1;
                        Projectile.velocity = Projectile.velocity.RotatedBy(rotation);
                    }

                    SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
                }
            }
            else
            {
                if (!Projectile.tileCollide && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                    Projectile.tileCollide = true;

                if (Projectile.velocity != Vector2.Zero)
                    Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2;
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);

            for (int index1 = 0; index1 < 20; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 92, 0.0f, 0.0f, 0, new Color(), 1f);
                if (Main.rand.Next(3) != 0)
                {
                    Dust dust1 = Main.dust[index2];
                    dust1.velocity = dust1.velocity * 2f;
                    Main.dust[index2].noGravity = true;
                    Dust dust2 = Main.dust[index2];
                    dust2.scale = dust2.scale * 1.75f;
                }
                else
                {
                    Dust dust = Main.dust[index2];
                    dust.scale = dust.scale * 0.5f;
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(BuffID.Chilled, 300);
            target.AddBuff(BuffID.Frostburn, 300);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
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

            SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = Color.White * Projectile.Opacity * 0.75f * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}