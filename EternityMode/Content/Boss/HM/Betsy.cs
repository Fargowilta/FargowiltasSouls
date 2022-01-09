using Fargowiltas.Items.Summons.Abom;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.Net.Strategies;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Boss.HM
{
    public class Betsy : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.DD2Betsy);

        public int FuryRingTimer;
        public int FuryRingShotRotationCounter;

        public bool DoFuryRingAttack;
        public bool InFuryRingAttackCooldown;
        public bool InPhase2;

        public bool DroppedSummon;

        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(FuryRingTimer), IntStrategies.CompoundStrategy },
                { new Ref<object>(FuryRingShotRotationCounter), IntStrategies.CompoundStrategy },

                { new Ref<object>(DoFuryRingAttack), BoolStrategies.CompoundStrategy },
                { new Ref<object>(InFuryRingAttackCooldown), BoolStrategies.CompoundStrategy },
                { new Ref<object>(InPhase2), BoolStrategies.CompoundStrategy },
            };

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.boss = true;
            npc.lifeMax = (int)(npc.lifeMax * 4.0 / 3.0);
        }

        public override bool PreAI(NPC npc)
        {
            EModeGlobalNPC.betsyBoss = npc.whoAmI;

            if (FargoSoulsWorld.SwarmActive)
                return true;

            if (FargoSoulsWorld.MasochistModeReal)
            {
                for (int i = 0; i < 3; i++)
                {
                    Rectangle rectangle = new Rectangle((int)Main.screenPosition.X + Main.screenWidth / 3, (int)Main.screenPosition.Y + Main.screenHeight / 3, Main.screenWidth / 3, Main.screenHeight / 3);
                    CombatText.NewText(rectangle, new Color(100 + Main.rand.Next(150), 100 + Main.rand.Next(150), 100 + Main.rand.Next(150)), Main.rand.Next(new List<string> {
                    Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy1"),
                    Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy2"),
                    Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy3"),
                    Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy4"),
                    Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy5"),
                    Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy6"),
                    Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy7"),
                    Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy8"),
                    Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy9"),
                    Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy10"),
                    Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy11"),
                    Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy12"),
                    Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy13"),
                    Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy14"),
                    Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy15"),
                    Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy16"),
                    Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy17"),
                    Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy18"),
                    Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy19"),
                    Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy20"),
                    Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy21"),
                    Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy22"),
                    Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy23"),
                    Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy24"),
                    Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy25"),
                    Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy26") + $"{Main.rand.Next(10)}{Main.rand.Next(10)}{Main.rand.Next(10)}{Main.rand.Next(10)}{Main.rand.Next(10)}" + Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy27"),
                    Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy26") + $"{Main.rand.Next(10)}{Main.rand.Next(10)}{Main.rand.Next(10)}{Main.rand.Next(10)}{Main.rand.Next(10)}" + Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Betsy28"),
                    }), Main.rand.NextBool(), Main.rand.NextBool());
                }

                if (Main.rand.NextBool(30) && npc.HasPlayerTarget)
                {
                    switch (Main.rand.Next(12))
                    {
                        case 0:
                            if (!Main.dedServ)
                                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Thunder").WithVolume(1f).WithPitchVariance(-0.5f), Main.player[npc.target].Center);
                            break;

                        case 1:
                            Main.PlaySound(SoundID.Roar, Main.player[npc.target].Center, 2); //arte scream
                            break;

                        case 2:
                            Main.PlaySound(SoundID.Roar, Main.player[npc.target].Center, 0);
                            break;

                        case 3:
                            Main.PlaySound(SoundID.ForceRoar, Main.player[npc.target].Center, -1); //eoc roar
                            break;

                        case 4:
                            if (!Main.dedServ)
                                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Monster94"), Main.player[npc.target].Center);
                            break;

                        case 5:
                            if (!Main.dedServ)
                                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Monster5").WithVolume(1.5f), Main.player[npc.target].Center);
                            break;

                        case 6:
                            if (!Main.dedServ)
                                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Thunder").WithVolume(1.5f).WithPitchVariance(1.5f), Main.player[npc.target].Center);
                            break;

                        case 7:
                            if (!Main.dedServ)
                                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Zombie_104"), Main.player[npc.target].Center);
                            break;

                        case 8:
                            if (!Main.dedServ)
                                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Monster70"), Main.player[npc.target].Center);
                            break;

                        case 9:
                            if (!Main.dedServ)
                                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Railgun"), Main.player[npc.target].Center);
                            break;

                        case 10:
                            if (!Main.dedServ)
                                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Navi"), Main.player[npc.target].Center);
                            break;

                        case 11:
                            if (!Main.dedServ)
                                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/ZaWarudo").WithVolume(1.5f), Main.player[npc.target].Center);
                            break;

                        default:
                            Main.PlaySound(SoundID.NPCDeath10, Main.player[npc.target].Center);
                            break;
                    }
                }
            }

            if (!InPhase2 && npc.life < npc.lifeMax / 2)
            {
                InPhase2 = true;
                Main.PlaySound(SoundID.Roar, npc.Center, 0);
            }

            if (npc.ai[0] == 6f) //when approaching for roar
            {
                if (npc.ai[1] == 0f)
                {
                    npc.position += npc.velocity;
                }
                else if (npc.ai[1] == 1f)
                {
                    DoFuryRingAttack = true;
                }
            }

            if (DoFuryRingAttack)
            {
                npc.velocity = Vector2.Zero;

                if (FuryRingTimer == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), npc.damage / 3, 0f, Main.myPlayer, 4);
                }

                FuryRingTimer++;
                if (FuryRingTimer % 2 == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(npc.Center, -Vector2.UnitY.RotatedBy(2 * Math.PI / 30 * FuryRingShotRotationCounter), ModContent.ProjectileType<BetsyFury>(), npc.damage / 3, 0f, Main.myPlayer, npc.target);
                        Projectile.NewProjectile(npc.Center, -Vector2.UnitY.RotatedBy(2 * Math.PI / 30 * -FuryRingShotRotationCounter), ModContent.ProjectileType<BetsyFury>(), npc.damage / 3, 0f, Main.myPlayer, npc.target);
                    }
                    FuryRingShotRotationCounter++;
                }
                if (FuryRingTimer > (InPhase2 ? 90 : 30) + 2)
                {
                    DoFuryRingAttack = false;
                    InFuryRingAttackCooldown = true;
                    FuryRingTimer = 0;
                    FuryRingShotRotationCounter = 0;
                }

                EModeGlobalNPC.Aura(npc, 1200, BuffID.WitheredWeapon, true, 226);
                EModeGlobalNPC.Aura(npc, 1200, BuffID.WitheredArmor, true, 226);
            }

            if (InFuryRingAttackCooldown)
            {
                EModeGlobalNPC.Aura(npc, 1200, BuffID.WitheredWeapon, true, 226);
                EModeGlobalNPC.Aura(npc, 1200, BuffID.WitheredArmor, true, 226);

                if (++FuryRingShotRotationCounter > 90)
                {
                    InFuryRingAttackCooldown = false;
                    FuryRingTimer = 0;
                    FuryRingShotRotationCounter = 0;
                }
                npc.position -= npc.velocity * 0.5f;
                if (FuryRingTimer % 2 == 0)
                    return false;
            }

            if (!DD2Event.Ongoing && npc.HasPlayerTarget && (!Main.player[npc.target].active || Main.player[npc.target].dead || npc.Distance(Main.player[npc.target].Center) > 3000))
            {
                int p = Player.FindClosest(npc.Center, 0, 0); //extra despawn code for when summoned outside event
                if (p < 0 || !Main.player[p].active || Main.player[p].dead || npc.Distance(Main.player[p].Center) > 3000)
                    npc.active = false;
            }

            EModeUtils.DropSummon(npc, ModContent.ItemType<BetsyEgg>(), FargoSoulsWorld.downedBetsy, ref DroppedSummon, NPC.downedGolemBoss);

            return true;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.WitheredArmor, 600);
            target.AddBuff(BuffID.WitheredWeapon, 600);
            target.AddBuff(ModContent.BuffType<MutantNibble>(), 600);
        }

        public override bool PreNPCLoot(NPC npc)
        {
            npc.boss = false;

            return base.PreNPCLoot(npc);
        }

        public override void NPCLoot(NPC npc)
        {
            base.NPCLoot(npc);

            npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<BetsysHeart>());
            npc.DropItemInstanced(npc.position, npc.Size, ItemID.GoldenCrate, 5);
            FargoSoulsWorld.downedBetsy = true;
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
            LoadBossHeadSprite(recolor, 34);
            LoadGoreRange(recolor, 1079, 1086);
            LoadExtra(recolor, 81);
            LoadExtra(recolor, 82);
        }
    }
}
