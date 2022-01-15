using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.EternityMode;
using FargowiltasSouls.EternityMode.Content.Boss.HM;
using FargowiltasSouls.EternityMode.Content.Boss.PHM;
using FargowiltasSouls.Projectiles.Masomode;

namespace FargowiltasSouls.Projectiles
{
    public class GlowLine : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Glow Line");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.alpha = 255;

            projectile.hide = true;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindProjectiles.Add(index);
        }

        public Color color = Color.White;

        public override bool CanDamage()
        {
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(counter);
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            counter = reader.ReadInt32();
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
        }

        private int counter;
        private int drawLayers = 1;

        public override void AI()
        {
            int maxTime = 60;
            float alphaModifier = 3;

            switch ((int)projectile.ai[0])
            {
                case 0: //abom flaming scythe telegraph, sticks to abom and follows his line of sight to player w/ offset
                    {
                        color = Color.Yellow;
                        maxTime = 30;
                        alphaModifier = 10;

                        NPC abom = FargoSoulsUtil.NPCExists(projectile.localAI[1], ModContent.NPCType<NPCs.AbomBoss.AbomBoss>());
                        if (abom != null)
                        {
                            projectile.Center = abom.Center;
                            projectile.rotation = abom.DirectionTo(Main.player[abom.target].Center).ToRotation() + projectile.ai[1];
                        }
                    }
                    break;

                case 1: //abom split sickle box telegraph, hides until after the sickles split
                    {
                        color = Color.Yellow;
                        maxTime = 90 + 60;
                        projectile.rotation = projectile.ai[1];
                        alphaModifier = 1;
                        if (counter < 90)
                            alphaModifier = 0;
                        else
                            projectile.velocity = Vector2.Zero;
                    }
                    break;

                case 2: //devi sparkling love, decelerates alongside energy hearts
                    {
                        color = Color.HotPink;
                        maxTime = 90;
                        projectile.rotation = projectile.ai[1];
                        alphaModifier = 1;
                        if (projectile.velocity != Vector2.Zero)
                        {
                            if (counter == 0)
                                projectile.localAI[1] = -projectile.velocity.Length() / maxTime;

                            float speed = projectile.velocity.Length();
                            speed += projectile.localAI[1];
                            projectile.velocity = Vector2.Normalize(projectile.velocity) * speed;
                        }
                    }
                    break;

                case 3: //abom laevateinn 1&2 telegraph, swing around to where actual sword will spawn
                    {
                        color = Color.Yellow;
                        maxTime = 60;
                        alphaModifier = 6f;
                        
                        NPC abom = FargoSoulsUtil.NPCExists(projectile.localAI[1], ModContent.NPCType<NPCs.AbomBoss.AbomBoss>());
                        if (abom != null)
                        {
                            projectile.Center = abom.Center;
                            if (counter == 0)
                                projectile.rotation = abom.DirectionTo(Main.player[abom.target].Center).ToRotation();
                            float targetRot = abom.DirectionTo(Main.player[abom.target].Center).ToRotation() + projectile.ai[1];
                            while (targetRot < -(float)Math.PI)
                                targetRot += 2f * (float)Math.PI;
                            while (targetRot > (float)Math.PI)
                                targetRot -= 2f * (float)Math.PI;
                            projectile.rotation = projectile.rotation.AngleLerp(targetRot, 0.05f);
                        }
                    }
                    break;

                case 4: //abom laevateinn 3 telegraph, swing around to where actual sword will spawn but slower
                    {
                        color = Color.Yellow;
                        maxTime = 150;
                        alphaModifier = 7f;

                        NPC abom = FargoSoulsUtil.NPCExists(projectile.localAI[1], ModContent.NPCType<NPCs.AbomBoss.AbomBoss>());
                        if (abom != null)
                        {
                            projectile.Center = abom.Center;
                            float targetRot = projectile.ai[1];
                            while (targetRot < -(float)Math.PI)
                                targetRot += 2f * (float)Math.PI;
                            while (targetRot > (float)Math.PI)
                                targetRot -= 2f * (float)Math.PI;
                            projectile.velocity = projectile.velocity.ToRotation().AngleLerp(targetRot, 0.05f).ToRotationVector2();
                        }

                        projectile.position -= projectile.velocity;
                        projectile.rotation = projectile.velocity.ToRotation();
                    }
                    break;

                case 5: //abom cirno, slide in to a halt from outside
                    {
                        color = new Color(0, 1f, 1f);
                        maxTime = 150;
                        alphaModifier = 10f;

                        NPC abom = FargoSoulsUtil.NPCExists(projectile.localAI[1], ModContent.NPCType<NPCs.AbomBoss.AbomBoss>());
                        if (abom != null)
                        {
                            Vector2 targetPos = abom.Center + Vector2.UnitX * projectile.ai[1];
                            projectile.Center = Vector2.Lerp(projectile.Center, targetPos, 0.025f);
                        }

                        projectile.position -= projectile.velocity;
                        projectile.rotation = projectile.velocity.ToRotation();
                    }
                    break;

                case 6: //eridanus vortex lightning starting angles
                    {
                        projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = true;

                        color = new Color(51, 255, 191);
                        maxTime = 90;

                        Player p = FargoSoulsUtil.PlayerExists(projectile.ai[1]);
                        if (p != null)
                        {
                            projectile.rotation = projectile.DirectionTo(p.Center).ToRotation();
                        }
                        else
                        {
                            projectile.ai[1] = Player.FindClosest(projectile.Center, 0, 0);
                        }

                        projectile.position -= projectile.velocity;
                        projectile.rotation += projectile.velocity.ToRotation(); //yes, PLUS because rotation is set up there, velocity is the offset
                    }
                    break;

                case 7: //celestial pillar explode
                    {
                        projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = true;

                        switch ((int)projectile.ai[1])
                        {
                            case 0: color = Color.Magenta; break; //nebula
                            case 1: color = Color.Orange; break; //solar
                            case 2: color = new Color(51, 255, 191); break; //vortex
                            default: color = Color.SkyBlue; break; //stardust
                        }
                        maxTime = 20;
                        alphaModifier = 2;

                        projectile.position -= projectile.velocity;
                        projectile.rotation = projectile.velocity.ToRotation();

                        if (counter == maxTime)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int j = 0; j < 4; j++)
                                {
                                    Vector2 speed = (8f * (j + 1) + 4f) * projectile.velocity;
                                    Projectile.NewProjectile(projectile.Center, speed, mod.ProjectileType("CelestialFragment"), projectile.damage, 0f, Main.myPlayer, projectile.ai[1]);
                                }
                            }
                        }
                    }
                    break;

                case 8: //prime limbs
                    {
                        color = Color.Yellow;
                        maxTime = 60;

                        NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[1], NPCID.PrimeCannon, NPCID.PrimeLaser, NPCID.PrimeSaw, NPCID.PrimeVice);
                        if (npc != null)
                        {
                            projectile.Center = npc.Center;
                            projectile.rotation = npc.rotation + MathHelper.PiOver2;
                        }
                        else
                        {
                            projectile.Kill();
                            return;
                        }

                        projectile.position -= projectile.velocity;
                        projectile.rotation += projectile.velocity.ToRotation(); //yes, PLUS because rotation is set up there, velocity is the offset
                    }
                    break;

                case 9: //reti telegraph
                    {
                        color = Color.Red;
                        maxTime = 120;
                        alphaModifier = 2;

                        NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[1], NPCID.Retinazer);
                        if (npc != null)
                        {
                            Vector2 offset = new Vector2(npc.width - 24, 0).RotatedBy(npc.rotation + 1.57079633);
                            projectile.Center = npc.Center + offset;
                            projectile.rotation = npc.rotation + MathHelper.PiOver2;
                        }
                        else
                        {
                            projectile.Kill();
                            return;
                        }
                    }
                    break;

                case 10: //deviantt shadowbeam telegraph
                    {
                        color = Color.Purple;
                        maxTime = 90;
                        alphaModifier = 2;

                        NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[1], ModContent.NPCType<NPCs.DeviBoss.DeviBoss>());
                        if (npc != null)
                        {
                            projectile.Center = npc.Center;
                            projectile.rotation = npc.localAI[0];
                        }
                        else
                        {
                            projectile.Kill();
                            return;
                        }
                    }
                    break;

                case 11: //destroyer telegraphs
                    {
                        maxTime = 90;
                        alphaModifier = 2;

                        NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[1], NPCID.TheDestroyerBody, NPCID.TheDestroyerTail);
                        if (npc == null)
                        {
                            projectile.Kill();
                            return;
                        }

                        NPC destroyer = FargoSoulsUtil.NPCExists(npc.realLife, NPCID.TheDestroyer);
                        if (destroyer == null || destroyer.GetEModeNPCMod<Destroyer>().IsCoiling)
                        {
                            projectile.Kill();
                            return;
                        }

                        color = npc.ai[2] == 0 ? Color.Red : Color.Yellow;
                        projectile.Center = npc.Center;
                        projectile.rotation = projectile.localAI[1];

                        if (counter == 0)
                            projectile.localAI[0] = Main.rand.NextFloat(0.9f, 1.1f);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (npc.ai[2] == 0)
                            {
                                if (counter == maxTime)
                                    Projectile.NewProjectile(projectile.Center, projectile.localAI[0] * projectile.rotation.ToRotationVector2(), ModContent.ProjectileType<DestroyerLaser>(), projectile.damage, projectile.knockBack, projectile.owner);
                            }
                            else
                            {
                                if (counter > maxTime - 20 && counter % 10 == 0)
                                    Projectile.NewProjectile(projectile.Center, projectile.localAI[0] * projectile.rotation.ToRotationVector2(), ModContent.ProjectileType<DarkStarHoming>(), projectile.damage, projectile.knockBack, projectile.owner, -1, 1f);
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

                        NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[1], NPCID.WallofFleshEye);
                        if (npc != null && (npc.GetEModeNPCMod<WallofFleshEye>().HasTelegraphedNormalLasers || Main.netMode == NetmodeID.MultiplayerClient))
                        {
                            projectile.rotation = npc.rotation + (npc.direction > 0 ? 0 : MathHelper.Pi);
                            projectile.velocity = projectile.rotation.ToRotationVector2();
                            projectile.Center = npc.Center + (npc.width - 52) * Vector2.UnitX.RotatedBy(projectile.rotation);

                            if (counter < npc.localAI[1])
                                counter = (int)npc.localAI[1];

                            projectile.alpha = (int)(255 * Math.Cos(Math.PI / 2 / maxTime * counter));
                        }
                        else
                        {
                            projectile.Kill();
                            return;
                        }
                    }
                    break;

                case 13: //mutant final spark tell
                    {
                        color = new Color(51, 255, 191);
                        maxTime = 90;
                        alphaModifier = counter > maxTime / 2 ? 6 : 3;
                        projectile.scale = 4f;

                        NPC mutant = FargoSoulsUtil.NPCExists(projectile.ai[1], ModContent.NPCType<NPCs.MutantBoss.MutantBoss>());
                        if (mutant != null)
                        {
                            float targetRot = MathHelper.WrapAngle(mutant.ai[3]);
                            projectile.velocity = projectile.velocity.ToRotation().AngleLerp(targetRot, 0.12f * (float)Math.Pow((float)counter / maxTime, 3f)).ToRotationVector2();
                        }

                        projectile.position -= projectile.velocity;
                        projectile.rotation = projectile.velocity.ToRotation();
                    }
                    break;

                case 14: //moon lord vortex telegraph
                    {
                        color = new Color(51, 255, 191);
                        maxTime = 180;
                        alphaModifier = 5;

                        Projectile vortex = FargoSoulsUtil.ProjectileExists(FargoSoulsUtil.GetByUUIDReal(projectile.owner, projectile.ai[1], ModContent.ProjectileType<MoonLordVortex>()));
                        if (vortex != null)
                        {
                            projectile.Center = vortex.Center;

                            projectile.position -= projectile.velocity;
                            projectile.rotation = projectile.velocity.ToRotation();
                        }
                        else if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            projectile.Kill();
                            return;
                        }
                    }
                    break;

                default:
                    break;
            }

            if (++counter > maxTime)
            {
                projectile.Kill();
                return;
            }

            if (alphaModifier >= 0)
            {
                projectile.alpha = 255 - (int)(255 * Math.Sin(Math.PI / maxTime * counter) * alphaModifier);
                if (projectile.alpha < 0)
                    projectile.alpha = 0;
            }

            color.A = 0;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return color * projectile.Opacity * (Main.mouseTextColor / 255f) * 0.9f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //Main.spriteBatch.End(); Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            const int length = 3000;
            Vector2 offset = projectile.rotation.ToRotationVector2() * length / 2f;
            Vector2 position = projectile.Center - Main.screenLastPosition + new Vector2(0f, projectile.gfxOffY) + offset;
            Rectangle destination = new Rectangle((int)position.X, (int)position.Y, length, (int)(rectangle.Height * projectile.scale));

            Color drawColor = projectile.GetAlpha(lightColor);

            for (int j = 0; j < drawLayers; j++)
                Main.spriteBatch.Draw(texture2D13, destination, new Rectangle?(rectangle), drawColor, projectile.rotation, origin2, SpriteEffects.None, 0f);

            //Main.spriteBatch.End(); Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
    }
}