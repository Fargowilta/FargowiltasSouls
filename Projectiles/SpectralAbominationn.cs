using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class SpectralAbominationn : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/NPCs/Eternals/AbominationnSoul";

        private const float PI = (float)Math.PI;
        private const float rotationPerTick = PI / 57f;
        private const float threshold = 150;
        private float ringRotation;
        private float scytheRotation;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spectral Abominationn");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            //Projectile.alpha = 100;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
        }

        public override void AI()
        {
            if (Projectile.localAI[1] == 0f)
            {
                Projectile.localAI[1] = Projectile.ai[1] + 1;

                //SoundEngine.PlaySound(SoundID.Zombie20, Projectile.Center);
                SoundEngine.PlaySound(SoundID.ForceRoarPitched, Projectile.Center);

                /*switch ((int)Projectile.ai[1])
                {
                    case 1: Projectile.DamageType = DamageClass.Melee; break;
                    case 2: Projectile.DamageType = DamageClass.Ranged; break;
                    case 3: Projectile.DamageType = DamageClass.Magic; break;
                    case 4: Projectile.DamageType = DamageClass.Summon; break;
                    case 5: Projectile.DamageType = DamageClass.Throwing; break;
                    default: break;
                }*/
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }

            if (Projectile.localAI[0]++ > 30f)
            {
                Projectile.localAI[0] = 0f;
                Projectile.ai[1]++;
            }

            if (Projectile.ai[1] % 2 == 1) //dash
            {
                //Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
                /*Projectile.frameCounter = 5;
                Projectile.frame = 6;*/

                int num22 = 7;
                for (int index1 = 0; index1 < num22; ++index1)
                {
                    Vector2 vector2_1 = (Vector2.Normalize(Projectile.velocity) * new Vector2((Projectile.width + 50) / 2f, Projectile.height) * 0.75f).RotatedBy((index1 - (num22 / 2 - 1)) * Math.PI / num22, new Vector2()) + Projectile.Center;
                    Vector2 vector2_2 = ((float)(Main.rand.NextDouble() * 3.14159274101257) - 1.570796f).ToRotationVector2() * Main.rand.Next(3, 8);
                    Vector2 vector2_3 = vector2_2;
                    int index2 = Dust.NewDust(vector2_1 + vector2_3, 0, 0, DustID.Shadowflame, vector2_2.X * 2f, vector2_2.Y * 2f, 100, new Color(), 1.4f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].noLight = true;
                    Main.dust[index2].velocity /= 4f;
                    Main.dust[index2].velocity -= Projectile.velocity;
                }
            }
            else //preparing to dash
            {
                int ai0 = (int)Projectile.ai[0];
                const float moveSpeed = 1f;
                if (Projectile.localAI[0] == 30f) //just about to dash
                {
                    if (Projectile.ai[0] >= 0 && Main.npc[ai0].CanBeChasedBy()) //has target
                    {
                        Projectile.velocity = Main.npc[ai0].Center - Projectile.Center;
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= 27f;
                        //Projectile.rotation = Projectile.velocity.ToRotation();
                        Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
                        /*Projectile.frameCounter = 5;
                        Projectile.frame = 6;*/
                    }
                    else //no target
                    {
                        Projectile.localAI[0] = -1f;
                        Projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 1000);
                        Projectile.netUpdate = true;
                        /*if (++Projectile.frameCounter > 5)
                        {
                            Projectile.frameCounter = 0;
                            if (++Projectile.frame > 5)
                                Projectile.frame = 0;
                        }*/
                    }
                }
                else //regular movement
                {
                    if (Projectile.ai[0] >= 0 && Main.npc[ai0].CanBeChasedBy()) //has target
                    {
                        Vector2 vel = Main.npc[ai0].Center - Projectile.Center;
                        //Projectile.rotation = vel.ToRotation();
                        if (vel.X > 0) //projectile is on left side of target
                        {
                            vel.X -= 300;
                            Projectile.direction = Projectile.spriteDirection = 1;
                        }
                        else //projectile is on right side of target
                        {
                            vel.X += 300;
                            Projectile.direction = Projectile.spriteDirection = -1;
                        }
                        vel.Y -= 200f;
                        vel.Normalize();
                        vel *= 12f;
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
                    else //no target
                    {
                        if (Projectile.velocity.X < -1f)
                            Projectile.velocity.X += moveSpeed;
                        else if (Projectile.velocity.X > 1f)
                            Projectile.velocity.X -= moveSpeed;
                        if (Projectile.velocity.Y > -8f)
                            Projectile.velocity.Y -= moveSpeed;
                        else if (Projectile.velocity.Y < -10f)
                            Projectile.velocity.Y += moveSpeed;
                        Projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 1000);
                        Projectile.netUpdate = true;
                    }
                    /*if (++Projectile.frameCounter > 5)
                    {
                        Projectile.frameCounter = 0;
                        if (++Projectile.frame > 5)
                            Projectile.frame = 0;
                    }*/
                }
            }
            Projectile.position += Projectile.velocity / 4f;

            if (++Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }

            ringRotation += rotationPerTick;
            scytheRotation += 0.5f;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (target.defense > 0)
                damage += target.defense / 2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 0;
            //target.AddBuff(ModContent.BuffType<OceanicMaul>(), 900);
            target.AddBuff(ModContent.BuffType<MutantNibble>(), 900);
            //target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 900);
        }

        public override void Kill(int timeleft)
        {
            SoundEngine.PlaySound(SoundID.Item84, Projectile.Center);
            if (Projectile.owner == Main.myPlayer)
            {
                SpawnRazorbladeRing(12, 12.5f, 0.75f);
                SpawnRazorbladeRing(12, 10f, -2f);
            }
        }

        private void SpawnRazorbladeRing(int max, float speed, float rotationModifier)
        {
            float rotation = 2f * (float)Math.PI / max;
            Vector2 vel = Projectile.velocity;
            vel.Normalize();
            vel *= speed;
            int type = ModContent.ProjectileType<AbomScytheFriendly>();
            for (int i = 0; i < max; i++)
            {
                vel = vel.RotatedBy(rotation);
                if (Projectile.CountsAsClass(DamageClass.Summon))
                {
                    FargoSoulsUtil.NewSummonProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, type, Projectile.originalDamage / 3,
                        Projectile.knockBack / 4f, Projectile.owner, rotationModifier * Projectile.spriteDirection);
                }
                else
                {
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, type, Projectile.damage / 3,
                        Projectile.knockBack / 4f, Projectile.owner, rotationModifier * Projectile.spriteDirection);
                    if (p != Main.maxProjectiles)
                        Main.projectile[p].DamageType = Projectile.DamageType;
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

            DrawRing();

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }

        private void DrawRing()
        {
            Texture2D texture2D13 = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Projectiles/AbomScytheFriendly", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            int num156 = texture2D13.Height / 4; //ypos of lower right corner of sprite to draw
            int y3 = 0; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = Color.White * Projectile.Opacity;

            for (int x = 0; x < 6; x++)
            {
                Vector2 drawOffset = new Vector2(threshold / 2f, 0f).RotatedBy(ringRotation);
                drawOffset = drawOffset.RotatedBy(2f * PI / 6f * x);
                Main.EntitySpriteDraw(texture2D13, Projectile.Center + drawOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, x % 2 == 0 ? scytheRotation : -scytheRotation, origin2, Projectile.scale, x % 2 == 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity * 0.75f;
        }
    }
}