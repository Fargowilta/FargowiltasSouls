using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.NPCs;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class WillBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Will Bomb");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 50;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.timeLeft = 40;
            projectile.tileCollide = false;
            projectile.penetrate = -1;

            cooldownSlot = 1;

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 1;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Vector2 reduction = Vector2.Normalize(projectile.velocity) * projectile.ai[0];
            projectile.velocity -= reduction;

            projectile.rotation += projectile.velocity.Length() * 0.03f * Math.Sign(projectile.velocity.X);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<Defenseless>(), 300);
                target.AddBuff(ModContent.BuffType<Midas>(), 300);
            }
            target.AddBuff(BuffID.Bleeding, 300);
        }

        private void SpawnSphereRing(int max, float speed, int damage, float rotationModifier)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            float rotation = 2f * (float)Math.PI / max;
            Vector2 vel = Vector2.UnitY * speed;
            int type = ModContent.ProjectileType<WillTyphoon>();
            for (int i = 0; i < max; i++)
            {
                vel = vel.RotatedBy(rotation);
                Projectile.NewProjectile(projectile.Center, vel, type, damage, 0f, Main.myPlayer, rotationModifier, speed);
            }
            Main.PlaySound(SoundID.Item84, projectile.Center);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item92, projectile.Center);

            if (Main.LocalPlayer.active)
                Main.LocalPlayer.GetModPlayer<FargoPlayer>().Screenshake = 30;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (FargoSoulsWorld.EternityMode)
                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<WillRitual>(), projectile.damage, 0f, Main.myPlayer, 0f, projectile.ai[1]);

                if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.championBoss, ModContent.NPCType<NPCs.Champions.WillChampion>()))
                {
                    if (Main.npc[EModeGlobalNPC.championBoss].ai[0] > -1)
                    {
                        if (Main.npc[EModeGlobalNPC.championBoss].localAI[2] == 1)
                        {
                            SpawnSphereRing(8, 8f, projectile.damage, 2f);
                            SpawnSphereRing(8, 8f, projectile.damage, -2f);
                        }

                        if (Main.npc[EModeGlobalNPC.championBoss].localAI[3] == 1)
                        {
                            SpawnSphereRing(8, 8f, projectile.damage, 0.5f);
                            SpawnSphereRing(8, 8f, projectile.damage, -0.5f);
                        }
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        float rotation = 0;
                        if (Main.npc[EModeGlobalNPC.championBoss].localAI[2] == 1)
                            rotation = MathHelper.Pi / 4f / 420f;
                        if (Main.npc[EModeGlobalNPC.championBoss].localAI[3] == 1)
                            rotation = MathHelper.Pi / -3f / 420f;

                        Projectile.NewProjectile(projectile.Center, Vector2.UnitX.RotatedBy(Math.PI / 4 * 2 * i + Math.PI / 4),
                            ModContent.ProjectileType<WillDeathray>(), projectile.damage, 0f, Main.myPlayer, rotation, projectile.ai[1]);
                    }
                }
            }

            projectile.position = projectile.Center;
            projectile.width = 250;
            projectile.height = 250;
            projectile.Center = projectile.position;

            Main.PlaySound(SoundID.Item, projectile.Center, 14);

            for (int index1 = 0; index1 < 20; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 87, 0f, 0f, 100, new Color(), 3f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 12f;
                Main.dust[index2].noLight = true;

                int index3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 87, 0f, 0f, 100, new Color(), 2f);
                Main.dust[index3].velocity *= 9f;
                Main.dust[index3].noGravity = true;
                Main.dust[index3].noLight = true;

                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 87, 0f, 0f, 100, default, 4.5f);
                Main.dust[d].velocity *= Main.rand.NextFloat(9f, 12f);
                Main.dust[d].position = projectile.Center;
            }

            for (int i = 0; i < 50; i++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default(Color), 4f);
                Main.dust[dust].scale *= Main.rand.NextFloat(1, 2.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity = Main.dust[dust].velocity.RotatedByRandom(MathHelper.ToRadians(40)) * 6f;
                Main.dust[dust].velocity *= 4f;

                dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default(Color), 4f);
                Main.dust[dust].velocity *= 8f;
            }

            float scaleFactor9 = 2.5f;
            for (int j = 0; j < 20; j++)
            {
                int gore = Gore.NewGore(projectile.position + new Vector2(Main.rand.Next(projectile.width), Main.rand.Next(projectile.height)), Vector2.Zero, Main.rand.Next(61, 64), scaleFactor9);
                Main.gore[gore].velocity.Y += 2f;
                Main.gore[gore].velocity *= 6f;
            }
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

            SpriteEffects effects = projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = Color.White * projectile.Opacity * 0.75f * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}