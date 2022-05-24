using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.AbomBoss
{
    public class AbomFlocko : ModProjectile
    {
        public override string Texture => "Terraria/Images/NPC_352";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Super Flocko");
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.timeLeft = 420;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            CooldownSlot = 1;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], ModContent.NPCType<NPCs.AbomBoss.AbomBoss>());
            if (npc == null)
            {
                Projectile.Kill();
                return;
            }

            Vector2 target = npc.Center;
            target.X += (npc.localAI[3] > 1 ? 1200 : 2000) * (float)Math.Sin(2 * Math.PI / 720 * Projectile.ai[1]++);
            target.Y -= 1100;

            Vector2 distance = target - Projectile.Center;
            float length = distance.Length();
            if (length > 100f)
            {
                distance /= 8f;
                Projectile.velocity = (Projectile.velocity * 23f + distance) / 24f;
            }
            else
            {
                if (Projectile.velocity.Length() < 12f)
                    Projectile.velocity *= 1.05f;
            }

            /*Projectile.localAI[0]++;
            if (Projectile.localAI[0] > 45)
            {
                Projectile.localAI[0] = 0f;
                if (Projectile.owner == Main.myPlayer)
                {
                    Vector2 vel = distance;
                    vel.Normalize();
                    vel *= 9f;
                    Projectile.NewProjectile(Projectile.Center, vel, ModContent.ProjectileType<FrostWave>(),
                        Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }*/

            if (++Projectile.localAI[0] > 90 && ++Projectile.localAI[1] > (npc.localAI[3] > 1 ? 4 : 2)) //spray shards
            {
                SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
                Projectile.localAI[1] = 0f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (Math.Abs(npc.Center.X - Projectile.Center.X) > 400)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Vector2 speed = new Vector2(Main.rand.Next(-1000, 1001), Main.rand.Next(-1000, 1001));
                            speed.Normalize();
                            speed *= 8f;
                            Projectile.NewProjectile(npc.GetSource_FromThis(), Projectile.Center + speed * 4f, speed, ModContent.ProjectileType<AbomFrostShard>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                    }
                    if (Main.player[npc.target].active && !Main.player[npc.target].dead && Main.player[npc.target].Center.Y < Projectile.Center.Y)
                    {
                        SoundEngine.PlaySound(SoundID.Item120, Projectile.position);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 vel = Projectile.DirectionTo(Main.player[npc.target].Center + new Vector2(Main.rand.Next(-200, 201), Main.rand.Next(-200, 201))) * 12f;
                            Projectile.NewProjectile(npc.GetSource_FromThis(), Projectile.Center, vel, ModContent.ProjectileType<AbomFrostWave>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                    }
                }
            }

            Projectile.rotation += Projectile.velocity.Length() / 12f * (Projectile.velocity.X > 0 ? -0.2f : 0.2f);
            if (++Projectile.frameCounter > 3)
            {
                if (++Projectile.frame >= 6)
                    Projectile.frame = 0;
                Projectile.frameCounter = 0;
            }
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int index1 = 0; index1 < 20; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool() ? 80 : 76, 0.0f, 0.0f, 0, new Color(), 1f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].noLight = true;
                Main.dust[index2].scale++;
                Main.dust[index2].velocity *= 4f;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 200);
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

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26 * 0.5f;
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