﻿using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Common.Utilities;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.NPCs;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.Bosses.VanillaEternity
{
    public class KingSlime : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.KingSlime);

        public int SpikeRainCounter; // Was Counter[0]

        public bool IsBerserk; // Was masoBool[0]
        public bool LandingAttackReady; // Was masoBool[1]
        public bool CurrentlyJumping; // Was masoBool[3]
        public bool DidSpecialTeleport;
        public int CertainAttackCooldown;

        public bool DroppedSummon;

        public float JumpTimer = 0;
        const int SpecialJumpTime = 60 * 15;
        public int SpecialJumpWindupTimer;

        const int SummonWaves = 6;
        public float SummonCounter = SummonWaves - 1;
        public bool SpecialJumping = false;

        public int DeathTimer = -1;
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write7BitEncodedInt(DeathTimer);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            DeathTimer = binaryReader.Read7BitEncodedInt();
        }
        public override bool SafePreAI(NPC npc)
        {
            if (DeathTimer >= 0) 
            {
                DeathAnimation(npc);
                if (++DeathTimer >= 300)
                {
                    npc.life = 0;
                    npc.dontTakeDamage = false;
                    npc.checkDead();
                }
                return false;
            }

            EModeGlobalNPC.slimeBoss = npc.whoAmI;
            // npc.color = Main.DiscoColor * 0.3f; // Rainbow colour

            ref float teleportTimer = ref npc.ai[2];

            if (CertainAttackCooldown > 0)
                CertainAttackCooldown--;

            Player player = Main.player[npc.target];

            /*
            if (JumpTimer < SpecialJumpTime)
            {
                JumpTimer += Math.Min(2 - npc.GetLifePercent(), SpecialJumpTime - JumpTimer);
            }
            */
            if (teleportTimer >= 145 && teleportTimer < 150) //at half of teleport timer progress, pause it and do special jump
            {
                if (JumpTimer < SpecialJumpTime)
                    JumpTimer = SpecialJumpTime;
                teleportTimer = 145;
            }
            if (npc.GetLifePercent() < SummonCounter / SummonWaves && (CertainAttackCooldown <= 0 || WorldSavingSystem.MasochistModeReal))
            {
                const int Slimes = 6;
                CertainAttackCooldown = 180;
                if (FargoSoulsUtil.HostCheck)
                {
                    for (int i = 0; i < Slimes; i++)
                    {
                        int x = (int)(npc.position.X + Main.rand.NextFloat(npc.width - 32));
                        int y = (int)(npc.position.Y + Main.rand.NextFloat(npc.height - 32));
                        int type = ModContent.NPCType<SlimeSwarm>();
                        int slime = NPC.NewNPC(npc.GetSource_FromThis(), x, y, type);
                        if (slime.IsWithinBounds(Main.maxNPCs))
                        {
                            Main.npc[slime].SetDefaults(type);
                            Main.npc[slime].velocity.X = Main.rand.NextFloat(-15, 16) * 0.1f;
                            Main.npc[slime].velocity.Y = Main.rand.NextFloat(-30, -15) * 0.3f;

                            if (npc.HasValidTarget)
                            {
                                Main.npc[slime].ai[0] = Math.Sign(player.Center.X - npc.Center.X);
                                Main.npc[slime].velocity.X = Main.rand.NextFloat(10, 16) * 0.4f * -npc.HorizontalDirectionTo(player.Center);
                            }

                            //Main.npc[slime].ai[0] = -1000 * Main.rand.Next(3);
                            //Main.npc[slime].ai[1] = 0f;
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, slime);
                            }
                        }
                    }
                }

                SoundEngine.PlaySound(SoundID.Item167, npc.Center);
                SummonCounter--;
            }

            if (WorldSavingSystem.MasochistModeReal)
                npc.position.X += npc.velocity.X * 0.2f;
            //FargoSoulsUtil.PrintAI(npc);
            // Attack that happens when landing
            if (LandingAttackReady)
            {
                if (npc.velocity.Y == 0f)
                {
                    LandingAttackReady = false;


                    if (JumpTimer >= SpecialJumpTime && !SpecialJumping && (CertainAttackCooldown <= 0 || WorldSavingSystem.MasochistModeReal))
                    {
                        SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/VanillaEternity/KingSlime/KSCharge"), npc.Center);
                        Particle p = new ExpandingBloomParticle(npc.Center, Vector2.Zero, Color.Blue, Vector2.One, Vector2.One * 60, 40, true, Color.Transparent);
                        SpecialJumping = true;
                        CertainAttackCooldown = 240;
                        SpecialJumpWindupTimer = 60;
                        p.Spawn();

                    }
                    else
                    {
                        if (SpecialJumping)
                        {
                            JumpTimer = 0;
                            SpecialJumping = false;
                            teleportTimer = 150; //continue teleport timer
                        }
                        else
                        {
                            if (FargoSoulsUtil.HostCheck)
                            {
                                /*
                                if (WorldSavingSystem.MasochistModeReal)
                                {
                                    for (int i = 0; i < 30; i++) //spike spray
                                    {
                                        Projectile.NewProjectile(npc.GetSource_FromThis(), new Vector2(npc.Center.X + Main.rand.Next(-5, 5), npc.Center.Y - 15),
                                            new Vector2(Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-8, -5)),
                                            ProjectileID.SpikedSlimeSpike, FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0f, Main.myPlayer);
                                    }
                                }
                                */

                                
                                if (WorldSavingSystem.MasochistModeReal && npc.HasValidTarget)
                                {
                                    SoundEngine.PlaySound(SoundID.Item21, player.Center);
                                    if (FargoSoulsUtil.HostCheck)
                                    {
                                        for (int i = 0; i < 6; i++)
                                        {
                                            Vector2 spawn = player.Center;
                                            spawn.X += Main.rand.Next(-150, 151);
                                            spawn.Y -= Main.rand.Next(600, 901);
                                            Vector2 speed = player.Center - spawn;
                                            speed.Normalize();
                                            speed *= IsBerserk ? 10f : 5f;
                                            speed = speed.RotatedByRandom(MathHelper.ToRadians(4));
                                            Projectile.NewProjectile(npc.GetSource_FromThis(), spawn, speed, ModContent.ProjectileType<SlimeBallHostile>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage, 4f / 6), 0f, Main.myPlayer);
                                        }
                                    }
                                }
                                
                            }
                        }
                    }

                }
            }
            else if (npc.velocity.Y > 0)
            {
                // If they're in the air, flag that the landing attack should be used next time they land
                LandingAttackReady = true;
            }

            if (npc.velocity.Y < 0) // Jumping up
            {
                if (!CurrentlyJumping) // Once per jump...
                {
                    CurrentlyJumping = true;


                    if (SpecialJumping) //special jump
                    {
                        npc.velocity.Y = -18;
                        int direction = Math.Sign(player.Center.X - npc.Center.X);
                        int pastPlayer = 1000;
                        Vector2 desiredDestination = player.Center + (Vector2.UnitX * pastPlayer * direction);

                        //funny highschool physics math
                        float jumpTime = Math.Abs(2 * npc.velocity.Y / npc.gravity);
                        npc.velocity.X = (desiredDestination.X - npc.Center.X) / jumpTime;
                        SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/VanillaEternity/KingSlime/KSJump"), npc.Center);
                    }
                    else
                    {

                        bool shootSpikes = false;

                        if (WorldSavingSystem.MasochistModeReal)
                            shootSpikes = true;



                        if (npc.HasValidTarget)
                        {
                            // If player is well above me, jump higher
                            if (player.Center.Y < npc.position.Y + npc.height - 240)
                            {
                                npc.velocity.Y *= 1.5f;
                                //shootSpikes = true;
                            }

                            //jump longer when player is further than threshold, scaling with distance up to cap
                            const int XThreshold = 0;
                            float xDif = Math.Abs(player.Center.X - npc.Center.X);
                            if (xDif > XThreshold)
                            {
                                float modifier = xDif - XThreshold;
                                modifier /= 700f;
                                modifier *= modifier;
                                modifier += 1;
                                modifier = MathHelper.Clamp(modifier, 1, 3);
                                npc.velocity.X *= modifier;
                                npc.velocity.Y *= Math.Min((float)Math.Cbrt(modifier), 1.5f);

                                // Flat addition
                                npc.velocity.X += Math.Sign(npc.velocity.X) * 2.25f;
                            }

                        }
                        if (npc.ai[1] == 0) // big jump
                            shootSpikes = false;

                        if (shootSpikes && FargoSoulsUtil.HostCheck)
                        {
                            const float gravity = 0.15f;
                            float time = 90f;
                            Vector2 distance = player.Center - npc.Center + player.velocity * 30f;
                            distance.X /= time;
                            distance.Y = distance.Y / time - 0.5f * gravity * time;
                            for (int i = 0; i < 15; i++)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, distance + Main.rand.NextVector2Square(-1f, 1f),
                                    ModContent.ProjectileType<SlimeSpike>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0f, Main.myPlayer);
                            }
                        }
                    }
                }
            }
            else
            {
                CurrentlyJumping = false;
            }

            if (npc.velocity.Y == 0) //on ground
            {
                if (SpecialJumpWindupTimer > 0)
                {
                    npc.ai[0] = -999; // no jumping until this is done
                    SpecialJumpWindupTimer--;
                    if (SpecialJumpWindupTimer == 0)
                        npc.ai[0] = -1; // ok now you can jump
                }

            }
            else //midair
            {
                if (SpecialJumping) //special jump
                {
                    JumpTimer++;



                    const int ProjTime = 5;
                    if (Math.Sign(npc.velocity.X) != Math.Sign(npc.DirectionTo(player.Center).X) && Math.Abs(npc.Center.X - player.Center.X) > 250 && npc.velocity.Y > 0)
                    {
                        npc.velocity.X /= 5;
                        SpecialJumping = false;
                        JumpTimer = 0;
                        teleportTimer = 150; //continue teleport timer
                    }

                    else if (JumpTimer % ProjTime < 1 && (JumpTimer % (ProjTime * 3) > 1 || WorldSavingSystem.MasochistModeReal))
                    {
                        SoundEngine.PlaySound(SoundID.Item17, npc.Center);
                        if (FargoSoulsUtil.HostCheck)
                        {
                            Vector2 spawnPos = npc.Bottom;
                            Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos, Vector2.Zero,
                                ModContent.ProjectileType<SlimeSpike2>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage, 4f / 6), 0f, Main.myPlayer);
                        }
                    }
                }
            }


            if ((IsBerserk || npc.life < npc.lifeMax * .66f) && npc.HasValidTarget && !SpecialJumping)
            {
                if (--SpikeRainCounter < 0) // Spike rain
                {
                    SpikeRainCounter = 240;

                    if (FargoSoulsUtil.HostCheck)
                    {
                        const int Gap = 110;
                        Vector2 spawnPos = player.Center + (Vector2.UnitX * Main.rand.Next(-Gap / 2, Gap / 2));
                        for (int i = -12; i <= 12; i++)
                        {
                            Vector2 spikePos = spawnPos;
                            spikePos.X += Gap * i;
                            spikePos.Y -= 500;
                            Projectile.NewProjectile(npc.GetSource_FromThis(), spikePos, (IsBerserk ? 6f : 0f) * Vector2.UnitY,
                                ModContent.ProjectileType<SlimeSpike2>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage, 4f / 6), 0f, Main.myPlayer);
                        }
                    }
                }
            }

            /*if (!masoBool[0]) //is not berserk
            {
                SharkCount = 0;

                if (npc.HasPlayerTarget)
                {
                    Player player = player;
                    if (player.active && !player.dead && player.Center.Y < npc.position.Y && npc.Distance(player.Center) < 1000f)
                    {
                        Counter[1]++; //timer runs if player is above me and nearby
                        if (Counter[1] >= 600 && FargoSoulsUtil.HostCheck) //go berserk
                        {
                            masoBool[0] = true;
                            npc.netUpdate = true;
                            NetUpdateMaso(npc.whoAmI);
                            if (Main.netMode == NetmodeID.Server)
                                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("King Slime has enraged!"), new Color(175, 75, 255));
                            else
                                Main.NewText("King Slime has enraged!", 175, 75, 255);
                        }
                    }
                    else
                    {
                        Counter[1] = 0;
                    }
                }
            }
            else //is berserk
            {
                SharkCount = 1;

                if (!masoBool[2])
                {
                    masoBool[2] = true;
                    SoundEngine.PlaySound(SoundID.Roar, npc.Center);
                }

                if (Counter[0] > 45) //faster slime spike rain
                    Counter[0] = 45;

                if (++Counter[2] > 30) //aimed spikes
                {
                    Counter[2] = 0;
                    const float gravity = 0.15f;
                    float time = 45f;
                    Vector2 distance = player.Center - npc.Center + player.velocity * 30f;
                    distance.X = distance.X / time;
                    distance.Y = distance.Y / time - 0.5f * gravity * time;
                    for (int i = 0; i < 15; i++)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, distance + Main.rand.NextVector2Square(-1f, 1f) * 2f,
                            ModContent.ProjectileType<SlimeSpike>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer);
                    }
                }

                if (npc.HasValidTarget && FargoSoulsUtil.HostCheck && player.position.Y > npc.position.Y) //player went back down
                {
                    masoBool[0] = false;
                    masoBool[2] = false;
                    NetUpdateMaso(npc.whoAmI);
                }
            }*/

            if (npc.ai[1] == 5) //when teleporting
            {
                if (npc.HasPlayerTarget && npc.ai[0] == 1) //update y pos once
                    npc.localAI[2] = player.Center.Y;

                Vector2 tpPos = new(npc.localAI[1], npc.localAI[2]);
                tpPos.X -= npc.width / 2;

                for (int i = 0; i < 10; i++)
                {
                    int d = Dust.NewDust(tpPos, npc.width, npc.height / 2, DustID.t_Slime, 0, 0, 75, new Color(78, 136, 255, 80), 2.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity.Y -= 3f;
                    Main.dust[d].velocity *= 3f;
                }
            }

            /* special teleport
            if (npc.life < npc.lifeMax / 3)
            {
                
                if (npc.ai[1] == 5) //when teleporting
                {
                    if (npc.ai[0] == 1 && !DidSpecialTeleport)
                        SoundEngine.PlaySound(SoundID.Roar, npc.Center);

                    if (npc.HasPlayerTarget && !DidSpecialTeleport) //live update tp position
                    {
                        Vector2 desiredTeleport = player.Center;
                        desiredTeleport.X += 800 * System.Math.Sign(player.Center.X - npc.Center.X); //tp ahead of player

                        if (Collision.CanHitLine(desiredTeleport, 0, 0, player.position, player.width, player.height))
                        {
                            npc.localAI[1] = desiredTeleport.X;
                            npc.localAI[2] = desiredTeleport.Y;
                        }
                    }
                }
                else if (npc.ai[1] == 6) //actually did the teleport and now regrowing
                {
                    DidSpecialTeleport = true;
                }
                else
                {
                    if (!DidSpecialTeleport)
                        teleportTimer += 60;

                    teleportTimer += 1f / 3f; //always increment the teleport timer
                }
                
            }
            */
            // Drop summon
            EModeUtils.DropSummon(npc, "SlimyCrown", NPC.downedSlimeKing, ref DroppedSummon);

            return base.SafePreAI(npc);
        }
        public override bool? CanFallThroughPlatforms(NPC npc)
        {
            if (SpecialJumping && !LandingAttackReady)
            {
                return false;
            }
            return base.CanFallThroughPlatforms(npc);
        }

        public override bool CheckDead(NPC npc)
        {
            if (DeathTimer != -1)
                return true;

            // Dont do the anim if mutant already exists
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<MutantBoss.MutantBoss>())
                || (ModContent.TryFind("Fargowiltas", "Mutant", out ModNPC mutant) && NPC.AnyNPCs(mutant.Type)) || !SoulConfig.Instance.BossRecolors)
            {
                return true;
            }

            if (WorldSavingSystem.HaveForcedMutantFromKS)
                return true;

            npc.life = 1;
            npc.active = true;

            // remove normal crown gore (manually spawned later)
            foreach (Gore gore in Main.gore.Where(g => g.active && g.type == GoreID.KingSlimeCrown))
                gore.active = false;
            DeathTimer++;
            npc.dontTakeDamage = true;
            FargoSoulsUtil.ClearHostileProjectiles(2, npc.whoAmI);
            npc.netUpdate = true;

            return false;
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (FargoSoulsUtil.HostCheck
                && !FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<MutantBoss.MutantBoss>())
                && ModContent.TryFind("Fargowiltas", "Mutant", out ModNPC mutant) && !NPC.AnyNPCs(mutant.Type))
            {   
                
                // manual gore spawn
                Gore.NewGore(npc.GetSource_FromThis(), npc.Center, -15 * Vector2.UnitY, GoreID.KingSlimeCrown);

                int n = NPC.NewNPC(npc.GetSource_FromThis(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<ReleasedMutant>());
                if (n != Main.maxNPCs && Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Slimed, 60);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
            LoadBossHeadSprite(recolor, 7);
            LoadGore(recolor, 734);
            LoadExtra(recolor, 39);

            LoadSpecial(recolor, ref TextureAssets.Ninja, ref FargowiltasSouls.TextureBuffer.Ninja, "Ninja");
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            bool resprite = WorldSavingSystem.EternityMode && SoulConfig.Instance.BossRecolors;
            if (!resprite)
                return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
            // Draw the Ninja (Mutant).
            var ninjaOffset = new Vector2(-npc.velocity.X * 2f, -npc.velocity.Y);
            var ninjaRotation = npc.velocity.X * 0.05f;
            
            switch (npc.frame.Y)
            {
                case 120:
                    ninjaOffset.Y += 2f;
                    break;

                case 360:
                    ninjaOffset.Y -= 2f;
                    break;

                case 480:
                    ninjaOffset.Y -= 6f;
                    break;
            }
            
            spriteBatch.Draw(TextureAssets.Ninja.Value, new Vector2(npc.position.X - screenPos.X + npc.width / 2f + ninjaOffset.X, npc.position.Y - screenPos.Y + npc.height / 2f + ninjaOffset.Y), new Rectangle(0, 0, TextureAssets.Ninja.Width(), TextureAssets.Ninja.Height()), drawColor, ninjaRotation, TextureAssets.Ninja.Size() / 2f, 1f, SpriteEffects.None, 0f);
            
            // We have to manually set drawing to immediate mode when rendering
            // the NPC normally.  Bestiary icons don't have this requirement.
            if (!npc.IsABestiaryIconDummy)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);
            }

            // Can also use npc.type instead.
            var ksTexture = TextureAssets.Npc[NPCID.KingSlime].Value;
            
            // Render KS through DrawData since we need to apply a game shader.
            var frameCount = Main.npcFrameCount[npc.type];
            var frameVertical = npc.frame.Y / npc.frame.Height;
            var frame = ksTexture.Frame(1, frameCount, 0, frameVertical);
            frame.Inflate(0, -2);
            
            var drawData = new DrawData(ksTexture, npc.Bottom - screenPos + new Vector2(0f, 2f), frame, drawColor /*with { A = 200 }*/, npc.rotation, frame.Size() * new Vector2(0.5f, 1f), npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            GameShaders.Misc["FargowiltasSouls:KingSlime"].Apply(drawData);
            drawData.Draw(spriteBatch);
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            
            if (!npc.IsABestiaryIconDummy)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }
            
            // Render the crown normally without the shader.
            var crownTexture = TextureAssets.Extra[39].Value;
            var center = npc.Center;

            var yOffset = (npc.frame.Y / (ksTexture.Height / Main.npcFrameCount[NPCID.KingSlime])) switch
            {
                0 => 2f,
                1 => -6f,
                2 => 2f,
                3 => 10f,
                4 => 2f,
                5 => 0f,
                _ => 0f,
            };

            var spriteEffects = npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            center.Y += npc.gfxOffY - (70f - yOffset) * npc.scale;
            spriteBatch.Draw(crownTexture, center - screenPos, null, Color.White, 0f, crownTexture.Size() / 2f, 1f, spriteEffects, 0f);
            return false;
        }

        public void DeathAnimation(NPC npc)
        {
            Particle p;
            float scaleMult;
            int screenshake = 3;
            npc.velocity.X *= 0.9f;
            Vector2 mutantEyePos = npc.Center + new Vector2(-5f, -12f);
            // Dust
            if (Main.rand.NextBool(5))
            {          
                SoundEngine.PlaySound(npc.HitSound, npc.Center);
            }
            bool recolor = SoulConfig.Instance.BossRecolors && WorldSavingSystem.EternityMode;
            Dust.NewDust(npc.TopLeft, npc.width, npc.height, DustID.t_Slime);

            if (DeathTimer == 100 || DeathTimer == 200 || DeathTimer == 250)
            {
                screenshake += 2;
                FargoSoulsUtil.ScreenshakeRumble(screenshake);
                SoundEngine.PlaySound(FargosSoundRegistry.MutantSword with { Volume = 0.6f}, npc.Center);
            }

            // initial charge up
            if (DeathTimer >= 180 && DeathTimer < 270)
            {
                Vector2 pos = npc.Center + 5 * Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi);
                scaleMult = (DeathTimer - 180) / 23f;
                p = new SparkParticle(pos, Vector2.UnitX.RotatedBy((pos - npc.Center).ToRotation()), Color.Teal, scaleMult * 0.1f, 10);
                p.Spawn();
            }

            if (DeathTimer >= 270)
            {
                // eye glow
                scaleMult = (DeathTimer - 270) / 14f;
                p = new SparkParticle(mutantEyePos, Vector2.UnitY, Color.Teal, 1.5f, 120);
                p.Scale *= scaleMult;
                p.Spawn();
                p = new SparkParticle(mutantEyePos, Vector2.UnitX, Color.Teal, 1.5f, 120);
                p.Scale *= scaleMult;
                p.Spawn();

                // explosions
                if (DeathTimer % 5 == 0)
                {
                    if (FargoSoulsUtil.HostCheck) 
                    {
                        Vector2 spawnPos = npc.position + new Vector2(Main.rand.Next(npc.width), Main.rand.Next(npc.height));
                        int type = ModContent.ProjectileType<MutantBombSmall>();
                        Projectile proj = Projectile.NewProjectileDirect(npc.GetSource_FromAI(), spawnPos, Vector2.Zero, type, 0, 0f, Main.myPlayer);
                        proj.scale *= 0.43f * scaleMult;
                    }
                    SoundEngine.PlaySound(SoundID.Item14, npc.Center);
                    FargoSoulsUtil.ScreenshakeRumble((DeathTimer - 270) / 15f);
                }
            }
            // grand finale
            if (DeathTimer == 298)
            {
                FargoSoulsUtil.ScreenshakeRumble(7f);
                SoundEngine.PlaySound(FargosSoundRegistry.MutantKSKill, npc.Center);
            }
        }
    }
    /*
    public class KingSlimeMinionRemovalHack : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher()
        {
            return new();
        }
        bool KILL = false;
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (source is EntitySource_Parent parent && parent.Entity is NPC sourceNPC && sourceNPC.type == NPCID.KingSlime)
            {
                Main.NewText("yeetus deletus");
                DELETE(npc);
                KILL = true;
            }
        }
        public override bool SafePreAI(NPC npc)
        {
            if (KILL)
            {
                DELETE(npc);
            }
            return base.SafePreAI(npc);
        }
        public override void SafePostAI(NPC npc)
        {
            if (KILL)
            {
                DELETE(npc);
            }
            base.SafePostAI(npc);
        }
        void DELETE(NPC npc)
        {
            npc.life = 0;
            npc.HitEffect();
            npc.checkDead();
            npc.active = false;
            npc.timeLeft = 0;
            npc = null;
        }
    }
    */
}
