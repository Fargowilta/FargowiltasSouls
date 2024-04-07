using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class PhantasmalFlail : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Phantasmal Flail");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 52;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            //Projectile.usesLocalNPCImmunity = true;
            //Projectile.localNPCHitCooldown = 0;
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 120) Projectile.ai[0] = 1f;

            if (Main.player[Projectile.owner].dead)
            {
                Projectile.Kill();
                return;
            }

            Main.player[Projectile.owner].itemAnimation = 5;
            Main.player[Projectile.owner].itemTime = 5;

            if (Projectile.alpha == 0)
            {
                if (Projectile.position.X + Projectile.width / 2 > Main.player[Projectile.owner].position.X + Main.player[Projectile.owner].width / 2)
                    Main.player[Projectile.owner].ChangeDir(1);
                else
                    Main.player[Projectile.owner].ChangeDir(-1);
            }

            Vector2 vector14 = new(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f);
            float num166 = Main.player[Projectile.owner].position.X + Main.player[Projectile.owner].width / 2 - vector14.X;
            float num167 = Main.player[Projectile.owner].position.Y + Main.player[Projectile.owner].height / 2 - vector14.Y;
            float distance = (float)Math.Sqrt(num166 * num166 + num167 * num167);
            if (Projectile.ai[0] == 0f)
            {
                if (distance > 600f) Projectile.ai[0] = 1f;
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;
                Projectile.ai[1] += 1f;
                if (Projectile.ai[1] > 8f) Projectile.ai[1] = 8f;
                if (Projectile.velocity.X < 0f)
                    Projectile.spriteDirection = -1;
                else
                    Projectile.spriteDirection = 1;

                /*if (Projectile.owner == Main.myPlayer)
                {
                    Vector2 speed = 10f * Vector2.Normalize(Projectile.velocity).RotatedBy(Math.PI / 2);
                    Projectile.NewProjectile(Projectile.Center, speed, ModContent.ProjectileType<PhantasmalBolt>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                    Projectile.NewProjectile(Projectile.Center, -speed, ModContent.ProjectileType<PhantasmalBolt>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                }*/
            }
            //plz retract sir
            else if (Projectile.ai[0] == 1f)
            {
                if (Projectile.localAI[1] == 0)
                {
                    Projectile.localAI[1] = 1f;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        const int max = 8;
                        const float dist = 100f;
                        const float rotation = 2f * (float)Math.PI / max;
                        for (int i = 0; i < max; i++)
                        {
                            Vector2 spawnPos = Projectile.Center + new Vector2(dist, 0f).RotatedBy(rotation * i);
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos, Vector2.Zero, ModContent.ProjectileType<PhantasmalSphere2>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                        }

                        const int boltMax = 32;
                        float rotationOffset = Main.rand.NextFloat(MathHelper.TwoPi);
                        for (int i = 0; i < boltMax; i++)
                        {
                            Vector2 speed = 10f * Vector2.UnitX.RotatedBy(rotationOffset + MathHelper.TwoPi / boltMax * i);
                            int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, speed, ModContent.ProjectileType<PhantasmalBolt>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                            if (p != Main.maxProjectiles)
                                Main.projectile[p].timeLeft /= 2;
                        }
                    }
                }

                Projectile.tileCollide = false;
                Projectile.rotation = (float)Math.Atan2(num167, num166) - 1.57f;
                float num169 = 30f;

                if (distance < 50f) Projectile.Kill();
                distance = num169 / distance;
                num166 *= distance;
                num167 *= distance;
                Projectile.velocity.X = num166 * 2;
                Projectile.velocity.Y = num167 * 2;
                if (Projectile.velocity.X < 0f)
                    Projectile.spriteDirection = 1;
                else
                    Projectile.spriteDirection = -1;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.penetrate < 0)
                target.immune[Projectile.owner] = 1;

            Projectile.penetrate = -1;
            Projectile.maxPenetrate = -1;

            int type = ModContent.ProjectileType<PhantasmalEyeLeashProj>();
            if (Main.player[Projectile.owner].ownedProjectileCounts[type] < 100)
            {
                int dist = 1200;
                for (int i = 0; i < 15; i++)
                {
                    Vector2 offset = new();
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    offset.X += (float)(Math.Sin(angle) * dist);
                    offset.Y += (float)(Math.Cos(angle) * dist);

                    Vector2 position = target.Center + offset - new Vector2(4, 4);
                    Vector2 velocity = Vector2.Normalize(target.Center - position) * 50;

                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, velocity,
                        type, Projectile.damage / 2, Projectile.knockBack, Projectile.owner, -10f);
                }
            }

            /*const int max = 10;
            for (int i = 0; i < max; i++)
            {
                Vector2 vector55 = Vector2.UnitX.RotatedBy(Math.PI * 2 / max * (i + Main.rand.NextDouble()));
                vector55 *= 50;
                int p = Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, vector55.X, vector55.Y,
                    ModContent.ProjectileType<PhantasmalEyeLeashProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner, -10f);
                if (p != Main.maxProjectiles)
                    Main.Projectile[p].penetrate = 3;
            }*/

            //retract
            Projectile.ai[0] = 1f;
        }

        // chain voodoo
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/BossWeapons/PhantasmalLeashFlailChain", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            Vector2 position = Projectile.Center;
            Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
            Rectangle? sourceRectangle = new Rectangle?();
            Vector2 origin = new(texture.Width * 0.5f, texture.Height * 0.5f);
            float num1 = texture.Height;
            Vector2 vector24 = mountedCenter - position;
            float rotation = (float)Math.Atan2(vector24.Y, vector24.X) - 1.57f;
            bool flag = true;
            if (float.IsNaN(position.X) && float.IsNaN(position.Y))
                flag = false;
            if (float.IsNaN(vector24.X) && float.IsNaN(vector24.Y))
                flag = false;
            while (flag)
                if (vector24.Length() < num1 + 1.0)
                {
                    flag = false;
                }
                else
                {
                    Vector2 vector21 = vector24;
                    vector21.Normalize();
                    position += vector21 * num1;
                    vector24 = mountedCenter - position;
                    Color color2 = Lighting.GetColor((int)position.X / 16, (int)(position.Y / 16.0));
                    color2 = Projectile.GetAlpha(color2);
                    Main.EntitySpriteDraw(texture, position - Main.screenPosition, sourceRectangle, color2, rotation, origin, 1f, SpriteEffects.None, 0);
                }


            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = SpriteEffects.None;

            for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 0.5f)
            {
                Color color27 = Color.White * Projectile.Opacity;
                color27.A = 0;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                int max0 = (int)i - 1;//Math.Max((int)i - 1, 0);
                if (max0 < 0)
                    continue;
                Vector2 value4 = Vector2.Lerp(Projectile.oldPos[(int)i], Projectile.oldPos[max0], 1 - i % 1);
                float num165 = MathHelper.Lerp(Projectile.oldRot[(int)i], Projectile.oldRot[max0], 1 - i % 1);
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}