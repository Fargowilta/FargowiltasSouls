using FargowiltasSouls.Buffs.Boss;
using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantEyeOfCthulhu : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/NPCs/Resprites/NPC_4";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eye of Cthulhu");
            Main.projFrames[Projectile.type] = Main.npcFrameCount[NPCID.EyeofCthulhu];
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            CooldownSlot = 1;

            Projectile.timeLeft = 216;

            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 2;

            Projectile.alpha = 255;
        }

        public override bool CanHitPlayer(Player target)
        {
            return target.hurtCooldowns[1] == 0;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        private const float degreesOffset = 45f / 2;
        private const float dashSpeed = 120f;
        private const float baseDistance = 700f;

        //private float goldScytheAngleOffset;
        //private float cyanScytheAngleOffset;

        public override bool? CanDamage()
        {
            return Projectile.ai[1] >= 120;
        }

        bool spawned;

        public override void AI()
        {
            Player player = FargoSoulsUtil.PlayerExists(Projectile.ai[0]);
            if (player == null)
            {
                Projectile.Kill();
                return;
            }

            void SpawnProjectile(Vector2 position)
            {
                float accel = 0.03f;

                Vector2 target = new Vector2(Projectile.localAI[0], Projectile.localAI[1]);// + 150f * Vector2.UnitX.RotatedBy(cyanScytheAngleOffset);
                target += 180 * Projectile.DirectionTo(target).RotatedBy(MathHelper.PiOver2);

                float angle = Projectile.DirectionTo(target).ToRotation();

                int p = Projectile.NewProjectile(Projectile.InheritSource(Projectile), position, Vector2.Zero, ModContent.ProjectileType<MutantScythe1>(), Projectile.damage, 0, Main.myPlayer, accel, angle);
                if (p != Main.maxProjectiles)
                {
                    Main.projectile[p].timeLeft = Projectile.timeLeft + 180 + 30 + 150; //+ 60 + 240;
                    if (FargoSoulsWorld.MasochistModeReal)
                        Main.projectile[p].timeLeft -= 30;
                }
            };

            if (!spawned)
            {
                spawned = true;

                SoundEngine.PlaySound(SoundID.ForceRoarPitched, Projectile.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, -1, NPCID.EyeofCthulhu);
            }

            if (++Projectile.ai[1] < 120)
            {
                Projectile.alpha -= 8;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;

                Projectile.position += player.velocity / 2f;

                float rangeModifier = Projectile.ai[1] * 1.5f / 120f;
                if (rangeModifier < 0.25f)
                    rangeModifier = 0.25f;
                if (rangeModifier > 1f)
                    rangeModifier = 1f;
                Vector2 target = player.Center + Projectile.DirectionFrom(player.Center).RotatedBy(MathHelper.ToRadians(20)) * baseDistance * rangeModifier;

                float speedModifier = 0.6f;
                if (Projectile.Center.X < target.X)
                {
                    Projectile.velocity.X += speedModifier;
                    if (Projectile.velocity.X < 0)
                        Projectile.velocity.X += speedModifier * 2;
                }
                else
                {
                    Projectile.velocity.X -= speedModifier;
                    if (Projectile.velocity.X > 0)
                        Projectile.velocity.X -= speedModifier * 2;
                }
                if (Projectile.Center.Y < target.Y)
                {
                    Projectile.velocity.Y += speedModifier;
                    if (Projectile.velocity.Y < 0)
                        Projectile.velocity.Y += speedModifier * 2;
                }
                else
                {
                    Projectile.velocity.Y -= speedModifier;
                    if (Projectile.velocity.Y > 0)
                        Projectile.velocity.Y -= speedModifier * 2;
                }
                if (Math.Abs(Projectile.velocity.X) > 24)
                    Projectile.velocity.X = 24 * Math.Sign(Projectile.velocity.X);
                if (Math.Abs(Projectile.velocity.Y) > 24)
                    Projectile.velocity.Y = 24 * Math.Sign(Projectile.velocity.Y);

                Projectile.rotation = Projectile.DirectionTo(player.Center).ToRotation() - MathHelper.PiOver2;
            }
            else if (Projectile.ai[1] == 120)
            {
                Projectile.localAI[0] = player.Center.X;
                Projectile.localAI[1] = player.Center.Y;
                Projectile.Center = player.Center + Projectile.DirectionFrom(player.Center) * baseDistance;
                Projectile.velocity = Vector2.Zero;
                Projectile.netUpdate = true;
                //goldScytheAngleOffset = Main.rand.NextFloat(MathHelper.TwoPi);
                //cyanScytheAngleOffset = goldScytheAngleOffset + MathHelper.Pi + Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2); //always somewhere in the opposite half
            }
            else if (Projectile.ai[1] == 121) //make the golden sickles
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    SpawnProjectile(Projectile.Center - Projectile.velocity / 2);

                    float accel = 0.025f;
                    Vector2 target = new Vector2(Projectile.localAI[0], Projectile.localAI[1]); //+ 150f * Vector2.UnitX.RotatedBy(goldScytheAngleOffset);
                    float angle = Projectile.DirectionTo(target).ToRotation();
                    int p = Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MutantScythe2>(), Projectile.damage, 0, Main.myPlayer, accel, angle);
                    if (p != Main.maxProjectiles)
                        Main.projectile[p].timeLeft = Projectile.timeLeft + 180 + 30;

                    if (FargoSoulsWorld.MasochistModeReal)
                    {
                        p = Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MutantScythe2>(), Projectile.damage, 0, Main.myPlayer, accel, angle);
                        if (p != Main.maxProjectiles)
                            Main.projectile[p].timeLeft = Projectile.timeLeft + 180 + 30 + 150;
                    }
                }

                /*if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    SpawnProjectile(Projectile.Center);
                    SpawnProjectile(Projectile.Center - Projectile.velocity / 2);
                }*/

                Projectile.velocity = dashSpeed * Projectile.DirectionTo(new Vector2(Projectile.localAI[0], Projectile.localAI[1])).RotatedBy(MathHelper.ToRadians(degreesOffset));
                Projectile.netUpdate = true;
                SoundEngine.PlaySound(SoundID.ForceRoarPitched, Projectile.Center);
            }
            else if (Projectile.ai[1] < 120 + baseDistance / dashSpeed * 2)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    SpawnProjectile(Projectile.Center);
                    SpawnProjectile(Projectile.Center - Projectile.velocity / 2);
                }
            }
            else
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    SpawnProjectile(Projectile.Center);
                    SpawnProjectile(Projectile.Center - Projectile.velocity / 2);
                }

                Projectile.ai[1] = 120;
            }

            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }

            if (Projectile.frame < 3)
                Projectile.frame = 3;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(BuffID.Obstructed, 15);
                target.GetModPlayer<FargoSoulsPlayer>().MaxLifeReduction += 100;
                target.AddBuff(ModContent.BuffType<OceanicMaul>(), 5400);
                target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 120);
                target.AddBuff(ModContent.BuffType<Berserked>(), 300);
                target.AddBuff(ModContent.BuffType<MutantFang>(), 180);
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 50; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0, 0, 0, default(Color), 2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 5f;
            }

            Vector2 goreSpeed = Projectile.localAI[0] != 0 && Projectile.localAI[1] != 0 ?
                dashSpeed / 4f * Projectile.DirectionTo(new Vector2(Projectile.localAI[0], Projectile.localAI[1])).RotatedBy(MathHelper.ToRadians(degreesOffset)) : Vector2.Zero;
            for (int i = 0; i < 2; i++)
            {
                if (!Main.dedServ)
                {
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position + new Vector2(Main.rand.NextFloat(Projectile.width), Main.rand.NextFloat(Projectile.height)), goreSpeed, ModContent.Find<ModGore>(Mod.Name, $"Gore_8").Type);
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position + new Vector2(Main.rand.NextFloat(Projectile.width), Main.rand.NextFloat(Projectile.height)), goreSpeed, ModContent.Find<ModGore>(Mod.Name, $"Gore_9").Type);
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position + new Vector2(Main.rand.NextFloat(Projectile.width), Main.rand.NextFloat(Projectile.height)), goreSpeed, ModContent.Find<ModGore>(Mod.Name, $"Gore_10").Type);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = Projectile.GetAlpha(lightColor);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            float scale = (Main.mouseTextColor / 200f - 0.35f) * 0.3f + 0.9f;
            scale *= Projectile.scale;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26 * (Projectile.ai[1] >= 120 ? 0.75f : 0.5f);
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, scale, SpriteEffects.None, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}

