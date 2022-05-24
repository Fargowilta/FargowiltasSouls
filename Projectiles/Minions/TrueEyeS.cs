using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class TrueEyeS : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_650";

        private float localAI0;
        private float localAI1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("True Eye of Cthulhu");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            //ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 32;
            Projectile.height = 42;
            Projectile.timeLeft *= 5;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.minion = true;

            Projectile.DamageType = DamageClass.Summon;

            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.whoAmI == Main.myPlayer && player.active && !player.dead && player.GetModPlayer<FargoSoulsPlayer>().TrueEyes)
            {
                Projectile.timeLeft = 2;
                Projectile.netUpdate = true;
            }

            if (Projectile.damage == 0)
                Projectile.damage = (int)(60f * player.GetDamage(DamageClass.Summon).Additive);

            //lighting effect?
            DelegateMethods.v3_1 = new Vector3(0.5f, 0.9f, 1f) * 1.5f;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * 6f, 20f, DelegateMethods.CastLightOpen);
            Utils.PlotTileLine(Projectile.Left, Projectile.Right, 20f, DelegateMethods.CastLightOpen);
            Utils.PlotTileLine(player.Center, player.Center + player.velocity * 6f, 40f, DelegateMethods.CastLightOpen);
            Utils.PlotTileLine(player.Left, player.Right, 40f, DelegateMethods.CastLightOpen);

            if (Projectile.ai[0] >= 0 && Projectile.ai[0] < Main.maxNPCs) //has target
            {
                NPC minionAttackTargetNpc = Projectile.OwnerMinionAttackTargetNPC;
                if (minionAttackTargetNpc != null && Projectile.ai[0] != minionAttackTargetNpc.whoAmI && minionAttackTargetNpc.CanBeChasedBy())
                    Projectile.ai[0] = minionAttackTargetNpc.whoAmI;

                Projectile.localAI[0]++;
                NPC npc = Main.npc[(int)Projectile.ai[0]];
                if (npc.CanBeChasedBy())
                {
                    switch ((int)Projectile.ai[1])
                    {
                        case 0: //true eye movement code
                            Vector2 newVel = npc.Center - Projectile.Center + new Vector2(-200f, -200f);
                            if (newVel != Vector2.Zero)
                            {
                                newVel.Normalize();
                                newVel *= 24f;
                                Projectile.velocity.X = (Projectile.velocity.X * 29 + newVel.X) / 30;
                                Projectile.velocity.Y = (Projectile.velocity.Y * 29 + newVel.Y) / 30;
                            }
                            if (Projectile.Distance(npc.Center) < 150f)
                            {
                                if (Projectile.Center.X < npc.Center.X)
                                    Projectile.velocity.X -= 0.25f;
                                else
                                    Projectile.velocity.X += 0.25f;

                                if (Projectile.Center.Y < npc.Center.Y)
                                    Projectile.velocity.Y -= 0.25f;
                                else
                                    Projectile.velocity.Y += 0.25f;
                            }

                            if (Projectile.localAI[0] > 60f)
                            {
                                if (Projectile.Distance(npc.Center) > 1500f) //give up if too far
                                {
                                    Projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, 1000);
                                    Projectile.ai[1] = 0f;
                                    Projectile.localAI[1] = 0f;
                                }
                                Projectile.localAI[0] = 0f;
                                Projectile.ai[1]++;
                            }
                            break;

                        case 1: //slow down
                            Projectile.velocity *= 0.9f;
                            if (Projectile.velocity.Length() < 1f) //stop, FIRE LASER
                            {
                                Projectile.velocity = Vector2.Zero;
                                Projectile.localAI[0] = 0f;
                                Projectile.ai[1]++;
                            }
                            break;

                        case 2: //shoot
                            if (Projectile.localAI[0] == 7f)
                            {
                                SoundEngine.PlaySound(SoundID.NPCDeath6 with { Volume = 0.75f, Pitch = 0 }, Projectile.Center);
                                ShootBolts(npc);
                            }
                            else if (Projectile.localAI[0] == 14f)
                            {
                                ShootBolts(npc);
                            }
                            else if (Projectile.localAI[0] > 21f)
                            {
                                Projectile.localAI[0] = 0f;
                                Projectile.ai[1]++;
                            }
                            break;

                        default:
                            Projectile.ai[1] = 0f;
                            goto case 0;
                    }
                }
                else //forget target
                {
                    Projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, 1000);
                    Projectile.ai[1] = 0f;
                    Projectile.localAI[0] = 0f;
                    Projectile.localAI[1] = 0f;
                }

                if (Projectile.rotation > 3.14159274101257)
                    Projectile.rotation = Projectile.rotation - 6.283185f;
                Projectile.rotation = Projectile.rotation <= -0.005 || Projectile.rotation >= 0.005 ? Projectile.rotation * 0.96f : 0.0f;
                if (++Projectile.frameCounter >= 4)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= Main.projFrames[Projectile.type])
                        Projectile.frame = 0;
                }
                UpdatePupil();
            }
            else
            {
                if (Projectile.localAI[1]++ > 15f)
                {
                    Projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, 1000);
                    Projectile.ai[1] = 0f;
                    Projectile.localAI[0] = 0f;
                    Projectile.localAI[1] = 0f;
                }

                Vector2 vector2_1 = new Vector2(player.direction * -100f, 0); //vanilla movement code
                Vector2 vector2_2 = player.MountedCenter + vector2_1;
                float num1 = Vector2.Distance(Projectile.Center, vector2_2);
                if (num1 > 1500) //teleport when out of range
                    Projectile.Center = player.Center + vector2_1;
                Vector2 vector2_3 = vector2_2 - Projectile.Center;
                float num2 = 4f;
                if (num1 < num2)
                    Projectile.velocity *= 0.25f;
                if (vector2_3 != Vector2.Zero)
                {
                    if (vector2_3.Length() < num2)
                        Projectile.velocity = vector2_3;
                    else
                        Projectile.velocity = vector2_3 * 0.1f;
                }
                if (Projectile.velocity.Length() > 6) //when moving fast, rotate in direction of velocity
                {
                    float num3 = Projectile.velocity.ToRotation() + 1.570796f;
                    if (Math.Abs(Projectile.rotation - num3) >= 3.14159274101257)
                        Projectile.rotation = num3 >= Projectile.rotation ? Projectile.rotation + 6.283185f : Projectile.rotation - 6.283185f;
                    Projectile.rotation = (Projectile.rotation * 11f + num3) / 12f;
                    if (++Projectile.frameCounter >= 4)
                    {
                        Projectile.frameCounter = 0;
                        if (++Projectile.frame >= Main.projFrames[Projectile.type])
                            Projectile.frame = 0;
                    }
                }
                else //when moving slow, calm down
                {
                    if (Projectile.rotation > 3.14159274101257)
                        Projectile.rotation = Projectile.rotation - 6.283185f;
                    Projectile.rotation = Projectile.rotation <= -0.005 || Projectile.rotation >= 0.005 ? Projectile.rotation * 0.96f : 0f;
                    if (++Projectile.frameCounter >= 6)
                    {
                        Projectile.frameCounter = 0;
                        if (++Projectile.frame >= Main.projFrames[Projectile.type])
                            Projectile.frame = 0;
                    }
                }

                UpdatePupil();
            }
            /*Main.NewText("local0 " + localAI0.ToString()
                + " local1 " + localAI1.ToString()
                + " ai0 " + Projectile.ai[0].ToString()
                + " ai1 " + Projectile.ai[1].ToString());*/
        }

        private void ShootBolts(NPC npc)
        {
            Vector2 spawn = Projectile.Center - Vector2.UnitY * 6f;
            Vector2 vel = npc.Center + npc.velocity * 15f - spawn;
            if (vel != Vector2.Zero)
            {
                vel.Normalize();
                vel *= 8f;
                for (int i = -1; i <= 1; i++)
                {
                    FargoSoulsUtil.NewSummonProjectile(Projectile.GetSource_FromThis(), spawn, vel.RotatedBy(Math.PI / 24 * i), ModContent.ProjectileType<PhantasmalBoltTrueEye>(),
                        Projectile.originalDamage / 3 * 7, 6f, Projectile.owner);
                }
            }
        }

        private void UpdatePupil()
        {
            Player player = Main.player[Projectile.owner];
            float f1 = (float)(localAI0 % 6.28318548202515 - 3.14159274101257);
            float num13 = (float)Math.IEEERemainder(localAI1, 1.0);
            if (num13 < 0.0)
                ++num13;
            float num14 = (float)Math.Floor(localAI1);
            float max = 0.999f;
            int num15 = 0;
            float amount = 0.1f;
            float f2;
            float num18;
            float num19;
            if (Projectile.ai[0] != -1f) //targeted an enemy
            {
                f2 = Projectile.AngleTo(Main.npc[(int)Projectile.ai[0]].Center);
                num15 = 2;
                num18 = MathHelper.Clamp(num13 + 0.05f, 0.0f, max);
                num19 = num14 + Math.Sign(-12f - num14);
            }
            else if (player.velocity.Length() > 3)
            {
                f2 = Projectile.AngleTo(Projectile.Center + player.velocity);
                num15 = 1;
                num18 = MathHelper.Clamp(num13 + 0.05f, 0.0f, max);
                num19 = num14 + Math.Sign(-10f - num14);
            }
            else
            {
                f2 = player.direction == 1 ? 0.0f : 3.141603f;
                num18 = MathHelper.Clamp(num13 + Math.Sign(0.75f - num13) * 0.05f, 0.0f, max);
                num19 = num14 + Math.Sign(0.0f - num14);
                amount = 0.12f;
            }
            Vector2 rotationVector2 = f2.ToRotationVector2();
            localAI0 = (float)(Vector2.Lerp(f1.ToRotationVector2(), rotationVector2, amount).ToRotation() + num15 * 6.28318548202515 + 3.14159274101257);
            localAI1 = num19 + num18;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 360);
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            int r = lightColor.R * 3 / 2;
            int g = lightColor.G * 3 / 2;
            int b = lightColor.B * 3 / 2;
            if (r > 255)
                r = 255;
            if (g > 255)
                g = 255;
            if (b > 255)
                b = 255;
            return new Color(r, g, b);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);

            Texture2D pupil = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Projectiles/Minions/TrueEyePupil", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Vector2 pupilOffset = new Vector2(localAI1 / 2f, 0f).RotatedBy(localAI0);
            pupilOffset += new Vector2(0f, -6f).RotatedBy(Projectile.rotation);
            Vector2 pupilOrigin = pupil.Size() / 2f;
            Main.EntitySpriteDraw(pupil, pupilOffset + Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(pupil.Bounds), Projectile.GetAlpha(lightColor), 0f, pupilOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}