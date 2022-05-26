using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.DemonKing
{
    public class DukeFishronMinion : ModProjectile
    {
        private const float PI = (float)Math.PI;
        private float rotationOffset;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Duke Fishron");
            Main.projFrames[Projectile.type] = 8;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 100;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 10;
            Projectile.GetGlobalProjectile<Projectiles.FargoSoulsGlobalProjectile>().CanSplit = false;
            Projectile.scale *= 0.75f;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(rotationOffset);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            rotationOffset = reader.ReadSingle();
        }

        bool spawn;

        public override void AI()
        {
            if (!spawn)
            {
                spawn = true;
                Projectile.ai[0] = -1;
            }

            if (Main.player[Projectile.owner].active && !Main.player[Projectile.owner].dead
                && Main.player[Projectile.owner].GetModPlayer<FargoSoulsPlayer>().DukeFishron)
            {
                Projectile.timeLeft = 2;
            }

            if (Projectile.Distance(Main.player[Projectile.owner].Center) > 3000)
                Projectile.Center = Main.player[Projectile.owner].Center;

            if (Projectile.localAI[0]++ > 30f) //timer handling everything else
            {
                Projectile.localAI[0] = 0f;
                rotationOffset = Main.rand.NextFloat(-PI / 2, PI / 2);
                Projectile.ai[1]++;
            }

            if (Projectile.localAI[1] > 0) //timer for rings on hit
                Projectile.localAI[1]--;

            if (Projectile.ai[1] % 2 == 1) //dash
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
                Projectile.frameCounter = 5;
                Projectile.frame = 6;

                if (Projectile.spriteDirection < 0)
                    Projectile.rotation += (float)Math.PI;

                /*if (Projectile.localAI[0] % 2 == 0 && Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, 10f * Vector2.UnitX.RotatedByRandom(2 * Math.PI), 
                        ModContent.ProjectileType<DukeBubble>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                }*/

                int num22 = 7;
                for (int index1 = 0; index1 < num22; ++index1)
                {
                    Vector2 vector2_1 = (Vector2.Normalize(Projectile.velocity) * new Vector2((Projectile.width + 50) / 2f, Projectile.height) * 0.75f).RotatedBy((index1 - (num22 / 2 - 1)) * Math.PI / num22, new Vector2()) + Projectile.Center;
                    Vector2 vector2_2 = ((float)(Main.rand.NextDouble() * 3.14159274101257) - 1.570796f).ToRotationVector2() * Main.rand.Next(3, 8);
                    Vector2 vector2_3 = vector2_2;
                    int index2 = Dust.NewDust(vector2_1 + vector2_3, 0, 0, 172, vector2_2.X * 2f, vector2_2.Y * 2f, 100, new Color(), 1.4f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].noLight = true;
                    Main.dust[index2].velocity /= 4f;
                    Main.dust[index2].velocity -= Projectile.velocity;
                }
            }
            else //preparing to dash
            {
                int ai0 = (int)Projectile.ai[0];
                float moveSpeed = 1f;
                if (Projectile.localAI[0] == 30f) //just about to dash
                {
                    if (Projectile.ai[0] >= 0 && Main.npc[ai0].CanBeChasedBy()) //has target
                    {
                        Projectile.velocity = Main.npc[ai0].Center - Projectile.Center + Main.npc[ai0].velocity * 10f;
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= 27f;
                        Projectile.rotation = Projectile.velocity.ToRotation();
                        Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
                        Projectile.frameCounter = 5;
                        Projectile.frame = 6;

                        if (Projectile.spriteDirection < 0)
                            Projectile.rotation += (float)Math.PI;
                    }
                    else //no target
                    {
                        Projectile.localAI[0] = -1f;
                        Projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, 1500);
                        Projectile.netUpdate = true;
                        if (++Projectile.frameCounter > 5)
                        {
                            Projectile.frameCounter = 0;
                            if (++Projectile.frame > 5)
                                Projectile.frame = 0;
                        }
                    }
                }
                else //regular movement
                {
                    if (Projectile.localAI[0] == 0)
                        Projectile.localAI[0] = Main.rand.Next(10);

                    if (Projectile.ai[0] >= 0 && Main.npc[ai0].CanBeChasedBy()) //has target
                    {
                        moveSpeed *= 1.5f;

                        Vector2 vel = Main.npc[ai0].Center - Projectile.Center;
                        Projectile.rotation = vel.ToRotation();
                        Vector2 offset = Vector2.Zero;
                        if (vel.X > 0) //projectile is on left side of target
                        {
                            offset.X = -360;
                            Projectile.direction = Projectile.spriteDirection = 1;
                        }
                        else //projectile is on right side of target
                        {
                            offset.X = 360;
                            Projectile.direction = Projectile.spriteDirection = -1;
                        }
                        offset = offset.RotatedBy(rotationOffset);
                        vel += offset;
                        vel.Normalize();
                        vel *= 24f;
                        if (Projectile.velocity.X < vel.X)
                        {
                            Projectile.velocity.X += moveSpeed;
                            if (Projectile.velocity.X < 0 && vel.X > 0)
                                Projectile.velocity.X += moveSpeed;
                        }
                        else if (Projectile.velocity.X > vel.X)
                        {
                            Projectile.velocity.X -= moveSpeed;
                            if (Projectile.velocity.X > 0 && vel.X < 0)
                                Projectile.velocity.X -= moveSpeed;
                        }
                        if (Projectile.velocity.Y < vel.Y)
                        {
                            Projectile.velocity.Y += moveSpeed;
                            if (Projectile.velocity.Y < 0 && vel.Y > 0)
                                Projectile.velocity.Y += moveSpeed;
                        }
                        else if (Projectile.velocity.Y > vel.Y)
                        {
                            Projectile.velocity.Y -= moveSpeed;
                            if (Projectile.velocity.Y > 0 && vel.Y < 0)
                                Projectile.velocity.Y -= moveSpeed;
                        }

                        if (Projectile.spriteDirection < 0)
                            Projectile.rotation += (float)Math.PI;
                    }
                    else //no target
                    {
                        Vector2 target = Main.player[Projectile.owner].Center;
                        target.X -= 60 * Main.player[Projectile.owner].direction * Projectile.minionPos;
                        target.Y -= 40;
                        if (Projectile.Distance(target) > 25)
                        {
                            moveSpeed *= 0.5f;

                            Vector2 vel = target - Projectile.Center;
                            Projectile.rotation = 0;
                            Projectile.direction = Projectile.spriteDirection = Main.player[Projectile.owner].direction;
                            vel.Normalize();
                            vel *= 24f;
                            if (Projectile.velocity.X < vel.X)
                            {
                                Projectile.velocity.X += moveSpeed;
                                if (Projectile.velocity.X < 0 && vel.X > 0)
                                    Projectile.velocity.X += moveSpeed;
                            }
                            else if (Projectile.velocity.X > vel.X)
                            {
                                Projectile.velocity.X -= moveSpeed;
                                if (Projectile.velocity.X > 0 && vel.X < 0)
                                    Projectile.velocity.X -= moveSpeed;
                            }
                            if (Projectile.velocity.Y < vel.Y)
                            {
                                Projectile.velocity.Y += moveSpeed;
                                if (Projectile.velocity.Y < 0 && vel.Y > 0)
                                    Projectile.velocity.Y += moveSpeed;
                            }
                            else if (Projectile.velocity.Y > vel.Y)
                            {
                                Projectile.velocity.Y -= moveSpeed;
                                if (Projectile.velocity.Y > 0 && vel.Y < 0)
                                    Projectile.velocity.Y -= moveSpeed;
                            }
                        }

                        if (Projectile.ai[0] != -1)
                        {
                            Projectile.ai[0] = -1;
                            Projectile.localAI[0] = 0;
                            Projectile.netUpdate = true;
                        }

                        if (Projectile.localAI[0] > 6)
                        {
                            Projectile.localAI[0] = 0;
                            Projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, 1500);
                            Projectile.netUpdate = true;
                        }
                    }
                    if (++Projectile.frameCounter > 5)
                    {
                        Projectile.frameCounter = 0;
                        if (++Projectile.frame > 5)
                            Projectile.frame = 0;
                    }
                }
            }
            Projectile.position += Projectile.velocity / 4f;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (target.defense > 0)
                damage += target.defense / 2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<MutantNibble>(), 900);

            if (Projectile.localAI[1] <= 0)
            {
                Projectile.localAI[1] = 60;

                SoundEngine.PlaySound(SoundID.Item84, Projectile.Center); //rings on hit
                if (Projectile.owner == Main.myPlayer)
                {
                    int modifier = Main.rand.NextBool() ? -1 : 1;
                    SpawnRazorbladeRing(8, 24f, -0.75f * modifier);
                    SpawnRazorbladeRing(8, 24f, 1.5f * modifier);
                }
            }
        }

        private void SpawnRazorbladeRing(int max, float speed, float rotationModifier)
        {
            float rotation = 2f * (float)Math.PI / max;
            Vector2 vel = Projectile.velocity;
            vel.Normalize();
            vel *= speed;
            int type = ModContent.ProjectileType<RazorbladeTyphoonFriendly2>();
            for (int i = 0; i < max; i++)
            {
                vel = vel.RotatedBy(rotation);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, type, Projectile.damage,
                    Projectile.knockBack / 4f, Projectile.owner, rotationModifier * Projectile.spriteDirection);
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

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = Color.Blue * Projectile.Opacity * 0.2f;
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