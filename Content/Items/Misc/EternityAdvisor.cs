using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Essences;
using FargowiltasSouls.Content.Items.Accessories.Expert;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Items.Accessories.Masomode;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Items.Armor;
using FargowiltasSouls.Content.Items.Consumables;
using FargowiltasSouls.Content.Items.Summons;
using FargowiltasSouls.Core.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Misc
{
    public class EternityAdvisor : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Eternity Advisor");
            // Tooltip.SetDefault("Suggests loadouts for Eternity Mode based on progression");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.maxStack = 1;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.value = Item.buyPrice(0, 1);
        }

        public override bool CanUseItem(Player player) => WorldSavingSystem.EternityMode;

        private static string GetBuildText(params int[] args)
        {
            string text = "";
            foreach (int itemType in args)
            {
                if (itemType != -1)
                {
                    text += $"[i:{itemType}]";
                }

            }
            return text;
        }

        private static string GetBuildTextRandom(params int[] args) //takes number of accs to use as first param and list of accs as the rest
        {
            List<int> choices = new();
            int maxSize = args.Length - 1;
            for (int i = 0; i < args[0]; i++)
            {
                int attempt = Main.rand.Next(maxSize) + 1; //skip the first number
                if (choices.Contains(args[attempt]) || args[attempt] == -1) //if already chose this acc or is -1, try to choose the next in line
                {
                    for (int j = 0; j < maxSize; j++)
                    {
                        if (++attempt >= maxSize) //wrap around at end of array
                            attempt = 1;
                        if (!choices.Contains(args[attempt]))
                            break;
                    }
                }
                choices.Add(args[attempt]);
            }
            return GetBuildText(choices.ToArray());
        }

        private int GetBossHelp(out string build, Player player)
        {
            build = "";
            int summonType = -1;
            string other = string.Empty;
            int[] meleeSpecific = null, rangerSpecific = null, mageSpecific = null, summonerSpecific = null;


            if (!WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.TrojanSquirrel])
            {
                summonType = ModContent.ItemType<SquirrelCoatofArms>();
                build += GetBuildText(
                    ModContent.ItemType<EurusSock>(),
                    ModContent.ItemType<PuffInABottle>(),
                    ModContent.ItemType<BorealWoodEnchant>(),
                    //ModContent.ItemType<PumpkinEnchant>(),
                    //ModContent.ItemType<PalmWoodEnchant>(),
                    ModContent.ItemType<CactusEnchant>()
                );
            }
            else if (!NPC.downedSlimeKing)
            {
                summonType = ItemID.SlimeCrown;
                build += GetBuildText(
                    ModContent.ItemType<EurusSock>(),
                    ModContent.ItemType<PuffInABottle>()
                ) + GetBuildTextRandom(
                    3,
                    ModContent.ItemType<EbonwoodEnchant>(),
                    ModContent.ItemType<BorealWoodEnchant>(),
                    ModContent.ItemType<PumpkinEnchant>(),
                    ModContent.ItemType<ShadewoodEnchant>(),
                    ModContent.ItemType<TungstenEnchant>(),
                    ModContent.ItemType<CactusEnchant>()
                );
            }
            else if (!NPC.downedBoss1)
            {
                summonType = ItemID.SuspiciousLookingEye;
                build += GetBuildText(
                    Main.rand.Next(new int[] { ItemID.HermesBoots, ItemID.SailfishBoots, ItemID.FlurryBoots, ItemID.RocketBoots, ItemID.SpectreBoots }),
                    Main.rand.Next(new int[] { ItemID.CloudinaBalloon, ItemID.SharkronBalloon, ItemID.SandstorminaBalloon, ItemID.BlizzardinaBalloon })
                ) + GetBuildTextRandom(
                    3,
                    ItemID.CharmofMyths,
                    ModContent.ItemType<NinjaEnchant>(),
                    ModContent.ItemType<LeadEnchant>(),
                    ModContent.ItemType<BorealWoodEnchant>(),
                    ModContent.ItemType<ShadewoodEnchant>(),
                    ModContent.ItemType<CactusEnchant>(),
                    ModContent.ItemType<PalmWoodEnchant>(),
                    ModContent.ItemType<TungstenEnchant>()
                );
            }
            else if (!NPC.downedBoss2)
            {
                summonType = WorldGen.crimson ? ItemID.BloodySpine : ItemID.WormFood;
                build += GetBuildText(
                    Main.rand.Next(new int[] { ItemID.SpectreBoots, ItemID.LightningBoots, ItemID.FrostsparkBoots }),
                    Main.rand.Next(new int[] { ItemID.BalloonHorseshoeFart, ItemID.BalloonHorseshoeSharkron, ItemID.WhiteHorseshoeBalloon }),
                    Main.rand.Next(new int[] { ItemID.EoCShield, ModContent.ItemType<JungleEnchant>() })
                ) + GetBuildTextRandom(
                    2,
                    ModContent.ItemType<LeadEnchant>(),
                    ModContent.ItemType<EbonwoodEnchant>(),
                    ModContent.ItemType<CactusEnchant>(),
                    ModContent.ItemType<TungstenEnchant>(),
                    ModContent.ItemType<CopperEnchant>()
                );
            }
            else if (!NPC.downedQueenBee)
            {
                summonType = ItemID.Abeemination;
                build += GetBuildText(
                    Main.rand.NextBool() ? ItemID.FrostsparkBoots : ItemID.LightningBoots,
                    Main.rand.Next(new int[] { ItemID.EoCShield, ModContent.ItemType<JungleEnchant>(), ModContent.ItemType<MeteorEnchant>() }),
                    ItemID.Bezoar,
                    Main.rand.Next(new int[] { ItemID.BalloonHorseshoeFart, ItemID.BalloonHorseshoeSharkron, ItemID.WhiteHorseshoeBalloon }),
                    Main.rand.Next(new int[] {
                                ModContent.ItemType<RainEnchant>(),
                                ModContent.ItemType<TungstenEnchant>(),
                                ModContent.ItemType<ShadowEnchant>(),
                                ModContent.ItemType<ShadewoodEnchant>(),
                                ModContent.ItemType<NinjaEnchant>()
                    })
                );
                other += $"[i:{ModContent.Find<ModItem>("Fargowiltas", "CityBuster").Type}]";
            }
            else if (!NPC.downedBoss3)
            {
                summonType = ModContent.TryFind("Fargowiltas", "SuspiciousSkull", out ModItem modItem) ? modItem.Type : ItemID.SkeletronMask;
                build += GetBuildText(
                    Main.rand.Next(new int[] { ItemID.FrostsparkBoots, ItemID.TerrasparkBoots }),
                    Main.rand.Next(new int[] { ItemID.EoCShield, ModContent.ItemType<JungleEnchant>(), ModContent.ItemType<QueenStinger>(), ModContent.ItemType<MeteorEnchant>() }),
                    Main.rand.Next(new int[] { ItemID.BundleofBalloons, ItemID.HorseshoeBundle, ModContent.ItemType<BeeEnchant>() })
                    ) + GetBuildTextRandom(
                    2,
                    ModContent.ItemType<SkullCharm>(),
                    ModContent.ItemType<ShadowEnchant>(),
                    ModContent.ItemType<TinEnchant>(),
                    ModContent.ItemType<TungstenEnchant>(),
                    ModContent.ItemType<NinjaEnchant>(),
                    ModContent.ItemType<CrimsonEnchant>()
                );
            }
            else if (!NPC.downedDeerclops)
            {
                summonType = ModContent.TryFind("Fargowiltas", "DeerThing2", out ModItem modItem) ? modItem.Type : ItemID.DeerThing;
                build += GetBuildText(
                    Main.rand.Next(new int[] { ItemID.FrostsparkBoots, ItemID.TerrasparkBoots}),
                    Main.rand.Next(new int[] { ItemID.EoCShield, ModContent.ItemType<JungleEnchant>(), ModContent.ItemType<QueenStinger>(), ModContent.ItemType<MeteorEnchant>() }),
                    Main.rand.Next(new int[] { ItemID.BundleofBalloons, ItemID.HorseshoeBundle, ModContent.ItemType<BeeEnchant>() })
                    ) + GetBuildTextRandom(
                    2,
                    ItemID.HandWarmer,
                    ModContent.ItemType<EbonwoodEnchant>(),
                    ModContent.ItemType<ShadowEnchant>(),
                    ModContent.ItemType<TungstenEnchant>(),
                    ModContent.ItemType<TinEnchant>(),
                    ModContent.ItemType<NinjaEnchant>()
                );
            }
            else if (!WorldSavingSystem.DownedDevi)
            {
                summonType = ModContent.ItemType<DevisCurse>();
                build += GetBuildText(
                    Main.rand.Next(new int[] { ItemID.FrostsparkBoots, ItemID.TerrasparkBoots}),
                    Main.rand.Next(new int[] { ItemID.EoCShield, ModContent.ItemType<JungleEnchant>(), ModContent.ItemType<QueenStinger>(), ModContent.ItemType<MeteorEnchant>() }),
                    Main.rand.Next(new int[] { ItemID.BundleofBalloons, ItemID.HorseshoeBundle, ModContent.ItemType<BeeEnchant>() }),
                    Main.rand.Next(new int[] { ModContent.ItemType<NymphsPerfume>(), ModContent.ItemType<ShadowEnchant>() }),
                    Main.rand.Next(new int[] { ModContent.ItemType<TinEnchant>(), ModContent.ItemType<NinjaEnchant>(), ModContent.ItemType<TungstenEnchant>() })
                    );
            }
            else if (!Main.hardMode)
            {
                summonType = ModContent.TryFind("Fargowiltas", "FleshyDoll", out ModItem modItem) ? modItem.Type : ItemID.GuideVoodooDoll;
                build += GetBuildText(
                    ModContent.ItemType<ZephyrBoots>(),
                    Main.rand.Next(new int[] { ModContent.ItemType<SupremeDeathbringerFairy>(), ModContent.ItemType<SparklingAdoration>() })
                ) + GetBuildTextRandom(3,
                    ModContent.ItemType<TinEnchant>(),
                    ModContent.ItemType<SkullCharm>(),
                    ModContent.ItemType<CopperEnchant>(),
                    ModContent.ItemType<NinjaEnchant>());
                other += $"[i:{ModContent.Find<ModItem>("Fargowiltas", "DoubleObsidianInstabridge").Type}]";
                meleeSpecific = new int[] { ModContent.ItemType<TungstenEnchant>() };
            }
            else if (!NPC.downedQueenSlime)
            {
                summonType = ModContent.TryFind("Fargowiltas", "JellyCrystal", out ModItem modItem) ? modItem.Type : ItemID.QueenSlimeCrystal;
                build += GetBuildText(
                    Main.rand.Next(new int[] { ModContent.ItemType<ZephyrBoots>(), ModContent.ItemType<MeteorEnchant>() }),
                    Main.rand.Next(new int[] { ItemID.FrozenWings, ItemID.AngelWings, ModContent.ItemType<BeeEnchant>() }),
                    Main.rand.Next(new int[] { ModContent.ItemType<SupremeDeathbringerFairy>(), ModContent.ItemType<SparklingAdoration>() })
                    ) + GetBuildTextRandom(
                    3,
                    ModContent.ItemType<MythrilEnchant>(),
                    ModContent.ItemType<OrichalcumEnchant>(),
                    ModContent.ItemType<AdamantiteEnchant>(),
                    ModContent.ItemType<PalladiumEnchant>()
                );
                other += GetBuildText(ModContent.ItemType<WizardEnchant>());

                meleeSpecific = new int[] { ItemID.WarriorEmblem, ModContent.ItemType<TungstenEnchant>() };
                rangerSpecific = new int[] { ItemID.RangerEmblem };
                mageSpecific = new int[] { ItemID.SorcererEmblem };
                summonerSpecific = new int[] { ItemID.SummonerEmblem, ItemID.PygmyNecklace };
            }
            else if (!WorldSavingSystem.downedBoss[(int)WorldSavingSystem.Downed.BanishedBaron])
            {
                summonType = ModContent.ItemType<MechLure>();
                build += GetBuildText(
                    ModContent.ItemType<ZephyrBoots>(),
                    Main.rand.Next(new int[] { ItemID.FrozenWings, ItemID.AngelWings, ModContent.ItemType<GelicWings>(), ModContent.ItemType<BeeEnchant>() }),
                    Main.rand.Next(new int[] { ModContent.ItemType<CrystalAssassinEnchant>(), ModContent.ItemType<MeteorEnchant>() })
                    ) + GetBuildTextRandom(
                    3,
                    ModContent.ItemType<SupremeDeathbringerFairy>(),
                    ModContent.ItemType<SparklingAdoration>(),
                    ModContent.ItemType<OrichalcumEnchant>(),
                    ModContent.ItemType<MythrilEnchant>(),
                    ModContent.ItemType<PalladiumEnchant>(),
                    ModContent.ItemType<PearlwoodEnchant>(),
                    ModContent.ItemType<FrostEnchant>()
                );
                other += GetBuildText(ModContent.ItemType<WizardEnchant>());

                meleeSpecific = new int[] { ItemID.WarriorEmblem, ModContent.ItemType<TungstenEnchant>() };
                rangerSpecific = new int[] { ItemID.RangerEmblem };
                mageSpecific = new int[] { ItemID.SorcererEmblem };
                summonerSpecific = new int[] { ItemID.SummonerEmblem, ItemID.PygmyNecklace };
            }
            else if (!NPC.downedMechBoss1)
            {
                summonType = ItemID.MechanicalWorm;
                build += GetBuildText(
                    Main.rand.Next(new int[] { ModContent.ItemType<ZephyrBoots>(), ModContent.ItemType<MeteorEnchant>() }),
                    Main.rand.Next(new int[] { ItemID.FrozenWings, ModContent.ItemType<GelicWings>() })
                ) + GetBuildTextRandom(
                    4,
                    ModContent.ItemType<SupremeDeathbringerFairy>(),
                    ModContent.ItemType<OrichalcumEnchant>(),
                    ModContent.ItemType<MythrilEnchant>(),
                    ModContent.ItemType<PalladiumEnchant>(),
                    ModContent.ItemType<PearlwoodEnchant>(),
                    ModContent.ItemType<FrostEnchant>()
                );
                other += GetBuildText(ModContent.ItemType<WizardEnchant>());
                meleeSpecific = new int[] { ItemID.WarriorEmblem, ModContent.ItemType<TungstenEnchant>() };
                rangerSpecific = new int[] { ItemID.RangerEmblem };
                mageSpecific = new int[] { ItemID.SorcererEmblem };
                summonerSpecific = new int[] { ItemID.SummonerEmblem, ItemID.PygmyNecklace };
            }
            else if (!NPC.downedMechBoss2)
            {
                summonType = ItemID.MechanicalEye;
                build += GetBuildText(
                    Main.rand.Next(new int[] { ModContent.ItemType<ZephyrBoots>(), ModContent.ItemType<MeteorEnchant>() }),
                    Main.rand.Next(new int[] { ItemID.FlameWings, ItemID.FrozenWings, ModContent.ItemType<GelicWings>() })
                ) + GetBuildTextRandom(
                    4,
                    ModContent.ItemType<SupremeDeathbringerFairy>(),
                    ModContent.ItemType<OrichalcumEnchant>(),
                    ModContent.ItemType<TungstenEnchant>(),
                    ModContent.ItemType<MythrilEnchant>(),
                    ModContent.ItemType<PalladiumEnchant>(),
                    ModContent.ItemType<PearlwoodEnchant>(),
                    ModContent.ItemType<FrostEnchant>()
                );
                other += GetBuildText(ModContent.ItemType<WizardEnchant>());
                meleeSpecific = new int[] { ModContent.ItemType<BarbariansEssence>(), ModContent.ItemType<TungstenEnchant>() };
                rangerSpecific = new int[] { ModContent.ItemType<SharpshootersEssence>() };
                mageSpecific = new int[] { ModContent.ItemType<ApprenticesEssence>() };
                summonerSpecific = new int[] { ModContent.ItemType<OccultistsEssence>(), ModContent.ItemType<AncientHallowEnchant>() };
            }
            else if (!NPC.downedMechBoss3)
            {
                summonType = ItemID.MechanicalSkull;
                build += GetBuildText(
                    Main.rand.Next(new int[] { ModContent.ItemType<ZephyrBoots>(), ModContent.ItemType<MeteorEnchant>() }),
                    Main.rand.Next(new int[] { ItemID.FlameWings, ItemID.FrozenWings, ModContent.ItemType<GelicWings>() })
                ) + GetBuildTextRandom(
                    4,
                    ModContent.ItemType<SupremeDeathbringerFairy>(),
                    ModContent.ItemType<OrichalcumEnchant>(),
                    ModContent.ItemType<TungstenEnchant>(),
                    ModContent.ItemType<MythrilEnchant>(),
                    ModContent.ItemType<PalladiumEnchant>(),
                    ModContent.ItemType<PearlwoodEnchant>(),
                    ModContent.ItemType<FrostEnchant>()
                );
                other += GetBuildText(ModContent.ItemType<WizardEnchant>());
                meleeSpecific = new int[] { ModContent.ItemType<BarbariansEssence>(), ModContent.ItemType<TungstenEnchant>() };
                rangerSpecific = new int[] { ModContent.ItemType<SharpshootersEssence>() };
                mageSpecific = new int[] { ModContent.ItemType<ApprenticesEssence>() };
                summonerSpecific = new int[] { ModContent.ItemType<OccultistsEssence>(), ModContent.ItemType<AncientHallowEnchant>() };
            }
            else if (!WorldSavingSystem.downedBoss[(int)WorldSavingSystem.Downed.Lifelight])
            {
                summonType = ModContent.ItemType<FragilePixieLamp>();
                build += GetBuildText(
                    Main.rand.Next(new int[] { ModContent.ItemType<AeolusBoots>(), ModContent.ItemType<MeteorEnchant>() }),
                    Main.rand.Next(new int[] { ItemID.FlameWings, ItemID.FrozenWings, ItemID.BeeWings, ModContent.ItemType<GelicWings>() })
                ) + GetBuildTextRandom(
                    4,
                    ModContent.ItemType<ChlorophyteEnchant>(),
                    ModContent.ItemType<SquireEnchant>(),
                    ModContent.ItemType<SupremeDeathbringerFairy>(),
                    ModContent.ItemType<OrichalcumEnchant>(),
                    ModContent.ItemType<HallowEnchant>(),
                    ModContent.ItemType<MythrilEnchant>(),
                    ModContent.ItemType<DubiousCircuitry>()
                );
                other += GetBuildText(ModContent.ItemType<WizardEnchant>());
                meleeSpecific = new int[] { ModContent.ItemType<BarbariansEssence>(), ModContent.ItemType<TungstenEnchant>() };
                rangerSpecific = new int[] { ModContent.ItemType<SharpshootersEssence>() };
                mageSpecific = new int[] { ModContent.ItemType<ApprenticesEssence>() };
                summonerSpecific = new int[] { ModContent.ItemType<OccultistsEssence>(), ModContent.ItemType<AncientHallowEnchant>() };
            }
            else if (!NPC.downedPlantBoss)
            {
                summonType = ModContent.TryFind("Fargowiltas", "PlanterasFruit", out ModItem modItem) ? modItem.Type : ItemID.PlanteraMask;
                build += GetBuildText(
                    Main.rand.Next(new int[] { ModContent.ItemType<AeolusBoots>(), ModContent.ItemType<MeteorEnchant>() }),
                    Main.rand.Next(new int[] { ItemID.FlameWings, ItemID.FrozenWings, ItemID.BeeWings, ModContent.ItemType<GelicWings>() })
                ) + GetBuildTextRandom(
                    3,
                    ModContent.ItemType<ForbiddenEnchant>(),
                    ModContent.ItemType<ChlorophyteEnchant>(),
                    ModContent.ItemType<DubiousCircuitry>(),
                    ModContent.ItemType<AncientShadowEnchant>(),
                    ModContent.ItemType<OrichalcumEnchant>(),
                    ModContent.ItemType<ApprenticeEnchant>(),
                    ModContent.ItemType<HallowEnchant>(),
                    ModContent.ItemType<TitaniumEnchant>()
                );
                other += GetBuildText(ModContent.ItemType<WizardEnchant>());
                other += $"[i:{ModContent.Find<ModItem>("Fargowiltas", "CityBuster").Type}]";
                meleeSpecific = new int[] { ModContent.ItemType<TungstenEnchant>() };
                summonerSpecific = new int[] { ModContent.ItemType<AncientHallowEnchant>() };
            }
            else if (!NPC.downedGolemBoss)
            {
                summonType = ItemID.LihzahrdPowerCell;
                build += GetBuildText(
                    ModContent.ItemType<AeolusBoots>(),
                    Main.rand.Next(new int[] { ItemID.SpookyWings })
                ) + GetBuildTextRandom(
                    3,
                    Main.rand.Next(new int[] { ItemID.MasterNinjaGear, ModContent.ItemType<MonkEnchant>(), ModContent.ItemType<ChlorophyteEnchant>(), ModContent.ItemType<MeteorEnchant>() }),
                    ModContent.ItemType<DubiousCircuitry>(),
                    ModContent.ItemType<CrimsonEnchant>(),
                    ModContent.ItemType<HallowEnchant>(),
                    ModContent.ItemType<AncientHallowEnchant>(),
                    ModContent.ItemType<ForbiddenEnchant>(),
                    ModContent.ItemType<LumpOfFlesh>()
                );
                other += GetBuildText(ModContent.ItemType<WizardEnchant>());
                other += $"[i:{ModContent.Find<ModItem>("Fargowiltas", "LihzahrdInstactuationBomb").Type}]";
                meleeSpecific = new int[] { ModContent.ItemType<BarbariansEssence>(), ModContent.ItemType<TungstenEnchant>() };
                rangerSpecific = new int[] { ModContent.ItemType<SharpshootersEssence>() };
                mageSpecific = new int[] { ModContent.ItemType<ApprenticesEssence>() };
                summonerSpecific = new int[] { ModContent.ItemType<OccultistsEssence>(), ModContent.ItemType<SpookyEnchant>(), ModContent.ItemType<TikiEnchant>() };
            }
            else if (!WorldSavingSystem.DownedBetsy)
            {
                summonType = ModContent.TryFind("Fargowiltas", "BetsyEgg", out ModItem modItem) ? modItem.Type : ItemID.BossMaskBetsy;
                build += GetBuildText(
                    ModContent.ItemType<AeolusBoots>(),
                    ItemID.BeetleWings,
                    ModContent.ItemType<LihzahrdTreasureBox>()
                ) + GetBuildTextRandom(
                    3,
                    Main.rand.Next(new int[] { ItemID.MasterNinjaGear, ModContent.ItemType<MonkEnchant>(), ModContent.ItemType<ChlorophyteEnchant>(), ModContent.ItemType<MeteorEnchant>() }),
                    ModContent.ItemType<DubiousCircuitry>(),
                    ModContent.ItemType<LumpOfFlesh>(),
                    ModContent.ItemType<CrimsonEnchant>(),
                    ModContent.ItemType<HallowEnchant>()
                );
                other += GetBuildText(ModContent.ItemType<WizardEnchant>());
                meleeSpecific = new int[] { ModContent.ItemType<BeetleEnchant>(), ModContent.ItemType<TungstenEnchant>() };
                summonerSpecific = new int[] { ModContent.ItemType<SpookyEnchant>(), ModContent.ItemType<TikiEnchant>() };
            }
            else if (!NPC.downedFishron)
            {
                summonType = ModContent.TryFind("Fargowiltas", "TruffleWorm2", out ModItem modItem) ? modItem.Type : ItemID.TruffleWorm;
                build += GetBuildText(
                    Main.rand.NextBool() ? ModContent.ItemType<AeolusBoots>() : ModContent.ItemType<ValhallaKnightEnchant>(),
                    ItemID.BetsyWings,
                    Main.rand.Next(new int[] { ModContent.ItemType<SupremeDeathbringerFairy>(), ModContent.ItemType<LihzahrdTreasureBox>(), ModContent.ItemType<BetsysHeart>(), ModContent.ItemType<MeteorEnchant>() })
                ) + GetBuildTextRandom(
                    3,
                    ModContent.ItemType<ForbiddenEnchant>(),
                    ModContent.ItemType<DarkArtistEnchant>(),
                    ModContent.ItemType<LumpOfFlesh>(),
                    ModContent.ItemType<PumpkingsCape>()
                );
                other += GetBuildText(ModContent.ItemType<WizardEnchant>());
                other += GetBuildText(ModContent.ItemType<RabiesVaccine>());
                meleeSpecific = new int[] { ModContent.ItemType<BeetleEnchant>(), ModContent.ItemType<TungstenEnchant>() };
                summonerSpecific = new int[] { ModContent.ItemType<SpookyEnchant>(), ModContent.ItemType<TikiEnchant>() };
            }
            else if (!NPC.downedEmpressOfLight)
            {
                summonType = ModContent.TryFind("Fargowiltas", "PrismaticPrimrose", out ModItem modItem) ? modItem.Type : ItemID.EmpressButterfly;
                build += GetBuildText(
                    Main.rand.NextBool() ? ModContent.ItemType<AeolusBoots>() : ModContent.ItemType<ValhallaKnightEnchant>(),
                    Main.rand.Next(new int[] { ItemID.BetsyWings, ItemID.FishronWings }),
                    Main.rand.Next(new int[] { ModContent.ItemType<SupremeDeathbringerFairy>(), ModContent.ItemType<LihzahrdTreasureBox>(), ModContent.ItemType<BetsysHeart>(), ModContent.ItemType<MeteorEnchant>() })
                ) + GetBuildTextRandom(
                    3,
                    ModContent.ItemType<ForbiddenEnchant>(),
                    ModContent.ItemType<DubiousCircuitry>(),
                    ModContent.ItemType<DarkArtistEnchant>(),
                    ModContent.ItemType<SpectreEnchant>(),
                    ModContent.ItemType<RainEnchant>()
                );
                other += GetBuildText(ModContent.ItemType<WizardEnchant>());
                meleeSpecific = new int[] { ModContent.ItemType<BeetleEnchant>(), ModContent.ItemType<TungstenEnchant>() };
                summonerSpecific = new int[] { ModContent.ItemType<SpookyEnchant>(), ModContent.ItemType<TikiEnchant>() };
            }
            else if (!NPC.downedAncientCultist)
            {
                summonType = ModContent.TryFind("Fargowiltas", "CultistSummon", out ModItem modItem) ? modItem.Type : ItemID.BossMaskCultist;
                build += GetBuildText(
                    Main.rand.NextBool() ? ModContent.ItemType<AeolusBoots>() : ModContent.ItemType<ValhallaKnightEnchant>(),
                    Main.rand.NextBool() ? ItemID.BetsyWings : ItemID.FishronWings,
                    ItemID.EmpressFlightBooster,
                    Main.rand.Next(new int[] { ModContent.ItemType<SupremeDeathbringerFairy>(), ModContent.ItemType<LihzahrdTreasureBox>(), ModContent.ItemType<BetsysHeart>(), ModContent.ItemType<MeteorEnchant>() })
                ) + GetBuildTextRandom(
                    2,
                    ModContent.ItemType<DubiousCircuitry>(),
                    ModContent.ItemType<DarkArtistEnchant>(),
                    ModContent.ItemType<LumpOfFlesh>(),
                    ModContent.ItemType<SpectreEnchant>()
                );
                other += GetBuildText(ModContent.ItemType<WizardEnchant>());
                meleeSpecific = new int[] { ModContent.ItemType<BeetleEnchant>(), ModContent.ItemType<TungstenEnchant>() };
                summonerSpecific = new int[] { ModContent.ItemType<SpookyEnchant>(), ModContent.ItemType<TikiEnchant>() };
            }
            else if (!NPC.downedMoonlord)
            {
                summonType = ModContent.TryFind("Fargowiltas", "CelestialSigil2", out ModItem modItem) ? modItem.Type : ItemID.CelestialSigil;
                build += GetBuildText(
                    ModContent.ItemType<GaiaHelmet>(),
                    ModContent.ItemType<GaiaPlate>(),
                    ModContent.ItemType<GaiaGreaves>()
                ) + " " + GetBuildText(

                    Main.rand.NextBool() ? ItemID.BetsyWings : ItemID.FishronWings,
                    ItemID.EmpressFlightBooster,
                    ModContent.ItemType<ChaliceoftheMoon>()
                ) + GetBuildTextRandom(
                    4,
                    Main.rand.NextBool() ? ModContent.ItemType<AeolusBoots>() : ModContent.ItemType<ValhallaKnightEnchant>(),
                    ModContent.ItemType<DubiousCircuitry>(),
                    ModContent.ItemType<PrecisionSeal>(),
                    ModContent.ItemType<MutantAntibodies>(),
                    ModContent.ItemType<DarkArtistEnchant>(),
                    ModContent.ItemType<LumpOfFlesh>(),
                    ModContent.ItemType<SpectreEnchant>()
                );
                other += GetBuildText(ModContent.ItemType<WizardEnchant>());
                meleeSpecific = new int[] { ModContent.ItemType<BeetleEnchant>(), ModContent.ItemType<TungstenEnchant>() };
                summonerSpecific = new int[] { ModContent.ItemType<SpookyEnchant>(), ModContent.ItemType<TikiEnchant>() };
            }
            else if (!WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.CosmosChampion])
            {
                summonType = ModContent.ItemType<SigilOfChampions>();
                build += GetBuildText(
                    ModContent.ItemType<FlightMasterySoul>(),
                    Main.rand.NextBool() ? ModContent.ItemType<SupersonicSoul>() : ModContent.ItemType<ColossusSoul>()
                    ) + GetBuildTextRandom(
                    4,
                    ModContent.ItemType<NebulaEnchant>(),
                    ModContent.ItemType<TerraForce>(),
                    ModContent.ItemType<EarthForce>(),
                    ModContent.ItemType<ShadowForce>(),
                    ModContent.ItemType<NatureForce>()
                );
                meleeSpecific = new int[] { ModContent.ItemType<BerserkerSoul>() };
                rangerSpecific = new int[] { ModContent.ItemType<SnipersSoul>() };
                mageSpecific = new int[] { ModContent.ItemType<ArchWizardsSoul>() };
                summonerSpecific = new int[] { ModContent.ItemType<ConjuristsSoul>(), ModContent.ItemType<SpiritForce>() };
            }
            else if (!WorldSavingSystem.DownedAbom)
            {
                summonType = ModContent.ItemType<AbomsCurse>();
                build += GetBuildText(
                    ModContent.ItemType<FlightMasterySoul>(),
                    ModContent.ItemType<UniverseCore>(),
                    ModContent.ItemType<ColossusSoul>()
                ) + GetBuildTextRandom(
                    3,
                    ModContent.ItemType<EarthForce>(),
                    ModContent.ItemType<CosmoForce>(),
                    ModContent.ItemType<SpiritForce>(),
                    ModContent.ItemType<NatureForce>(),
                    ModContent.ItemType<HeartoftheMasochist>()
                );
                meleeSpecific = new int[] { ModContent.ItemType<BerserkerSoul>() };
                rangerSpecific = new int[] { ModContent.ItemType<SnipersSoul>() };
                mageSpecific = new int[] { ModContent.ItemType<ArchWizardsSoul>() };
                summonerSpecific = new int[] { ModContent.ItemType<ConjuristsSoul>(), ModContent.ItemType<SpiritForce>() };
            }
            else if (!WorldSavingSystem.DownedMutant)
            {
                summonType = ModContent.ItemType<AbominationnVoodooDoll>();
                build += GetBuildText(
                    ModContent.ItemType<TerrariaSoul>(),
                    ModContent.ItemType<MasochistSoul>(),
                    ModContent.ItemType<UniverseSoul>(),
                    ModContent.ItemType<DimensionSoul>(),
                    ModContent.ItemType<SparklingAdoration>(),
                    ModContent.ItemType<AbominableWand>()
                );
                meleeSpecific = new int[] { ModContent.ItemType<BerserkerSoul>() };
                rangerSpecific = new int[] { ModContent.ItemType<SnipersSoul>() };
                mageSpecific = new int[] { ModContent.ItemType<ArchWizardsSoul>() };
                summonerSpecific = new int[] { ModContent.ItemType<ConjuristsSoul>() };
            }
            else
            {
                summonType = ModContent.ItemType<MutantsCurse>();
                build += GetBuildText(
                    ModContent.ItemType<MutantMask>(),
                    ModContent.ItemType<MutantBody>(),
                    ModContent.ItemType<MutantPants>()
                ) + " " + GetBuildText(
                    ModContent.ItemType<EternitySoul>(),
                    ModContent.ItemType<MasochistSoul>(),
                    ModContent.ItemType<UniverseSoul>(),
                    ModContent.ItemType<SparklingAdoration>(),
                    ModContent.ItemType<AbominableWand>(),
                    ModContent.ItemType<MutantEye>()
                );

                meleeSpecific = new int[] { ModContent.ItemType<BerserkerSoul>() };
                rangerSpecific = new int[] { ModContent.ItemType<SnipersSoul>() };
                mageSpecific = new int[] { ModContent.ItemType<ArchWizardsSoul>() };
                summonerSpecific = new int[] { ModContent.ItemType<ConjuristsSoul>() };
            }


            if (Main.hardMode)
            {
                if (NPC.downedMechBossAny)
                {
                    if (!player.inventory.Any(i => !i.IsAir && i.type == ModContent.ItemType<BionomicCluster>())
                    && !player.armor.Any(i => !i.IsAir && i.type == ModContent.ItemType<BionomicCluster>())
                    && !player.armor.Any(i => !i.IsAir && i.type == ModContent.ItemType<MasochistSoul>())
                    && !WorldSavingSystem.DownedAbom)
                    {
                        other += $" [i:{ModContent.ItemType<BionomicCluster>()}]";
                    }
                }

                if (ModContent.TryFind("Fargowiltas", "Omnistation", out ModItem omni1) && ModContent.TryFind("Fargowiltas", "Omnistation2", out ModItem omni2))
                {
                    bool hasOmni = false;
                    if (player.inventory.Any(i => !i.IsAir && (i.type == omni1.Type || i.type == omni2.Type)))
                        hasOmni = true;
                    else if (ModContent.TryFind("Fargowiltas", "Omnistation", out ModBuff omnibuff) && player.HasBuff(omnibuff.Type))
                        hasOmni = true;

                    if (!hasOmni)
                    {
                        other += Main.rand.NextBool()
                            ? $" [i:{omni1.Type}]"
                            : $" [i:{omni2.Type}]";
                    }
                }
            }

            string classSpecific = ClassSpecific(player, meleeSpecific, rangerSpecific, mageSpecific, summonerSpecific);

            build = Language.GetTextValue("Mods.FargowiltasSouls.Items.EternityAdvisor.General", build);

            if (!string.IsNullOrEmpty(classSpecific))
                classSpecific = "\n" + Language.GetTextValue("Mods.FargowiltasSouls.Items.EternityAdvisor.ClassSpecific", classSpecific);
            build += classSpecific;

            if (!string.IsNullOrEmpty(other))
                other = "\n" + Language.GetTextValue("Mods.FargowiltasSouls.Items.EternityAdvisor.Other", other);
            build += other;

            build += "\n" + Language.GetTextValue("Mods.FargowiltasSouls.Items.EternityAdvisor.Summon", $"[i:{summonType}]");

            return summonType;
        }
        private static string ClassSpecific(Player player, int[] melee = null, int[] ranged = null, int[] magic = null, int[] summoner = null)
        {
            double Damage(DamageClass damageClass) => Math.Round(player.GetTotalDamage(damageClass).Additive * player.GetTotalDamage(damageClass).Multiplicative * 100 - 100);
            double meleeDmg = Damage(DamageClass.Melee);
            double rangedDmg = Damage(DamageClass.Ranged);
            double mageDmg = Damage(DamageClass.Magic);
            double summonDmg = Damage(DamageClass.Summon);

            string output = string.Empty;
            int[] items;
            if (meleeDmg > Utils.Max(rangedDmg, mageDmg, summonDmg))
                items = melee;
            else if (rangedDmg > Math.Max(mageDmg, summonDmg))
                items = ranged;
            else if (mageDmg > summonDmg)
                items = magic;
            else
                items = summoner;

            if (items == null)
                return string.Empty;

            foreach (int item in items)
            {
                output += $"[i:{item}]";
            }
            return output;
        }
        public override bool? UseItem(Player player)
        {
            if (player.ItemTimeIsZero)
            {
                GetBossHelp(out string dialogue, player);
                if (player.whoAmI == Main.myPlayer)
                    Main.NewText(dialogue);

                SoundEngine.PlaySound(SoundID.Meowmere, player.Center);
            }
            return true;
        }
    }
}