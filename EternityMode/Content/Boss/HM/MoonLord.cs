using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Deathrays;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Boss.HM
{
    public abstract class MoonLord : EModeNPCBehaviour
    {
        public abstract int GetVulnerabilityState(NPC npc);

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[ModContent.BuffType<ClippedWings>()] = true;
            npc.buffImmune[ModContent.BuffType<Lethargic>()] = true;
            npc.buffImmune[BuffID.Suffocation] = true;
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int CooldownSlot)
        {
            return false;
        }

        public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
        {
            int masoStateML = GetVulnerabilityState(npc);
            if (item.CountsAsClass(DamageClass.Melee) && masoStateML > 0 && masoStateML < 4 && !player.buffImmune[ModContent.BuffType<NullificationCurse>()] && !FargoSoulsWorld.SwarmActive)
                return false;

            return base.CanBeHitByItem(npc, player, item);
        }

        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            if (!Main.player[projectile.owner].buffImmune[ModContent.BuffType<NullificationCurse>()] && !FargoSoulsWorld.SwarmActive)
            {
                switch (GetVulnerabilityState(npc))
                {
                    case 0: if (!projectile.CountsAsClass(DamageClass.Melee)) return false; break;
                    case 1: if (!projectile.CountsAsClass(DamageClass.Ranged)) return false; break;
                    case 2: if (!projectile.CountsAsClass(DamageClass.Magic)) return false; break;
                    case 3: if (!FargoSoulsUtil.IsSummonDamage(projectile)) return false; break;
                    default: break;
                }
            }

            return base.CanBeHitByProjectile(npc, projectile);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
        }
    }

    public class MoonLordCore : MoonLord
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.MoonLordCore);

        public override int GetVulnerabilityState(NPC npc) => VulnerabilityState;

        public int VulnerabilityState;
        public int AttackMemory;

        public float VulnerabilityTimer;
        public float AttackTimer;

        public bool EnteredPhase2;
        public bool SpawnedRituals;

        public bool DroppedSummon;
        public int SkyTimer;



        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(VulnerabilityState);
            binaryWriter.Write7BitEncodedInt(AttackMemory);
            binaryWriter.Write(VulnerabilityTimer);
            binaryWriter.Write(AttackTimer);
            bitWriter.WriteBit(EnteredPhase2);
            bitWriter.WriteBit(SpawnedRituals);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            VulnerabilityState = binaryReader.Read7BitEncodedInt();
            AttackMemory = binaryReader.Read7BitEncodedInt();
            VulnerabilityTimer = binaryReader.ReadSingle();
            AttackTimer = binaryReader.ReadSingle();
            EnteredPhase2 = bitReader.ReadBit();
            SpawnedRituals = bitReader.ReadBit();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lifeMax = (int)Math.Round(npc.lifeMax * 2.5);
        }

        public override bool SafePreAI(NPC npc)
        {
            bool result = base.SafePreAI(npc);

            EModeGlobalNPC.moonBoss = npc.whoAmI;

            if (FargoSoulsWorld.SwarmActive)
                return result;

            if (!SpawnedRituals)
            {
                SpawnedRituals = true;
                VulnerabilityState = 0;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<LunarRitual>(), 25, 0f, Main.myPlayer, 0f, npc.whoAmI);
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<FragmentRitual>(), 0, 0f, Main.myPlayer, 0f, npc.whoAmI);
                }
            }

            if (Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost && VulnerabilityState >= 0 && VulnerabilityState <= 3)
                Main.LocalPlayer.AddBuff(ModContent.BuffType<NullificationCurse>(), 2);

            npc.position -= npc.velocity * 2f / 3f; //SLOW DOWN

            if (npc.dontTakeDamage)
            {
                if (AttackTimer == 370 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        NPC bodyPart = Main.npc[(int)npc.localAI[i]];
                        if (bodyPart.active)
                            Projectile.NewProjectile(npc.GetSource_FromThis(), bodyPart.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, bodyPart.whoAmI, bodyPart.type);
                    }
                }

                if (AttackTimer > 400)
                {
                    AttackTimer = 0;
                    npc.netUpdate = true;
                    NetSync(npc);

                    switch (VulnerabilityState)
                    {
                        case 0: //melee
                            for (int i = 0; i < 3; i++)
                            {
                                NPC bodyPart = Main.npc[(int)npc.localAI[i]];

                                if (bodyPart.active)
                                {
                                    int damage = 30;
                                    for (int j = -2; j <= 2; j++)
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            Projectile.NewProjectile(npc.GetSource_FromThis(), bodyPart.Center,
                                                6f * bodyPart.DirectionFrom(Main.player[npc.target].Center).RotatedBy(Math.PI / 2 / 4 * j),
                                                ModContent.ProjectileType<MoonLordFireball>(), damage, 0f, Main.myPlayer, 20, 20 + 60);
                                        }
                                    }
                                }
                            }
                            break;

                        case 1: //ranged
                            for (int j = 0; j < 6; j++)
                            {
                                Vector2 spawn = Main.player[npc.target].Center + 500 * npc.DirectionFrom(Main.player[npc.target].Center).RotatedBy(MathHelper.TwoPi / 6 * (j + 0.5f));
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), spawn, Vector2.Zero, ModContent.ProjectileType<LightningVortexHostile>(), 30, 0f, Main.myPlayer, 1, Main.player[npc.target].DirectionFrom(spawn).ToRotation());
                            }
                            break;

                        case 2: //magic
                            for (int i = 0; i < 3; i++)
                            {
                                NPC bodyPart = Main.npc[(int)npc.localAI[i]];

                                if (bodyPart.active &&
                                    ((i == 2 && bodyPart.type == NPCID.MoonLordHead) ||
                                    bodyPart.type == NPCID.MoonLordHand))
                                {
                                    int damage = 35;
                                    const int max = 6;
                                    for (int j = 0; j < max; j++)
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            int p = Projectile.NewProjectile(npc.GetSource_FromThis(), bodyPart.Center,
                                              2.5f * bodyPart.DirectionFrom(Main.player[npc.target].Center).RotatedBy(Math.PI * 2 / max * (j + 0.5)),
                                              ModContent.ProjectileType<MoonLordNebulaBlaze>(), damage, 0f, Main.myPlayer);
                                            if (p != Main.maxProjectiles)
                                                Main.projectile[p].timeLeft = 1200;
                                        }
                                    }
                                }
                            }
                            break;

                        case 3: //summoner
                            for (int i = 0; i < 3; i++)
                            {
                                NPC bodyPart = Main.npc[(int)npc.localAI[i]];

                                if (bodyPart.active &&
                                    ((i == 2 && bodyPart.type == NPCID.MoonLordHead) ||
                                    bodyPart.type == NPCID.MoonLordHand))
                                {
                                    Vector2 speed = Main.player[npc.target].Center - bodyPart.Center;
                                    speed.Normalize();
                                    speed *= 5f;
                                    for (int j = -1; j <= 1; j++)
                                    {
                                        Vector2 vel = speed.RotatedBy(MathHelper.ToRadians(15) * j);
                                        FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), bodyPart.Center, NPCID.AncientLight, 0, 0f, (Main.rand.NextFloat() - 0.5f) * 0.3f * 6.28318548202515f / 60f, vel.X, vel.Y, velocity: vel);
                                    }
                                }
                            }
                            break;

                        default: //phantasmal eye rings
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                const int max = 4;
                                const int speed = 8;
                                const float rotationModifier = 0.5f;
                                int damage = 40;
                                float rotation = 2f * (float)Math.PI / max;
                                Vector2 vel = Vector2.UnitY * speed;
                                int type = ModContent.ProjectileType<Projectiles.MutantBoss.MutantSphereRing>();
                                for (int i = 0; i < max; i++)
                                {
                                    vel = vel.RotatedBy(rotation);
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel, type, damage, 0f, Main.myPlayer, rotationModifier, speed);
                                        if (p != Main.maxProjectiles)
                                            Main.projectile[p].timeLeft = 1800 - (int)VulnerabilityTimer;
                                        p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel, type, damage, 0f, Main.myPlayer, -rotationModifier, speed);
                                        if (p != Main.maxProjectiles)
                                            Main.projectile[p].timeLeft = 1800 - (int)VulnerabilityTimer;
                                    }
                                }
                                SoundEngine.PlaySound(SoundID.Item84, npc.Center);
                            }
                            break;
                    }
                }
            }
            else //only when vulnerable
            {
                if (!EnteredPhase2)
                {
                    EnteredPhase2 = true;
                    AttackTimer = 0;
                    SoundEngine.PlaySound(SoundID.Roar, Main.LocalPlayer.Center);
                    npc.netUpdate = true;
                    NetSync(npc);
                }

                Player player = Main.player[npc.target];
                switch (VulnerabilityState)
                {
                    case 0: //melee
                        {
                            if (AttackTimer > 30)
                            {
                                AttackTimer -= 300;
                                AttackMemory = AttackMemory == 0 ? 1 : 0;

                                float handToAttackWith = npc.localAI[AttackMemory];
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), Main.npc[(int)handToAttackWith].Center, Vector2.Zero, ModContent.ProjectileType<MoonLordSun>(), 60, 0f, Main.myPlayer, npc.whoAmI, handToAttackWith);
                            }
                        }
                        break;

                    case 1: //vortex
                        {
                            if (AttackMemory == 0) //spawn the vortex
                            {
                                AttackMemory = 1;
                                for (int i = -1; i <= 1; i += 2)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<MoonLordVortex>(), 40, 0f, Main.myPlayer, i, npc.whoAmI);
                                }
                            }
                        }
                        break;

                    case 2: //nebula
                        {
                            if (AttackTimer > 30)
                            {
                                AttackTimer -= 420;

                                for (int i = 0; i < 3; i++)
                                {
                                    NPC bodyPart = Main.npc[(int)npc.localAI[i]];
                                    int damage = 35;
                                    for (int j = -2; j <= 2; j++)
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            Projectile.NewProjectile(npc.GetSource_FromThis(), bodyPart.Center,
                                                2.5f * bodyPart.DirectionFrom(Main.player[npc.target].Center).RotatedBy(Math.PI / 2 / 2 * (j + Main.rand.NextFloat(-0.25f, 0.25f))),
                                                ModContent.ProjectileType<MoonLordNebulaBlaze2>(), damage, 0f, Main.myPlayer, npc.whoAmI);
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    case 3: //stardust
                        {
                            if (AttackTimer > 360)
                            {
                                AttackTimer -= 360;
                                AttackMemory = 0;
                            }

                            float baseRotation = MathHelper.ToRadians(50);
                            if (++AttackMemory == 10)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), Main.npc[(int)npc.localAI[0]].Center, Main.npc[(int)npc.localAI[0]].DirectionTo(player.Center), ModContent.ProjectileType<PhantasmalDeathrayMLSmall>(),
                                        60, 0f, Main.myPlayer, baseRotation * Main.rand.NextFloat(0.9f, 1.1f), npc.localAI[0]);
                            }
                            else if (AttackMemory == 20)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), Main.npc[(int)npc.localAI[1]].Center, Main.npc[(int)npc.localAI[2]].DirectionTo(player.Center), ModContent.ProjectileType<PhantasmalDeathrayMLSmall>(),
                                        60, 0f, Main.myPlayer, -baseRotation * Main.rand.NextFloat(0.9f, 1.1f), npc.localAI[1]);
                            }
                            else if (AttackMemory == 30)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), Main.npc[(int)npc.localAI[2]].Center, Main.npc[(int)npc.localAI[1]].DirectionTo(player.Center), ModContent.ProjectileType<PhantasmalDeathrayMLSmall>(),
                                        60, 0f, Main.myPlayer, baseRotation * Main.rand.NextFloat(0.9f, 1.1f), npc.localAI[2]);
                            }
                            else if (AttackMemory == 40)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, npc.DirectionTo(player.Center), ModContent.ProjectileType<PhantasmalDeathrayMLSmall>(),
                                        60, 0f, Main.myPlayer, -baseRotation * Main.rand.NextFloat(0.9f, 1.1f), npc.whoAmI);
                            }
                        }
                        break;

                    default: //any
                        {
                            if (AttackMemory == 0) //spawn the moons
                            {
                                AttackMemory = 1;

                                foreach (Projectile p in Main.projectile.Where(p => p.active && p.hostile))
                                {
                                    if (p.type == ModContent.ProjectileType<LunarRitual>() && p.ai[1] == npc.whoAmI) //find my arena
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            for (int i = 0; i < 4; i++)
                                            {
                                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, p.DirectionTo(Main.player[npc.target].Center).RotatedBy(MathHelper.TwoPi / 4 * i), ModContent.ProjectileType<MoonLordMoon>(),
                                                    60, 0f, Main.myPlayer, p.identity, 1450);
                                            }
                                            for (int i = 0; i < 4; i++)
                                            {
                                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, p.DirectionTo(Main.player[npc.target].Center).RotatedBy(MathHelper.TwoPi / 4 * (i + 0.5f)), ModContent.ProjectileType<MoonLordMoon>(),
                                                    60, 0f, Main.myPlayer, p.identity, -950);
                                            }
                                        }
                                        break;
                                    }
                                }
                            }

                            if (FargoSoulsWorld.MasochistModeReal && AttackTimer > 300)
                            {
                                AttackTimer -= 540;

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    const int max = 8;
                                    const int speed = 8;
                                    const float rotationModifier = 0.5f;
                                    int damage = 40;
                                    float rotation = 2f * (float)Math.PI / max;
                                    Vector2 vel = Vector2.UnitY * speed;
                                    int type = ModContent.ProjectileType<Projectiles.MutantBoss.MutantSphereRing>();
                                    for (int i = 0; i < max; i++)
                                    {
                                        vel = vel.RotatedBy(rotation);
                                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel, type, damage, 0f, Main.myPlayer, rotationModifier, speed);
                                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel, type, damage, 0f, Main.myPlayer, -rotationModifier, speed);
                                    }
                                    SoundEngine.PlaySound(SoundID.Item84, npc.Center);
                                }
                            }
                        }
                        break;
                }
            }

            if (npc.ai[0] == 2f) //moon lord is dead
            {
                VulnerabilityState = 4;
                VulnerabilityTimer = 0;
                AttackTimer = 0;
            }
            else //moon lord isn't dead
            {
                const float maxRampup = 3;
                float lerp = (float)npc.life / npc.lifeMax;
                if (FargoSoulsWorld.MasochistModeReal)
                    lerp *= lerp;
                float increment = (int)Math.Round(MathHelper.Lerp(maxRampup, 1, lerp));

                VulnerabilityTimer += increment;
                AttackTimer += increment;

                if (VulnerabilityTimer > 1800) //next vuln phase
                {
                    VulnerabilityState = ++VulnerabilityState % 5;

                    VulnerabilityTimer = 0;
                    AttackTimer = 0;
                    AttackMemory = 0;

                    npc.netUpdate = true;
                    NetSync(npc);

                    if (FargoSoulsWorld.MasochistModeReal)
                    {
                        switch (VulnerabilityState)
                        {
                            case 0: //melee
                                for (int i = 0; i < 3; i++)
                                {
                                    NPC bodyPart = Main.npc[(int)npc.localAI[i]];

                                    if (bodyPart.active && bodyPart.type == NPCID.MoonLordHead)
                                    {
                                        for (int j = -3; j <= 3; j++)
                                        {
                                            FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(),
                                                bodyPart.Center, NPCID.SolarGoop, target: npc.target,
                                                velocity: -10f * Vector2.UnitY.RotatedBy(MathHelper.ToRadians(20 * j)));
                                        }
                                    }
                                }
                                break;

                            case 1: //ranged
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(
                                          npc.GetSource_FromThis(),
                                          npc.Center, Vector2.Zero,
                                          ModContent.ProjectileType<MoonLordVortexOld>(),
                                          40, 0f, Main.myPlayer, 0, npc.whoAmI);
                                }
                                break;

                            case 2: //magic
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    //for (int i = -1; i <= 1; i++)
                                    //{
                                    int p = Projectile.NewProjectile(
                                      npc.GetSource_FromThis(),
                                      npc.Center, Vector2.Zero,
                                      ModContent.ProjectileType<GlowLine>(),
                                      0, 0f, Main.myPlayer, 17f, npc.whoAmI);
                                    if (p != Main.maxProjectiles)
                                    {
                                        //Main.projectile[p].localAI[0] = 950f * i;
                                        if (Main.netMode == NetmodeID.Server)
                                            NetMessage.SendData(MessageID.SyncProjectile, number: p);
                                    }
                                    //}
                                }
                                break;

                            case 3: //summon
                                for (int i = 0; i < 3; i++)
                                {
                                    NPC bodyPart = Main.npc[(int)npc.localAI[i]];

                                    if (bodyPart.active)
                                    {
                                        for (int j = -2; j <= 2; j++)
                                        {
                                            Vector2 vel = 9f * bodyPart.DirectionTo(Main.player[npc.target].Center).RotatedBy(MathHelper.Pi / 5 * j);
                                            FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(),
                                                bodyPart.Center, NPCID.AncientLight, 0,
                                                0f,
                                                (Main.rand.NextFloat() - 0.5f) * 0.3f * 6.28318548202515f / 60f,
                                                vel.X,
                                                vel.Y,
                                                npc.target,
                                                vel);
                                        }
                                    }
                                }
                                break;

                            default:
                                break;
                        }
                    }
                }
            }

            //because 1.4 is fucking stupid and time freeze prevents custom skies from working I HATE 1.4
            if (Main.GameModeInfo.IsJourneyMode && CreativePowerManager.Instance.GetPower<CreativePowers.FreezeTime>().Enabled)
                CreativePowerManager.Instance.GetPower<CreativePowers.FreezeTime>().SetPowerInfo(false);

            if (!Main.dedServ && ++SkyTimer > 30 && NPC.FindFirstNPC(npc.type) == npc.whoAmI)
            {
                SkyTimer = 0;

                if (!SkyManager.Instance["FargowiltasSouls:MoonLordSky"].IsActive())
                    SkyManager.Instance.Activate("FargowiltasSouls:MoonLordSky");

                void HandleScene(string name)
                {
                    if (!Filters.Scene[$"FargowiltasSouls:{name}"].IsActive())
                        Filters.Scene.Activate($"FargowiltasSouls:{name}");
                }

                switch (VulnerabilityState)
                {
                    case 0: HandleScene("Solar"); break;
                    case 1: HandleScene("Vortex"); break;
                    case 2: HandleScene("Nebula"); break;
                    case 3:
                        HandleScene("Stardust");
                        if (VulnerabilityTimer < 120) //so that player isn't punished for using weapons during prior phase
                            Main.LocalPlayer.GetModPlayer<EModePlayer>().MasomodeMinionNerfTimer = 0;
                        break;
                    default: break;
                }
            }

            EModeUtils.DropSummon(npc, "CelestialSigil2", NPC.downedMoonlord, ref DroppedSummon, NPC.downedAncientCultist);

            return result;
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            LeadingConditionRule emodeRule = new LeadingConditionRule(new EModeDropCondition());
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<GalacticGlobe>()));
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.LunarOre, 150));
            npcLoot.Add(emodeRule);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadBossHeadSprite(recolor, 8);
            for (int i = 13; i <= 26; i++)
            {
                if (i == 20) continue;
                LoadExtra(recolor, i);
            }
            LoadExtra(recolor, 29);
        }
    }

    public class MoonLordFreeEye : MoonLord
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.MoonLordFreeEye);

        public override int GetVulnerabilityState(NPC npc)
        {
            NPC core = FargoSoulsUtil.NPCExists(npc.ai[3], NPCID.MoonLordCore);
            return core == null ? -1 : core.GetGlobalNPC<MoonLordCore>().VulnerabilityState;
        }

        public int OnSpawnCounter;
        public int RitualProj;

        public bool SpawnSynchronized;
        public bool SlowMode;

        public float LastState;


        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(OnSpawnCounter);
            binaryWriter.Write7BitEncodedInt(RitualProj);
            bitWriter.WriteBit(SpawnSynchronized);
            bitWriter.WriteBit(SlowMode);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            OnSpawnCounter = binaryReader.Read7BitEncodedInt();
            RitualProj = binaryReader.Read7BitEncodedInt();
            SpawnSynchronized = bitReader.ReadBit();
            SlowMode = bitReader.ReadBit();
        }

        public override bool SafePreAI(NPC npc)
        {
            if (FargoSoulsWorld.SwarmActive)
                return true;

            NPC core = FargoSoulsUtil.NPCExists(npc.ai[3], NPCID.MoonLordCore);

            if (core == null)
                return true;

            if (!SpawnSynchronized && ++OnSpawnCounter > 2) //sync to other eyes of same core when spawned
            {
                SpawnSynchronized = true;
                OnSpawnCounter = 0;
                for (int i = 0; i < Main.maxProjectiles; i++) //find ritual
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<LunarRitual>()
                        && Main.projectile[i].ai[1] == npc.ai[3])
                    {
                        RitualProj = i;
                        break;
                    }
                }
                for (int i = 0; i < Main.maxNPCs; i++) //eye sync
                {
                    if (Main.npc[i].active && Main.npc[i].type == NPCID.MoonLordFreeEye && Main.npc[i].ai[3] == npc.ai[3] && i != npc.whoAmI)
                    {
                        npc.ai[0] = Main.npc[i].ai[0];
                        npc.ai[1] = Main.npc[i].ai[1];
                        npc.ai[2] = Main.npc[i].ai[2];
                        npc.ai[3] = Main.npc[i].ai[3];
                        npc.localAI[0] = Main.npc[i].localAI[0];
                        npc.localAI[1] = Main.npc[i].localAI[1];
                        npc.localAI[2] = Main.npc[i].localAI[2];
                        npc.localAI[3] = Main.npc[i].localAI[3];
                        break;
                    }
                }
                npc.netUpdate = true;
                NetSync(npc);
            }

            if (FargoSoulsWorld.MasochistModeReal && LastState != npc.ai[0])
            {
                LastState = npc.ai[0];

                for (int i = 0; i < Main.maxNPCs; i++) //gradually desync from each other
                {
                    if (Main.npc[i].active && Main.npc[i].type == NPCID.MoonLordFreeEye && Main.npc[i].ai[3] == npc.ai[3])
                    {
                        if (i == npc.whoAmI)
                            break;

                        npc.ai[1] += 1;
                    }
                }
            }

            if (core.dontTakeDamage && !FargoSoulsWorld.MasochistModeReal) //behave slower until p2 proper
            {
                SlowMode = !SlowMode;
                if (SlowMode)
                {
                    npc.position -= npc.velocity;
                    return false;
                }
            }

            Projectile ritual = FargoSoulsUtil.ProjectileExists(RitualProj, ModContent.ProjectileType<LunarRitual>());
            if (ritual != null && ritual.ai[1] == npc.ai[3])
            {
                int threshold = (int)ritual.localAI[0] - 150;
                if (GetVulnerabilityState(npc) == 4)
                    threshold = 800 - 150;
                if (npc.Distance(ritual.Center) > threshold) //stay within ritual range
                {
                    npc.Center = Vector2.Lerp(npc.Center, ritual.Center + npc.DirectionFrom(ritual.Center) * threshold, 0.05f);
                }
            }

            return true;
        }
    }

    public class MoonLordBodyPart : MoonLord
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(NPCID.MoonLordHead, NPCID.MoonLordHand, NPCID.MoonLordLeechBlob);

        public override int GetVulnerabilityState(NPC npc)
        {
            NPC core = FargoSoulsUtil.NPCExists(npc.ai[3], NPCID.MoonLordCore);
            return core == null ? -1 : core.GetGlobalNPC<MoonLordCore>().VulnerabilityState;
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            //if (npc.type == NPCID.MoonLordHead) npc.lifeMax /= 2;
        }
    }
}
