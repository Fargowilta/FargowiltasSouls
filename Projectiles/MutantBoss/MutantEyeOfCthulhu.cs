using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantEyeOfCthulhu : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/NPCs/Resprites/NPC_4";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Eye of Cthulhu");
            Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.EyeofCthulhu];
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 80;
            projectile.height = 80;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            cooldownSlot = 1;

            projectile.timeLeft = 216;

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 2;

            projectile.alpha = 255;
        }

        public override bool CanHitPlayer(Player target)
        {
            return target.hurtCooldowns[1] == 0;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
        }

        private const float degreesOffset = 45f / 2;
        private const float dashSpeed = 120f;
        private const float baseDistance = 700f;

        //private float goldScytheAngleOffset;
        //private float cyanScytheAngleOffset;

        public override bool CanDamage()
        {
            return projectile.ai[1] >= 120;
        }

        public override void AI()
        {
            Player player = FargoSoulsUtil.PlayerExists(projectile.ai[0]);
            if (player == null)
            {
                projectile.Kill();
                return;
            }

            void SpawnProjectile(Vector2 position)
            {
                float accel = 0.03f;

                Vector2 target = new Vector2(projectile.localAI[0], projectile.localAI[1]);// + 150f * Vector2.UnitX.RotatedBy(cyanScytheAngleOffset);
                target += 180 * projectile.DirectionTo(target).RotatedBy(MathHelper.PiOver2);

                float angle = projectile.DirectionTo(target).ToRotation();

                int p = Projectile.NewProjectile(position, Vector2.Zero, ModContent.ProjectileType<MutantScythe1>(), projectile.damage, 0, Main.myPlayer, accel, angle);
                if (p != Main.maxProjectiles)
                    Main.projectile[p].timeLeft = projectile.timeLeft + 180 + 30 + 150; //+ 60 + 240;
            };

            if (projectile.ai[1]++ == 0)
            {
                Main.PlaySound(SoundID.ForceRoar, (int)projectile.Center.X, (int)projectile.Center.Y, -1, 1f, 0f);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, -1, NPCID.EyeofCthulhu);
            }
            else if (projectile.ai[1] < 120)
            {
                projectile.alpha -= 8;
                if (projectile.alpha < 0)
                    projectile.alpha = 0;

                projectile.position += player.velocity / 2f;

                float rangeModifier = projectile.ai[1] * 1.5f / 120f;
                if (rangeModifier < 0.25f)
                    rangeModifier = 0.25f;
                if (rangeModifier > 1f)
                    rangeModifier = 1f;
                Vector2 target = player.Center + projectile.DirectionFrom(player.Center).RotatedBy(MathHelper.ToRadians(20)) * baseDistance * rangeModifier;

                float speedModifier = 0.6f;
                if (projectile.Center.X < target.X)
                {
                    projectile.velocity.X += speedModifier;
                    if (projectile.velocity.X < 0)
                        projectile.velocity.X += speedModifier * 2;
                }
                else
                {
                    projectile.velocity.X -= speedModifier;
                    if (projectile.velocity.X > 0)
                        projectile.velocity.X -= speedModifier * 2;
                }
                if (projectile.Center.Y < target.Y)
                {
                    projectile.velocity.Y += speedModifier;
                    if (projectile.velocity.Y < 0)
                        projectile.velocity.Y += speedModifier * 2;
                }
                else
                {
                    projectile.velocity.Y -= speedModifier;
                    if (projectile.velocity.Y > 0)
                        projectile.velocity.Y -= speedModifier * 2;
                }
                if (Math.Abs(projectile.velocity.X) > 24)
                    projectile.velocity.X = 24 * Math.Sign(projectile.velocity.X);
                if (Math.Abs(projectile.velocity.Y) > 24)
                    projectile.velocity.Y = 24 * Math.Sign(projectile.velocity.Y);

                projectile.rotation = projectile.DirectionTo(player.Center).ToRotation() - MathHelper.PiOver2;
            }
            else if (projectile.ai[1] == 120)
            {
                projectile.localAI[0] = player.Center.X;
                projectile.localAI[1] = player.Center.Y;
                projectile.Center = player.Center + projectile.DirectionFrom(player.Center) * baseDistance;
                projectile.velocity = Vector2.Zero;
                projectile.netUpdate = true;
                //goldScytheAngleOffset = Main.rand.NextFloat(MathHelper.TwoPi);
                //cyanScytheAngleOffset = goldScytheAngleOffset + MathHelper.Pi + Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2); //always somewhere in the opposite half
            }
            else if (projectile.ai[1] == 121)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float accel = 0.025f;
                    Vector2 target = new Vector2(projectile.localAI[0], projectile.localAI[1]); //+ 150f * Vector2.UnitX.RotatedBy(goldScytheAngleOffset);
                    float angle = projectile.DirectionTo(target).ToRotation();
                    int p = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<MutantScythe2>(), projectile.damage, 0, Main.myPlayer, accel, angle);
                    if (p != Main.maxProjectiles)
                        Main.projectile[p].timeLeft = projectile.timeLeft + 180 + 30;
                }

                /*if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    SpawnProjectile(projectile.Center);
                    SpawnProjectile(projectile.Center - projectile.velocity / 2);
                }*/

                projectile.velocity = dashSpeed * projectile.DirectionTo(new Vector2(projectile.localAI[0], projectile.localAI[1])).RotatedBy(MathHelper.ToRadians(degreesOffset));
                projectile.netUpdate = true;
                Main.PlaySound(SoundID.ForceRoar, (int)projectile.Center.X, (int)projectile.Center.Y, -1, 1f, 0f);
            }
            else if (projectile.ai[1] < 120 + baseDistance / dashSpeed * 2)
            {
                projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    SpawnProjectile(projectile.Center);
                    SpawnProjectile(projectile.Center - projectile.velocity / 2);
                }
            }
            else
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    SpawnProjectile(projectile.Center);
                    SpawnProjectile(projectile.Center - projectile.velocity / 2);
                }

                projectile.ai[1] = 120;
            }

            if (++projectile.frameCounter > 6)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                    projectile.frame = 0;
            }

            if (projectile.frame < 3)
                projectile.frame = 3;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(BuffID.Obstructed, 15);
                target.GetModPlayer<FargoPlayer>().MaxLifeReduction += 100;
                target.AddBuff(mod.BuffType("OceanicMaul"), 5400);
                target.AddBuff(mod.BuffType("CurseoftheMoon"), 120);
                target.AddBuff(mod.BuffType("Berserked"), 300);
                target.AddBuff(mod.BuffType("MutantFang"), 180);
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 50; i++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Blood, 0, 0, 0, default(Color), 2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 5f;
            }

            Vector2 goreSpeed = projectile.localAI[0] != 0 && projectile.localAI[1] != 0 ?
                dashSpeed / 4f * projectile.DirectionTo(new Vector2(projectile.localAI[0], projectile.localAI[1])).RotatedBy(MathHelper.ToRadians(degreesOffset)) : Vector2.Zero;
            for (int i = 0; i < 2; i++)
            {
                Gore.NewGore(projectile.position + new Vector2(Main.rand.NextFloat(projectile.width), Main.rand.NextFloat(projectile.height)), goreSpeed, mod.GetGoreSlot("Gores/EyeOfCthulhu/Gore_8"));
                Gore.NewGore(projectile.position + new Vector2(Main.rand.NextFloat(projectile.width), Main.rand.NextFloat(projectile.height)), goreSpeed, mod.GetGoreSlot("Gores/EyeOfCthulhu/Gore_9"));
                Gore.NewGore(projectile.position + new Vector2(Main.rand.NextFloat(projectile.width), Main.rand.NextFloat(projectile.height)), goreSpeed, mod.GetGoreSlot("Gores/EyeOfCthulhu/Gore_10"));
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = projectile.GetAlpha(lightColor);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            float scale = (Main.mouseTextColor / 200f - 0.35f) * 0.3f + 0.9f;
            scale *= projectile.scale;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = color26 * (projectile.ai[1] >= 120 ? 0.75f : 0.5f);
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, scale, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}

