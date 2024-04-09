using FargowiltasSouls.Common.Utilities;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.Bosses.VanillaEternity
{
    public class QueenSlime : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.QueenSlimeBoss);

        public int StompTimer;
        public int StompCounter;
        public int RainTimer;
        public int SpikeCounter;

        public float StompVelocityX;
        public float StompVelocityY;

        public bool SpawnedMinions1;
        public bool SpawnedMinions2;
        public bool GelatinSubjectDR;
        public int RainDirection;

        public bool DroppedSummon;

        private const float StompTravelTime = 60;
        private const float StompGravity = 1.6f;


        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(StompTimer);
            binaryWriter.Write7BitEncodedInt(StompCounter);
            binaryWriter.Write7BitEncodedInt(RainTimer);
            binaryWriter.Write(StompVelocityX);
            binaryWriter.Write(StompVelocityY);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            StompTimer = binaryReader.Read7BitEncodedInt();
            StompCounter = binaryReader.Read7BitEncodedInt();
            RainTimer = binaryReader.Read7BitEncodedInt();
            StompVelocityX = binaryReader.ReadSingle();
            StompVelocityY = binaryReader.ReadSingle();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.25, MidpointRounding.ToEven);

            StompTimer = -360;
        }

        private bool Stompy(NPC npc)
        {
            if (StompTimer == 0) //ready to super stomp
            {
                StompTimer = 1;

                SoundEngine.PlaySound(SoundID.ForceRoarPitched, npc.Center);

                if (FargoSoulsUtil.HostCheck)
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, NPCID.WallofFleshEye);

                npc.netUpdate = true;
                NetSync(npc);
                return false;
            }
            else if (StompTimer > 0 && StompTimer < 30) //give time to react
            {
                StompTimer++;

                npc.rotation = 0;

                if (NPCInAnyTiles(npc))
                    npc.position.Y -= 16;

                return false;
            }
            else if (StompTimer == 30)
            {
                if (!npc.HasValidTarget)
                    npc.TargetClosest(false);

                if (npc.HasValidTarget && StompCounter++ < 3)
                {
                    StompTimer++;

                    npc.ai[1] = 1f;

                    Vector2 target = Main.player[npc.target].Center;
                    for (int i = 0; i < 3; i++)
                    {
                        Tile tile = Framing.GetTileSafely(target);
                        if (tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]))
                            break;
                        target.Y += 16f;
                    }
                    target.Y -= Player.defaultHeight;

                    Vector2 distance = target - npc.Bottom;
                    if (StompCounter == 1 || StompCounter == 2)
                        distance.X += 300f * Math.Sign(Main.player[npc.target].Center.X - npc.Center.X);
                    float time = StompTravelTime;
                    if (StompCounter < 0) //enraged
                        time /= 2;
                    distance.X /= time;
                    distance.Y = distance.Y / time - 0.5f * StompGravity * time;
                    StompVelocityX = distance.X;
                    StompVelocityY = distance.Y;

                    SoundEngine.PlaySound(SoundID.Item92, npc.Center);

                    npc.netUpdate = true;
                    NetSync(npc);
                    return false;
                }
                else //done enough stomps
                {
                    StompCounter = 0;
                    StompTimer = -360;

                    npc.velocity.X = 0;

                    npc.ai[1] = 2000f; //proceed to next thing immediately
                    npc.ai[2] = 1f;
                    npc.netUpdate = true;
                    NetSync(npc);
                }
            }
            else if (StompTimer > 30)
            {
                npc.rotation = 0;
                npc.noTileCollide = true;

                float time = StompTravelTime;
                if (StompCounter < 0) //enraged
                    time /= 2;

                if (++StompTimer > time + 30)
                {
                    npc.noTileCollide = false;

                    //when landed on a surface
                    if (npc.velocity.Y == 0 || NPCInAnyTiles(npc) || StompTimer >= time * 2 + 25)
                    {
                        npc.velocity = Vector2.Zero;

                        if (WorldSavingSystem.MasochistModeReal)
                        {
                            StompTimer = 25;
                        }
                        else
                        {
                            StompTimer = /*NPC.AnyNPCs(ModContent.NPCType<GelatinSlime>()) ? 1 :*/ 15;
                        }

                        if (npc.DeathSound != null)
                            SoundEngine.PlaySound(npc.DeathSound.Value, npc.Center);

                        if (FargoSoulsUtil.HostCheck)
                        {
                            int smashDamage = WorldSavingSystem.MasochistModeReal && Main.getGoodWorld
                                ? FargoSoulsUtil.ScaledProjectileDamage(npc.damage) : 0;
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ProjectileID.QueenSlimeSmash, smashDamage, 0f, Main.myPlayer);

                            for (int j = -1; j <= 1; j += 2) //spray spikes
                            {
                                Vector2 baseVel = Vector2.UnitX.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(10) * j));
                                const int max = 12;
                                for (int i = 0; i < max; i++)
                                {
                                    Vector2 vel = Main.rand.NextFloat(5f, 15f) * j * baseVel.RotatedBy(MathHelper.PiOver4 * 0.8f / max * i * -j);
                                    vel *= WorldSavingSystem.MasochistModeReal ? 2f : 1.5f;
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel, ProjectileID.QueenSlimeMinionBlueSpike, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                                }
                            }
                        }

                        return false;
                    }
                }

                //damn queen slime ai glitching out and not fastfalling properly sometimes
                float correction = StompVelocityY - npc.velocity.Y;
                if (correction > StompGravity)
                    npc.position.Y += correction;

                npc.velocity.X = StompVelocityX;
                npc.velocity.Y = StompVelocityY;
                StompVelocityY += StompGravity;

                return false;
            }

            return true;
        }

        public override bool SafePreAI(NPC npc)
        {
            EModeGlobalNPC.queenSlimeBoss = npc.whoAmI;

            if (WorldSavingSystem.SwarmActive)
                return true;

            void TrySpawnMinions(ref bool check, double threshold)
            {
                if (!check && npc.life < npc.lifeMax * threshold)
                {
                    check = true;

                    FargoSoulsUtil.PrintLocalization($"Mods.{Mod.Name}.NPCs.EMode.GelatinSubjects", new Color(175, 75, 255));

                    for (int i = 0; i < 7; i++)
                    {
                        FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, ModContent.NPCType<GelatinSubject>(), npc.whoAmI, target: npc.target,
                            velocity: Main.rand.NextFloat(8f) * npc.DirectionFrom(Main.player[npc.target].Center).RotatedByRandom(MathHelper.PiOver2));
                    }

                    if (FargoSoulsUtil.HostCheck)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.type);
                    }
                }
            }

            TrySpawnMinions(ref SpawnedMinions1, 0.75);
            TrySpawnMinions(ref SpawnedMinions2, 0.25);

            GelatinSubjectDR = NPC.AnyNPCs(ModContent.NPCType<GelatinSubject>());
            npc.HitSound = GelatinSubjectDR ? SoundID.Item27 : SoundID.NPCHit1;

            //ai0
            //0 = default
            //3 = chase?
            //4 = stomp
            //5 = shooty gels

            if (npc.ai[0] == 5) //when shooting, p1 and p2
            {
                if (NPC.AnyNPCs(ModContent.NPCType<GelatinSubject>()))
                    npc.ai[1] -= 0.5f;

                if (npc.ai[1] == 45 && --SpikeCounter < 0) //every few shots
                {
                    SpikeCounter = 4;
                    NetSync(npc);

                    SoundEngine.PlaySound(SoundID.Roar, npc.Center);

                    if (FargoSoulsUtil.HostCheck)
                    {
                        Vector2 focus = Main.player[npc.target].Center;
                        for (int i = 0; i < 50; i++)
                        {
                            Tile tile = Framing.GetTileSafely(focus);
                            if (tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]))
                                break;
                            focus.Y += 16f;
                        }
                        focus.Y -= Player.defaultHeight / 2f;

                        for (int i = -5; i <= 5; i++)
                        {
                            Vector2 targetPos = focus;
                            targetPos.X += 330 * i;

                            float minionTravelTime = StompTravelTime + Main.rand.Next(30);
                            float minionGravity = 0.4f;
                            Vector2 vel = targetPos - npc.Center;
                            vel.X /= minionTravelTime;
                            vel.Y = vel.Y / minionTravelTime - 0.5f * minionGravity * minionTravelTime;

                            FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, ModContent.NPCType<GelatinSlime>(), npc.whoAmI, minionTravelTime, minionGravity, vel.X, vel.Y, target: npc.target);
                        }
                    }
                }
            }

            if (npc.life > npc.lifeMax / 2) //phase 1
            {
                if (StompTimer > 0 || npc.ai[0] == 0 && npc.velocity.Y == 0)
                {
                    if (StompTimer < 0)
                        StompTimer++;
                    else
                        npc.ai[0] = 4; //activates trail visual

                    if (!Stompy(npc))
                        return false;
                }
            }
            else //phase 2
            {
                npc.defense = npc.defDefense / 2;

                if (RainTimer < 0)
                    RainTimer++;

                if (RainTimer <= 0 && StompTimer < 0) //dont run timer during rain attack
                    StompTimer++;

                if (npc.ai[0] == 0) //basic flying ai
                {
                    if (RainTimer == 0)
                    {
                        if (npc.velocity.Y < 0)
                            npc.position.Y += npc.velocity.Y;

                        npc.ai[1] -= 1; //dont progress to next ai

                        if (npc.HasValidTarget && Math.Abs(npc.Center.Y - (Main.player[npc.target].Center.Y - 250)) < 32)
                        {
                            RainTimer = 1; //begin attack
                            NetSync(npc);

                            npc.netUpdate = true;

                            SoundEngine.PlaySound(SoundID.Roar, npc.Center);

                            if (FargoSoulsUtil.HostCheck)
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, -16);
                        }
                    }
                    else if (RainTimer > 0) //actually doing rain
                    {
                        npc.velocity.X *= 0.9f;

                        npc.ai[1] -= 1f; //dont progress ai

                        RainTimer++;

                        const int delay = 45;
                        const int timeBeforeStreamsMove = 45;
                        const int maxAttackTime = 480;
                        int attackTimer = RainTimer - delay - timeBeforeStreamsMove;
                        if (attackTimer < 0)
                            attackTimer = 0;

                        if (RainTimer == delay)
                            RainDirection = Math.Sign(Main.player[npc.target].Center.X - npc.Center.X);

                        if (RainTimer > delay && RainTimer < delay + maxAttackTime && RainTimer % 5 == 0)
                        {
                            const float maxWavy = 200;
                            Vector2 focusPoint = new(npc.Center.X, Math.Min(npc.Center.Y, Main.player[npc.target].Center.Y));
                            focusPoint.X += maxWavy * RainDirection * (float)Math.Sin(Math.PI * 2f / maxAttackTime * attackTimer * 1.5f);
                            focusPoint.Y -= 500;

                            for (int i = -4; i <= 4; i++)
                            {
                                Vector2 spawnPos = focusPoint + Main.rand.NextVector2Circular(32, 32);
                                spawnPos.X += 330 * i;
                                if (FargoSoulsUtil.HostCheck)
                                {
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos, 8f * Vector2.UnitY,
                                      ProjectileID.QueenSlimeMinionBlueSpike, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                                }
                            }
                        }

                        bool endAttack = RainTimer > delay + maxAttackTime + 90;
                        if (npc.Distance(Main.player[npc.target].Center) > 1200)
                        {
                            endAttack = true;

                            StompTimer = 0;
                            StompCounter = -3; //enraged super stomps
                        }

                        if (!npc.HasValidTarget)
                        {
                            npc.TargetClosest(false);
                            if (!npc.HasValidTarget)
                                endAttack = true;
                        }

                        if (endAttack)
                        {
                            RainTimer = -1000;
                            npc.netUpdate = true;
                            NetSync(npc);

                            if (StompTimer == 0) //transition directly to stompy if ready
                            {
                                npc.ai[0] = 4f;
                                npc.ai[1] = 0f;
                            }
                        }
                    }
                    else
                    {
                        npc.ai[1] += 1; //proceed to next ais faster
                    }
                }
                else if (npc.ai[0] == 4) //stompy
                {
                    if (!Stompy(npc))
                        return false;

                    if (!WorldSavingSystem.MasochistModeReal)
                    {
                        if (npc.ai[1] == 0)
                            SoundEngine.PlaySound(npc.DeathSound, npc.Center);

                        if (npc.ai[1] < 70) //artificial startup on this stupid no-telegraph dive
                        {
                            float ratio = 1f - npc.ai[1] / 70;
                            npc.position.Y -= npc.velocity.Y * ratio;
                        }
                    }
                }
                else if (npc.ai[0] == 5) //when shooting
                {
                    //be careful to stay above player
                    if (npc.HasValidTarget && npc.Bottom.Y > Main.player[npc.target].Top.Y - 80 && npc.velocity.Y > -8f)
                        npc.velocity.Y -= 0.8f;
                }
            }

            //FargoSoulsUtil.PrintAI(npc);

            EModeUtils.DropSummon(npc, "JellyCrystal", NPC.downedQueenSlime, ref DroppedSummon, Main.hardMode);

            return true;
        }

        private static bool NPCInAnyTiles(NPC npc)
        {
            //WHERE'S TJHE FKC IJNGI METHOD FOR HTIS? ITS NOT COLLISION.SOLKIDCOLLIOSOM ITS NOPT COLLISON.SOLDITILES I HATE 1.4 IHATE TMODLAOREI I HATE THIS FUSPTID FUCKIGN GNAME SOFU KIGN MCUCH FUCK FUCK FUCK
            bool isInTilesIncludingPlatforms = false;
            for (int x = 0; x < npc.width; x += 16)
            {
                for (float y = npc.height / 2; y < npc.height; y += 16)
                {
                    Tile tile = Framing.GetTileSafely((int)(npc.position.X + x) / 16, (int)(npc.position.Y + y) / 16);
                    if (tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]))
                    {
                        isInTilesIncludingPlatforms = true;
                        break;
                    }
                }
            }

            return isInTilesIncludingPlatforms;
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (npc.life < npc.lifeMax / 2)
                modifiers.FinalDamage *= 0.8f;

            if (GelatinSubjectDR)
                modifiers.FinalDamage *= 0.25f;

            base.ModifyIncomingHit(npc, ref modifiers);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Slimed, 240);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadBossHeadSprite(recolor, 38);
            LoadGore(recolor, 1258);
            LoadGore(recolor, 1259);
            LoadExtra(recolor, 177);
            LoadExtra(recolor, 180);
            LoadExtra(recolor, 185);
        }
    }

    public class QueenSlimeMinion : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.QueenSlimeMinionBlue,
            NPCID.QueenSlimeMinionPink,
            NPCID.QueenSlimeMinionPurple
        );

        public bool TimeToFly;
        public bool Landed;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            bitWriter.WriteBit(TimeToFly);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            TimeToFly = bitReader.ReadBit();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            if (WorldSavingSystem.MasochistModeReal)
                npc.knockBackResist = 0;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (WorldSavingSystem.MasochistModeReal)
            {
                if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.queenSlimeBoss, NPCID.QueenSlimeBoss))
                {
                    Vector2 target = Main.player[Main.npc[EModeGlobalNPC.queenSlimeBoss].target].Top;
                    if (TimeToFly)
                    {

                        npc.velocity = Math.Min(npc.velocity.Length(), 20f) * npc.SafeDirectionTo(target);
                        npc.position += 8f * npc.SafeDirectionTo(target);

                        if (npc.Distance(target) < 300f)
                        {
                            TimeToFly = false;
                            NetSync(npc);

                            npc.velocity += 8f * npc.SafeDirectionTo(target).RotatedByRandom(MathHelper.PiOver4);
                            npc.netUpdate = true;
                        }
                    }
                    else if (npc.Distance(target) > 900f)
                    {
                        TimeToFly = true;
                        NetSync(npc);
                    }
                }
                else
                {
                    TimeToFly = false;
                }

                npc.noTileCollide = TimeToFly;
            }
            else
            {
                npc.localAI[0] = 30f; //prevent firing

                if (npc.type == NPCID.QueenSlimeMinionPurple)
                {
                    npc.position -= npc.velocity * 0.5f;
                }
                else
                {
                    if (!Landed) //tl;dr dont fall on the player
                    {
                        if (npc.velocity.Y == 0)
                            Landed = true;

                        Player p = FargoSoulsUtil.PlayerExists(Player.FindClosest(npc.Center, 0, 0));
                        if (p != null)
                        {
                            const float minDist = 16 * 8;
                            float dist = npc.Center.X - p.Center.X;
                            if (Math.Abs(dist) < minDist)
                            {
                                npc.velocity.X *= 0.95f;
                                npc.velocity.X += (minDist - Math.Abs(dist)) * Math.Sign(dist) * 0.05f;
                            }
                        }
                    }
                }
            }

            //if (npc.velocity.Y != 0)
            //    npc.localAI[0] = 25f;
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Slimed, 180);
            target.AddBuff(ModContent.BuffType<SmiteBuff>(), 360);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
            LoadGore(recolor, 1260);
        }
    }
}
