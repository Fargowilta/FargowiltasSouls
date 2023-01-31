using FargowiltasSouls.EternityMode;
using FargowiltasSouls.EternityMode.Content.Boss.HM;
//using FargowiltasSouls.EternityMode.Content.Boss.HM;
using FargowiltasSouls.EternityMode.Content.Boss.PHM;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class GlowLine : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glow Line");
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 2400;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.alpha = 255;

            Projectile.hide = true;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public Color color = Color.White;

        public override bool? CanDamage()
        {
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(counter);
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            counter = reader.ReadInt32();
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        private int counter;
        private int drawLayers = 1;

        public override void AI()
        {
            int maxTime = 60;
            float alphaModifier = 3;

            switch ((int)Projectile.ai[0])
            {
                case 0: //abom flaming scythe telegraph, sticks to abom and follows his line of sight to player w/ offset
                    {
                        color = Color.Yellow;
                        maxTime = 30;
                        alphaModifier = 10;

                        NPC abom = FargoSoulsUtil.NPCExists(Projectile.localAI[1], ModContent.NPCType<NPCs.AbomBoss.AbomBoss>());
                        if (abom != null)
                        {
                            Projectile.Center = abom.Center;
                            Projectile.rotation = abom.DirectionTo(Main.player[abom.target].Center).ToRotation() + Projectile.ai[1];
                        }
                    }
                    break;

                case 1: //abom split sickle box telegraph, hides until after the sickles split
                    {
                        color = Color.Yellow;
                        maxTime = 90 + 60;
                        Projectile.rotation = Projectile.ai[1];
                        alphaModifier = 1;
                        if (counter < 90)
                            alphaModifier = 0;
                        else
                            Projectile.velocity = Vector2.Zero;
                    }
                    break;

                case 2: //devi sparkling love, decelerates alongside energy hearts
                    {
                        color = Color.HotPink;
                        maxTime = 90;
                        Projectile.scale = 0.5f;
                        Projectile.rotation = Projectile.ai[1];
                        alphaModifier = 0.5f;
                        if (Projectile.velocity != Vector2.Zero)
                        {
                            if (counter == 0)
                                Projectile.localAI[1] = -Projectile.velocity.Length() / maxTime;

                            float speed = Projectile.velocity.Length();
                            speed += Projectile.localAI[1];
                            Projectile.velocity = Vector2.Normalize(Projectile.velocity) * speed;
                        }
                    }
                    break;

                case 3: //abom laevateinn 1&2 telegraph, swing around to where actual sword will spawn
                    {
                        color = Color.Yellow;
                        maxTime = 60;
                        alphaModifier = 6f;

                        NPC abom = FargoSoulsUtil.NPCExists(Projectile.localAI[1], ModContent.NPCType<NPCs.AbomBoss.AbomBoss>());
                        if (abom != null)
                        {
                            Projectile.Center = abom.Center;
                            if (counter == 0)
                                Projectile.rotation = abom.DirectionTo(Main.player[abom.target].Center).ToRotation();
                            float targetRot = abom.DirectionTo(Main.player[abom.target].Center).ToRotation() + Projectile.ai[1];
                            while (targetRot < -(float)Math.PI)
                                targetRot += 2f * (float)Math.PI;
                            while (targetRot > (float)Math.PI)
                                targetRot -= 2f * (float)Math.PI;
                            Projectile.rotation = Projectile.rotation.AngleLerp(targetRot, 0.05f);
                        }
                    }
                    break;

                case 4: //abom laevateinn 3 telegraph, swing around to where actual sword will spawn but slower
                    {
                        color = Color.Yellow;
                        maxTime = 150;
                        alphaModifier = 7f;

                        NPC abom = FargoSoulsUtil.NPCExists(Projectile.localAI[1], ModContent.NPCType<NPCs.AbomBoss.AbomBoss>());
                        if (abom != null)
                        {
                            Projectile.Center = abom.Center;
                            float targetRot = Projectile.ai[1];
                            while (targetRot < -(float)Math.PI)
                                targetRot += 2f * (float)Math.PI;
                            while (targetRot > (float)Math.PI)
                                targetRot -= 2f * (float)Math.PI;
                            Projectile.velocity = Projectile.velocity.ToRotation().AngleLerp(targetRot, 0.05f).ToRotationVector2();
                        }

                        Projectile.position -= Projectile.velocity;
                        Projectile.rotation = Projectile.velocity.ToRotation();
                    }
                    break;

                case 5: //abom cirno, slide in to a halt from outside
                    {
                        color = new Color(0, 1f, 1f);
                        maxTime = 120;
                        alphaModifier = 4f;

                        NPC abom = FargoSoulsUtil.NPCExists(Projectile.localAI[1], ModContent.NPCType<NPCs.AbomBoss.AbomBoss>());
                        if (abom != null)
                        {
                            Vector2 targetPos = abom.Center + Vector2.UnitX * Projectile.ai[1];
                            Projectile.Center = Vector2.Lerp(Projectile.Center, targetPos, 0.03f);
                        }

                        Projectile.position -= Projectile.velocity;
                        Projectile.rotation = Projectile.velocity.ToRotation();
                    }
                    break;

                case 6: //eridanus vortex lightning starting angles
                    {
                        Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFreezeImmune = true;

                        color = new Color(51, 255, 191);
                        maxTime = 90;

                        Player p = FargoSoulsUtil.PlayerExists(Projectile.ai[1]);
                        if (p != null)
                        {
                            Projectile.rotation = Projectile.DirectionTo(p.Center).ToRotation();
                        }
                        else
                        {
                            Projectile.ai[1] = Player.FindClosest(Projectile.Center, 0, 0);
                        }

                        Projectile.position -= Projectile.velocity;
                        Projectile.rotation += Projectile.velocity.ToRotation(); //yes, PLUS because rotation is set up there, velocity is the offset
                    }
                    break;

                case 7: //celestial pillar explode
                    {
                        Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFreezeImmune = true;

                        switch ((int)Projectile.ai[1])
                        {
                            case 0: color = Color.Magenta; break; //nebula
                            case 1: color = Color.Orange; break; //solar
                            case 2: color = new Color(51, 255, 191); break; //vortex
                            default: color = Color.SkyBlue; break; //stardust
                        }
                        maxTime = 20;
                        alphaModifier = -1;
                        Projectile.alpha = 0;
                        Projectile.scale = 0.5f;

                        Projectile.position -= Projectile.velocity;
                        Projectile.rotation = Projectile.velocity.ToRotation();

                        if (Main.LocalPlayer.active && !Main.dedServ)
                            Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 30;

                        if (counter == maxTime)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int j = 0; j < 4; j++)
                                {
                                    Vector2 speed = (8f * (j + 1) + 4f) * Projectile.velocity;
                                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, speed, ModContent.ProjectileType<CelestialFragment>(), Projectile.damage, 0f, Main.myPlayer, Projectile.ai[1]);
                                }
                            }
                        }
                    }
                    break;

                case 8: //prime limbs
                    {
                        color = new Color(51, 255, 191, 0);
                        maxTime = 60;

                        NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], NPCID.PrimeCannon, NPCID.PrimeLaser, NPCID.PrimeSaw, NPCID.PrimeVice);
                        if (npc != null)
                        {
                            Projectile.Center = npc.Center;
                            Projectile.rotation = npc.rotation + MathHelper.PiOver2;
                        }
                        else
                        {
                            Projectile.Kill();
                            return;
                        }

                        Projectile.position -= Projectile.velocity;
                        Projectile.rotation += Projectile.velocity.ToRotation(); //yes, PLUS because rotation is set up there, velocity is the offset
                    }
                    break;

                case 9: //reti telegraph
                    {
                        color = Color.Red;
                        maxTime = 120;
                        alphaModifier = 2;

                        NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], NPCID.Retinazer);
                        if (npc != null)
                        {
                            Vector2 offset = new Vector2(npc.width - 24, 0).RotatedBy(npc.rotation + 1.57079633);
                            Projectile.Center = npc.Center + offset;
                            Projectile.rotation = npc.rotation + MathHelper.PiOver2;
                        }
                        else
                        {
                            Projectile.Kill();
                            return;
                        }
                    }
                    break;

                case 10: //deviantt shadowbeam telegraph
                    {
                        color = Color.Purple;
                        maxTime = 90;
                        alphaModifier = 1;
                        Projectile.scale = 0.5f;

                        NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], ModContent.NPCType<NPCs.DeviBoss.DeviBoss>());
                        if (npc != null)
                        {
                            Projectile.Center = npc.Center;
                            Projectile.rotation = npc.localAI[0];
                        }
                        else
                        {
                            Projectile.Kill();
                            return;
                        }
                    }
                    break;

                case 11: //destroyer telegraphs
                    {
                        maxTime = 90;
                        alphaModifier = -1;
                        Projectile.Opacity = Math.Clamp((float)counter / maxTime, 0f, 1f);

                        Projectile.scale = 0.6f;

                        NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], NPCID.TheDestroyerBody, NPCID.TheDestroyerTail);
                        if (npc == null)
                        {
                            Projectile.Kill();
                            return;
                        }

                        NPC destroyer = FargoSoulsUtil.NPCExists(npc.realLife, NPCID.TheDestroyer);
                        if (destroyer == null || destroyer.GetGlobalNPC<Destroyer>().IsCoiling)
                        {
                            Projectile.Kill();
                            return;
                        }

                        if (counter == 0)
                            Projectile.localAI[0] = Main.rand.NextFloat(0.9f, 1.1f);

                        color = npc.ai[2] == 0 ? Color.Red : Color.Yellow;
                        Projectile.Center = npc.Center;

                        float rotationModifier = (1f - Projectile.localAI[0]) * 10f;
                        float maxDegreeVariance = FargoSoulsWorld.MasochistModeReal ? 60 : 30;
                        Projectile.localAI[1] += MathHelper.ToRadians(maxDegreeVariance) * rotationModifier / maxTime;
                        Projectile.rotation = Projectile.localAI[1];

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (npc.ai[2] == 0)
                            {
                                if (counter == maxTime)
                                {
									//only make blue telegraph in emode
									if (!FargoSoulsWorld.MasochistModeReal)
                                    {
										Projectile.NewProjectile(Projectile.InheritSource(Projectile),
											Projectile.Center, Projectile.rotation.ToRotationVector2(),
											Projectile.type,
											Projectile.damage, Projectile.knockBack, Projectile.owner, 16f);
									}

                                    Projectile.NewProjectile(Projectile.InheritSource(Projectile),
                                        Projectile.Center, Projectile.localAI[0] * Projectile.rotation.ToRotationVector2(),
                                        ModContent.ProjectileType<DestroyerLaser>(),
                                        Projectile.damage, Projectile.knockBack, Projectile.owner);
                                }
                            }
                            else
                            {
                                if (counter > maxTime - 20 && counter % 10 == 0)
                                {
									//only make blue telegraph in emode
									if (!FargoSoulsWorld.MasochistModeReal)
                                    {
										Projectile.NewProjectile(Projectile.InheritSource(Projectile),
											Projectile.Center, Projectile.rotation.ToRotationVector2(),
											Projectile.type,
											Projectile.damage, Projectile.knockBack, Projectile.owner, 16f);
									}

                                    Projectile.NewProjectile(Projectile.InheritSource(Projectile),
                                        Projectile.Center, Projectile.localAI[0] * Projectile.rotation.ToRotationVector2(),
                                        ModContent.ProjectileType<DarkStarHoming>(),
                                        Projectile.damage, Projectile.knockBack, Projectile.owner, -1, 1f);
                                }
                            }
                        }
                    }
                    break;

                case 12: //wof vanilla laser telegraph
                    {
                        color = Color.Purple;
                        maxTime = 645;
                        drawLayers = 4;
                        alphaModifier = -1;

                        NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], NPCID.WallofFleshEye);
                        if (npc != null && (npc.GetGlobalNPC<WallofFleshEye>().HasTelegraphedNormalLasers || Main.netMode == NetmodeID.MultiplayerClient))
                        {
                            Projectile.rotation = npc.rotation + (npc.direction > 0 ? 0 : MathHelper.Pi);
                            Projectile.velocity = Projectile.rotation.ToRotationVector2();
                            Projectile.Center = npc.Center + (npc.width - 52) * Vector2.UnitX.RotatedBy(Projectile.rotation);

                            if (counter < npc.localAI[1])
                                counter = (int)npc.localAI[1];

                            Projectile.alpha = (int)(255 * Math.Cos(Math.PI / 2 / maxTime * counter));
                        }
                        else
                        {
                            Projectile.Kill();
                            return;
                        }
                    }
                    break;

                case 13: //mutant final spark tell
                    {
                        color = new Color(51, 255, 191);
                        maxTime = 90;
                        alphaModifier = counter > maxTime / 2 ? 6 : 3;
                        Projectile.scale = 4f;

                        NPC mutant = FargoSoulsUtil.NPCExists(Projectile.ai[1], ModContent.NPCType<NPCs.MutantBoss.MutantBoss>());
                        if (mutant != null)
                        {
                            float targetRot = MathHelper.WrapAngle(mutant.ai[3]);
                            Projectile.velocity = Projectile.velocity.ToRotation().AngleLerp(targetRot, 0.12f * (float)Math.Pow((float)counter / maxTime, 3f)).ToRotationVector2();
                        }

                        Projectile.position -= Projectile.velocity;
                        Projectile.rotation = Projectile.velocity.ToRotation();
                    }
                    break;

                case 14: //moon lord vortex telegraph
                    {
                        color = new Color(51, 255, 191);
                        maxTime = 180;
                        alphaModifier = 5;

                        Projectile vortex = FargoSoulsUtil.ProjectileExists(FargoSoulsUtil.GetProjectileByIdentity(Projectile.owner, Projectile.ai[1], ModContent.ProjectileType<MoonLordVortex>()));
                        if (vortex != null)
                        {
                            Projectile.Center = vortex.Center;

                            Projectile.position -= Projectile.velocity;
                            Projectile.rotation = Projectile.velocity.ToRotation();
                        }
                        else if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.Kill();
                            return;
                        }
                    }
                    break;

                case 15: //nebula pillar distortion fields
                    {
                        color = Color.Purple;
                        maxTime = 270;
                        alphaModifier = 4;
                        drawLayers = 4;
                        Projectile.scale = 24f;

                        Projectile.rotation = Projectile.ai[1];

                        if (Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost
                            && Projectile.Colliding(Projectile.Hitbox, Main.LocalPlayer.Hitbox))
                        {
                            Main.LocalPlayer.AddBuff(BuffID.VortexDebuff, 2);
                        }
                    }
                    break;

                case 16: //destroyer blue laser line up true telegraph
                    {
                        color = Color.SkyBlue;
                        maxTime = 30;
                        alphaModifier = -1;
                        Projectile.Opacity = Math.Clamp(1f - (float)counter / maxTime, 0f, 1f);
                        Projectile.scale = 0.6f;

                        Projectile.rotation = Projectile.velocity.ToRotation();
                        Projectile.position -= Projectile.velocity;
                    }
                    break;

                case 17: //moon lord nebula distortion field
                    {
                        color = Color.Purple;
                        maxTime = 270;
                        alphaModifier = 2;
                        drawLayers = 2;
                        Projectile.scale = 24f;

                        NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], NPCID.MoonLordCore);
                        if (npc == null)
                        {
                            Projectile.Kill();
                            return;
                        }
                        else
                        {
                            if (counter == 0)
                            {
                                for (int i = 0; i < Main.maxProjectiles; i++)
                                {
                                    if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<LunarRitual>() && Main.projectile[i].ai[1] == npc.whoAmI)
                                    {
                                        Projectile.localAI[1] = i;
                                        break;
                                    }
                                }
                            }

                            Projectile ritual = FargoSoulsUtil.ProjectileExists(Projectile.localAI[1], ModContent.ProjectileType<LunarRitual>());
                            if (ritual != null && ritual.ai[1] == npc.whoAmI)
                            {
                                Projectile.Center = ritual.Center;
                                Projectile.position.X += Projectile.localAI[0];
                                Projectile.position.Y += 1500;
                            }
                            Projectile.rotation = -MathHelper.PiOver2;

                            if (npc.GetGlobalNPC<MoonLordCore>().VulnerabilityState <= 2)
                            {
                                if (counter > maxTime / 2)
                                    counter = maxTime / 2;
                            }
                            else
                            {
                                if (counter < maxTime - 60)
                                    counter = maxTime - 60;
                            }
                        }

                        if (Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost
                            && Projectile.Colliding(Projectile.Hitbox, Main.LocalPlayer.Hitbox))
                        {
                            Main.LocalPlayer.AddBuff(BuffID.VortexDebuff, 2);
                        }
                    }
                    break;

                case 18: //cultist arena new visual
                    {
                        color = Color.Cyan * 0.75f;
                        maxTime = 60 * 2;
                        alphaModifier = 1;

                        NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], NPCID.CultistBoss);
                        if (npc != null)
                        {
                            if (counter > maxTime / 2)
                                counter = maxTime / 2;
							float ratio = (float)counter / (maxTime / 2);
							Projectile.scale = 0.5f + 2.5f * ratio;

                            if (npc.ai[0] == 5)
                            {
                                //in here so it doesnt kill itself as soon as it spawns
                                if (counter > 0 && npc.ai[1] == 1f && Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.Kill();
                                    return;
                                }

                                int ritual = (int)npc.ai[2];
                                if (ritual > -1 && ritual < Main.maxProjectiles && Main.projectile[ritual].active && Main.projectile[ritual].type == ProjectileID.CultistRitual)
                                {
                                    if (counter == 0)
                                    {
                                        Vector2 offset = Projectile.Center - Main.projectile[ritual].Center;
                                        Projectile.localAI[0] = offset.X;
                                        Projectile.localAI[1] = offset.Y;
                                    }
									
                                    Projectile.Center = Main.projectile[ritual].Center + new Vector2(Projectile.localAI[0], Projectile.localAI[1]) * ratio;
                                }
                            }
                        }

                        Projectile.position -= Projectile.velocity;
                        Projectile.rotation = Projectile.velocity.ToRotation();
                    }
                    break;

                case 19: //timber head squrrl warning
                    {
                        color = new Color(93, 255, 241, 0) * 0.75f;
                        alphaModifier = 1;
                        Projectile.scale = 2f;

                        maxTime = 20 * 2;
                        if (counter < maxTime / 2) //effectively start at max brightness then fade
                            counter = maxTime / 2;

                        Projectile.position -= Projectile.velocity;
                        Projectile.rotation = Projectile.velocity.ToRotation();
                    }
                    break;

                default:
                    Main.NewText("glow line: you shouldnt be seeing this text, show terry");
                    break;
            }

            if (++counter > maxTime)
            {
                Projectile.Kill();
                return;
            }

            if (alphaModifier >= 0)
            {
                Projectile.alpha = 255 - (int)(255 * Math.Sin(Math.PI / maxTime * counter) * alphaModifier);
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }

            color.A = 0;
        }

        public override void Kill(int timeLeft)
        {
            base.Kill(timeLeft);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float num6 = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 3000f, 16f * Projectile.scale, ref num6))
            {
                return true;
            }
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return color * Projectile.Opacity * (Main.mouseTextColor / 255f) * 0.9f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Main.spriteBatch.End(); Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = texture2D13.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            const int length = 3000;
            Vector2 offset = Projectile.rotation.ToRotationVector2() * length / 2f;
            Vector2 position = Projectile.Center - Main.screenLastPosition + new Vector2(0f, Projectile.gfxOffY) + offset;
            const float resolutionCompensation = 128f / 24f; //i made the image higher res, this compensates to keep original display size
            Rectangle destination = new Rectangle((int)position.X, (int)position.Y, length, (int)(rectangle.Height * Projectile.scale / resolutionCompensation));

            Color drawColor = Projectile.GetAlpha(lightColor);

            for (int j = 0; j < drawLayers; j++)
                Main.EntitySpriteDraw(new DrawData(texture2D13, destination, new Rectangle?(rectangle), drawColor, Projectile.rotation, origin2, SpriteEffects.None, 0));

            //Main.spriteBatch.End(); Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
    }
}