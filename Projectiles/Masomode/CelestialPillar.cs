using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class CelestialPillar : ModProjectile
    {
        private int target = -1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Celestial Pillar");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            CooldownSlot = 1;
            Projectile.penetrate = -1;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 1;
        }

        public override bool? CanDamage()
        {
            return Projectile.alpha == 0;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(target);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            target = reader.ReadInt32();
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                int type;
                switch ((int)Projectile.ai[0])
                {
                    case 0: type = 242; break; //nebula
                    case 1: type = 127; break; //solar
                    case 2: type = 229; break; //vortex
                    default: type = 135; break; //stardust
                }
                for (int index = 0; index < 50; ++index)
                {
                    Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, type, 0.0f, 0.0f, 0, new Color(), 1f)];
                    dust.velocity *= 10f;
                    dust.fadeIn = 1f;
                    dust.scale = 1 + Main.rand.NextFloat() + Main.rand.Next(4) * 0.3f;
                    if (Main.rand.Next(3) != 0)
                    {
                        dust.noGravity = true;
                        dust.velocity *= 3f;
                        dust.scale *= 2f;
                    }
                }
            }
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 2;
                if (Projectile.alpha <= 0)
                {
                    Projectile.alpha = 0;
                    if (target != -1)
                    {
                        SoundEngine.PlaySound(SoundID.Item89, Projectile.Center);
                        Projectile.velocity = Main.player[target].Center - Projectile.Center;
                        float distance = Projectile.velocity.Length();
                        Projectile.velocity.Normalize();
                        const float speed = 22f;
                        Projectile.velocity *= speed;
                        Projectile.timeLeft = (int)(distance / speed);
                        Projectile.netUpdate = true;
                        return;
                    }
                    else
                    {
                        Projectile.Kill();
                    }
                }

                Projectile.velocity.Y += 10f / 120f;
                Projectile.rotation += Projectile.velocity.Length() / 20f;

                if (target >= 0 && Main.player[target].active && !Main.player[target].dead)
                {
                    if (Projectile.alpha < 100)
                    {
                        Projectile.rotation = Projectile.rotation.AngleLerp(
                          (Main.player[target].Center - Projectile.Center).ToRotation(), (255 - Projectile.alpha) / 255f * 0.08f);
                    }
                }
                else
                {
                    int possibleTarget = -1;
                    float maxDistance = 9000f;
                    for (int i = 0; i < 255; i++)
                    {
                        if (Main.player[i].active && !Main.player[i].dead)
                        {
                            float distance = Projectile.Distance(Main.player[i].Center);
                            if (distance < maxDistance)
                            {
                                possibleTarget = i;
                                maxDistance = distance;
                            }
                        }
                    }
                    if (possibleTarget != -1)
                    {
                        target = possibleTarget;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            Projectile.frame = (int)Projectile.ai[0];
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (target.mount.Active)
                target.mount.Dismount(target);
            target.velocity.X = Projectile.velocity.X < 0 ? -15f : 15f;
            target.velocity.Y = -10f;
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 300);
            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 600);
            switch ((int)Projectile.ai[0])
            {
                case 0: target.AddBuff(ModContent.BuffType<ReverseManaFlow>(), 360); break; //nebula
                case 1: target.AddBuff(ModContent.BuffType<Atrophied>(), 360); break; //solar
                case 2: target.AddBuff(ModContent.BuffType<Jammed>(), 360); break; //vortex
                default: target.AddBuff(ModContent.BuffType<Antisocial>(), 360); break; //stardust
            }
            Projectile.timeLeft = 0;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);
            int type;
            switch ((int)Projectile.ai[0])
            {
                case 0: type = 242; break; //nebula
                case 1: type = 127; break; //solar
                case 2: type = 229; break; //vortex
                default: type = 135; break; //stardust
            }
            for (int index = 0; index < 80; ++index)
            {
                Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, type, 0.0f, 0.0f, 0, new Color(), 1f)];
                dust.velocity *= 10f;
                dust.fadeIn = 1f;
                dust.scale = 1 + Main.rand.NextFloat() + Main.rand.Next(4) * 0.3f;
                if (Main.rand.Next(3) != 0)
                {
                    dust.noGravity = true;
                    dust.velocity *= 3f;
                    dust.scale *= 2f;
                }
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                const int max = 22;
                const float rotationInterval = 2f * (float)Math.PI / max;
                for (int i = 0; i < max; i++)
                {
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Normalize(Projectile.velocity).RotatedBy(rotationInterval * i),
                          ModContent.ProjectileType<GlowLine>(), Projectile.damage / 3, 0f, Main.myPlayer, 7, Projectile.ai[0]);
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255 - Projectile.alpha);
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
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 3)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}