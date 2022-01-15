using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.NPCs;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantPillar : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Masomode/CelestialPillar";

        private int target = -1;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Celestial Pillar");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 120;
            projectile.height = 120;
            projectile.aiStyle = -1;
            projectile.alpha = 255;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 600;
            cooldownSlot = 1;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = true;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(target);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            target = reader.ReadInt32();
        }

        public override bool CanDamage()
        {
            return projectile.alpha == 0;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1f;
                int type;
                switch ((int)projectile.ai[0])
                {
                    case 0: type = 242; break; //nebula
                    case 1: type = 127; break; //solar
                    case 2: type = 229; break; //vortex
                    default: type = 135; break; //stardust
                }
                for (int index = 0; index < 50; ++index)
                {
                    Dust dust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, type, 0.0f, 0.0f, 0, new Color(), 1f)];
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
            if (projectile.alpha > 0)
            {
                projectile.velocity.Y += 5f / 120f;
                projectile.rotation += projectile.velocity.Length() / 20f * 2f;
                projectile.localAI[1] += projectile.velocity.Y;
                projectile.alpha -= 2;
                if (projectile.alpha <= 0)
                {
                    projectile.alpha = 0;
                    if (target != -1)
                    {
                        Main.PlaySound(SoundID.Item89, projectile.Center);
                        projectile.velocity = Main.player[target].Center - projectile.Center;
                        float distance = projectile.velocity.Length();
                        projectile.velocity.Normalize();
                        const float speed = 32f;
                        projectile.velocity *= speed;
                        projectile.timeLeft = (int)(distance / speed);
                        projectile.netUpdate = true;
                        return;
                    }
                    else
                    {
                        projectile.Kill();
                    }
                }
                else
                {
                    NPC npc = Main.npc[(int)projectile.ai[1]];
                    projectile.Center = npc.Center;
                    projectile.position.Y += projectile.localAI[1];
                }

                if (target >= 0 && Main.player[target].active && !Main.player[target].dead)
                {
                    if (projectile.alpha < 100)
                    {
                        projectile.rotation = projectile.rotation.AngleLerp(
                          (Main.player[target].Center - projectile.Center).ToRotation(), (255 - projectile.alpha) / 255f * 0.08f);
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
                            float distance = projectile.Distance(Main.player[i].Center);
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
                        projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                projectile.rotation = projectile.velocity.ToRotation();
            }
            projectile.frame = (int)projectile.ai[0];
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (target.mount.Active)
                target.mount.Dismount(target);
            target.velocity.X = projectile.velocity.X < 0 ? -15f : 15f;
            target.velocity.Y = -10f;
            target.AddBuff(mod.BuffType("Stunned"), 60);
            target.AddBuff(mod.BuffType("CurseoftheMoon"), 600);
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(mod.BuffType("MarkedforDeath"), 240);
                target.AddBuff(mod.BuffType("MutantFang"), 180);
            }
            switch ((int)projectile.ai[0])
            {
                case 0: target.AddBuff(mod.BuffType("ReverseManaFlow"), 360); break; //nebula
                case 1: target.AddBuff(mod.BuffType("Atrophied"), 360); break; //solar
                case 2: target.AddBuff(mod.BuffType("Jammed"), 360); break; //vortex
                default: target.AddBuff(mod.BuffType("Asocial"), 360); break; //stardust
            }
            projectile.timeLeft = 0;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item92, projectile.Center);
            int type;
            switch ((int)projectile.ai[0])
            {
                case 0: type = 242; break; //nebula
                case 1: type = 127; break; //solar
                case 2: type = 229; break; //vortex
                default: type = 135; break; //stardust
            }
            for (int index = 0; index < 80; ++index)
            {
                Dust dust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, type, 0.0f, 0.0f, 0, new Color(), 1f)];
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
                int fragmentDuration = 240;
                if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<NPCs.MutantBoss.MutantBoss>())
                    && Main.npc[EModeGlobalNPC.mutantBoss].ai[0] == 19)
                {
                    fragmentDuration = 420 - (int)Main.npc[EModeGlobalNPC.mutantBoss].ai[1] + 120;
                }

                const int max = 24;
                const float rotationInterval = 2f * (float)Math.PI / max;
                for (int j = 0; j < 4; j++)
                {
                    Vector2 speed = new Vector2(0f, 5f * (j + 0.5f)).RotatedBy(projectile.rotation);
                    for (int i = 0; i < max; i++)
                    {
                        int p = Projectile.NewProjectile(projectile.Center, speed.RotatedBy(rotationInterval * i),
                            mod.ProjectileType("MutantFragment"), projectile.damage / 2, 0f, Main.myPlayer, projectile.ai[0]);
                        if (p != Main.maxProjectiles)
                            Main.projectile[p].timeLeft = fragmentDuration;
                    }
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255 - projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i += 3)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}