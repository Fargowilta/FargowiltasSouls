using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class WillBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Will Bomb");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = 40;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;

            CooldownSlot = 1;

            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 1;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Vector2 reduction = Vector2.Normalize(Projectile.velocity) * Projectile.ai[0];
            Projectile.velocity -= reduction;

            Projectile.rotation += Projectile.velocity.Length() * 0.03f * Math.Sign(Projectile.velocity.X);
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
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, vel, type, damage, 0f, Main.myPlayer, rotationModifier, speed);
            }
            SoundEngine.PlaySound(SoundID.Item84, Projectile.Center);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);

            if (Main.LocalPlayer.active)
                Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 30;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (FargoSoulsWorld.EternityMode)
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<WillRitual>(), Projectile.damage, 0f, Main.myPlayer, 0f, Projectile.ai[1]);

                if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.championBoss, ModContent.NPCType<NPCs.Champions.WillChampion>()))
                {
                    if (Main.npc[EModeGlobalNPC.championBoss].ai[0] > -1)
                    {
                        if (Main.npc[EModeGlobalNPC.championBoss].localAI[2] == 1)
                        {
                            SpawnSphereRing(8, 8f, Projectile.damage, 2f);
                            SpawnSphereRing(8, 8f, Projectile.damage, -2f);
                        }

                        if (Main.npc[EModeGlobalNPC.championBoss].localAI[3] == 1)
                        {
                            SpawnSphereRing(8, 8f, Projectile.damage, 0.5f);
                            SpawnSphereRing(8, 8f, Projectile.damage, -0.5f);
                        }
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        float rotation = 0;
                        if (Main.npc[EModeGlobalNPC.championBoss].localAI[2] == 1)
                            rotation = MathHelper.Pi / 4f / 420f;
                        if (Main.npc[EModeGlobalNPC.championBoss].localAI[3] == 1)
                            rotation = MathHelper.Pi / -3f / 420f;

                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.UnitX.RotatedBy(Math.PI / 4 * 2 * i + Math.PI / 4),
                            ModContent.ProjectileType<WillDeathray>(), Projectile.damage, 0f, Main.myPlayer, rotation, Projectile.ai[1]);
                    }
                }
            }

            Projectile.position = Projectile.Center;
            Projectile.width = 250;
            Projectile.height = 250;
            Projectile.Center = Projectile.position;

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            for (int index1 = 0; index1 < 20; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 87, 0f, 0f, 100, new Color(), 3f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 12f;
                Main.dust[index2].noLight = true;

                int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 87, 0f, 0f, 100, new Color(), 2f);
                Main.dust[index3].velocity *= 9f;
                Main.dust[index3].noGravity = true;
                Main.dust[index3].noLight = true;

                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 87, 0f, 0f, 100, default, 4.5f);
                Main.dust[d].velocity *= Main.rand.NextFloat(9f, 12f);
                Main.dust[d].position = Projectile.Center;
            }

            for (int i = 0; i < 50; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default(Color), 4f);
                Main.dust[dust].scale *= Main.rand.NextFloat(1, 2.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity = Main.dust[dust].velocity.RotatedByRandom(MathHelper.ToRadians(40)) * 6f;
                Main.dust[dust].velocity *= 4f;

                dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default(Color), 4f);
                Main.dust[dust].velocity *= 8f;
            }

            float scaleFactor9 = 2.5f;
            for (int j = 0; j < 20; j++)
            {
                int gore = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position + new Vector2(Main.rand.Next(Projectile.width), Main.rand.Next(Projectile.height)), Vector2.Zero, Main.rand.Next(61, 64), scaleFactor9);
                Main.gore[gore].velocity.Y += 2f;
                Main.gore[gore].velocity *= 6f;
            }
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