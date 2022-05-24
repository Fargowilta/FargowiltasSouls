using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class SparklingLove : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Items/Weapons/FinalUpgrades/SparklingLove";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sparkling Love");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            //Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;

            Projectile.aiStyle = -1;
            Projectile.scale = 2f;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Projectile.direction = Projectile.spriteDirection = Main.rand.NextBool() ? -1 : 1;

                if (Projectile.owner == Main.myPlayer)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Normalize(Projectile.velocity).RotatedBy(Math.PI / 2 * i),
                            ModContent.ProjectileType<SparklingLoveDeathray>(), Projectile.damage, Projectile.knockBack, Projectile.owner,
                            (float)Math.PI / 2 * 1.0717f * Projectile.direction, Projectile.identity);
                    }
                }
            }

            if (Projectile.ai[0] == 0)
            {
                if (Projectile.Distance(Main.player[Projectile.owner].Center) > 800)
                {
                    Projectile.ai[0] = 1;
                    Projectile.netUpdate = true;

                    if (Projectile.localAI[1] == 0)
                        Projectile.localAI[1] = 1;
                }
            }
            else
            {
                Projectile.extraUpdates = 0;
                Projectile.velocity = Projectile.DirectionTo(Main.player[Projectile.owner].Center) * (Projectile.velocity.Length() + 1f / 10f);

                if (Projectile.Distance(Main.player[Projectile.owner].Center) <= Projectile.velocity.Length())
                    Projectile.Kill();
            }

            if (Projectile.localAI[1] == 1)
            {
                Projectile.localAI[1] = 2;
                HeartBurst(Projectile.Center);
            }

            Projectile.rotation += Projectile.direction * -0.4f;

            for (int i = 0; i < 2; i++)
            {
                int num812 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    86, Projectile.velocity.X / 2, Projectile.velocity.Y / 2, 0, default(Color), 1.7f);
                Main.dust[num812].noGravity = true;
            }
        }

        private void HeartBurst(Vector2 spawnPos)
        {
            if (Projectile.owner != Main.myPlayer)
                return;

            SoundEngine.PlaySound(SoundID.Item21, spawnPos);
            for (int i = 0; i < 8; i++)
            {
                Vector2 vel = 14f * Vector2.Normalize(Projectile.velocity).RotatedBy(Math.PI / 4 * (i + 0.5));
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos, vel, ModContent.ProjectileType<SparklingLoveHeart>(), Projectile.damage, Projectile.knockBack, Projectile.owner, -1, 45);
                FargoSoulsUtil.HeartDust(spawnPos, vel.ToRotation(), vel);
            }

            /*for (int index1 = 0; index1 < 20; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 272, 0f, 0f, 100, new Color(), 2f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 7f * Projectile.scale;
                Main.dust[index2].noLight = true;
                int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 272, 0f, 0f, 100, new Color(), 1f);
                Main.dust[index3].velocity *= 4f * Projectile.scale;
                Main.dust[index3].noGravity = true;
                Main.dust[index3].noLight = true;
            }

            for (int i = 0; i < 60; i++) //warning dust ring
            {
                Vector2 vector6 = Vector2.UnitY * 5f * Projectile.scale;
                vector6 = vector6.RotatedBy((i - (60 / 2 - 1)) * 6.28318548f / 60) + spawnPos;
                Vector2 vector7 = vector6 - spawnPos;
                int d = Dust.NewDust(vector6 + vector7, 0, 0, 86, 0f, 0f, 0, default(Color), 2.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = vector7;
            }*/
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.localAI[1] == 0)
            {
                Projectile.localAI[1] = 2;
                HeartBurst(target.Center);
            }

            target.AddBuff(BuffID.Lovestruck, 300);
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
            Texture2D texture2D14 = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Items/Weapons/FinalUpgrades/SparklingLove_glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Main.EntitySpriteDraw(texture2D14, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * Projectile.Opacity, Projectile.rotation, origin2, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}