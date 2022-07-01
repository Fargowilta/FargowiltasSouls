using FargowiltasSouls.Items.Accessories.Enchantments;
using FargowiltasSouls.Items.Accessories.Expert;
using FargowiltasSouls.Items.Accessories.Forces;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.Items.Accessories.Souls;
using FargowiltasSouls.Items.Armor;
using FargowiltasSouls.Items.Summons;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Misc
{
    public class EternityAdvisor : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eternity Advisor");
            Tooltip.SetDefault("Suggests loadouts for Eternity Mode based on progression");

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

        public override bool CanUseItem(Player player) => FargoSoulsWorld.EternityMode;

        private string GetBuildText(params int[] args)
        {
            string text = "";
            foreach (int itemType in args)
                text += $"[i:{itemType}]";
            return text;
        }

        private string GetBuildTextRandom(params int[] args) //takes number of accs to use as first param and list of accs as the rest
        {
            List<int> choices = new List<int>();
            int maxSize = args.Length - 1;
            for (int i = 0; i < args[0]; i++)
            {
                int attempt = Main.rand.Next(maxSize) + 1; //skip the first number
                if (choices.Contains(args[attempt])) //if already chose this acc, try to choose the next in line
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

        private int GetBossHelp(ref string build, Player player)
        {
            int summonType = -1;

            if (!FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.TrojanSquirrel])
            {
                summonType = ModContent.ItemType<SquirrelCoatofArms>();
                build = GetBuildText(
                    ModContent.ItemType<EurusSock>(),
                    ModContent.ItemType<PuffInABottle>()
                );
            }
            else if (!NPC.downedSlimeKing)
            {
                summonType = ItemID.SlimeCrown;
                build = GetBuildText(
                    ModContent.ItemType<EurusSock>(),
                    ModContent.ItemType<PuffInABottle>()
                ) + GetBuildTextRandom(
                    3,
                    ItemID.ShinyRedBalloon,
                    ItemID.BandofRegeneration,
                    ItemID.SharkToothNecklace,
                    ModContent.ItemType<EbonwoodEnchant>(),
                    ModContent.ItemType<CactusEnchant>(),
                    ModContent.ItemType<PalmWoodEnchant>(),
                    ModContent.ItemType<JungleEnchant>()
                );
            }
            else if (!NPC.downedBoss1)
            {
                summonType = ItemID.SuspiciousLookingEye;
                build = GetBuildText(
                    Main.rand.Next(new int[] { ItemID.HermesBoots, ItemID.SailfishBoots, ItemID.FlurryBoots }),
                    Main.rand.Next(new int[] { ItemID.CloudinaBottle, ItemID.TsunamiInABottle, ItemID.SandstorminaBottle, ItemID.BlizzardinaBottle })
                ) + GetBuildTextRandom(
                    3,
                    ItemID.CharmofMyths,
                    ItemID.CrossNecklace,
                    ItemID.SharkToothNecklace,
                    ModContent.ItemType<SlimyShield>(),
                    ModContent.ItemType<BorealWoodEnchant>(),
                    ModContent.ItemType<PalmWoodEnchant>(),
                    ModContent.ItemType<CactusEnchant>(),
                    ModContent.ItemType<JungleEnchant>()
                );
            }
            else if (!NPC.downedBoss2)
            {
                summonType = WorldGen.crimson ? ItemID.BloodySpine : ItemID.WormFood;
                build = GetBuildText(
                    Main.rand.NextBool() ? ItemID.EoCShield : ModContent.ItemType<JungleEnchant>(),
                    ItemID.SpectreBoots,
                    Main.rand.Next(new int[] { ItemID.BalloonHorseshoeFart, ItemID.BalloonHorseshoeSharkron, ItemID.WhiteHorseshoeBalloon })
                ) + GetBuildTextRandom(
                    2,
                    ItemID.CharmofMyths,
                    ModContent.ItemType<AgitatingLens>(),
                    ModContent.ItemType<LeadEnchant>(),
                    ModContent.ItemType<ShadewoodEnchant>(),
                    ModContent.ItemType<EbonwoodEnchant>(),
                    ModContent.ItemType<TungstenEnchant>()
                );
            }
            else if (!NPC.downedQueenBee)
            {
                summonType = ItemID.Abeemination;
                build = GetBuildText(
                    ItemID.EoCShield,
                    ItemID.SpectreBoots,
                    ItemID.Bezoar,
                    Main.rand.Next(new int[] { ItemID.BalloonHorseshoeFart, ItemID.BalloonHorseshoeSharkron, ItemID.WhiteHorseshoeBalloon }),
                    Main.rand.Next(new int[] {
                                ItemID.WormScarf,
                                ModContent.ItemType<GuttedHeart>(),
                                ModContent.ItemType<DarkenedHeart>(),
                                ModContent.ItemType<TungstenEnchant>()
                    })
                );
                build += $"[i:{ModContent.Find<ModItem>("Fargowiltas", "CityBuster").Type}]";
            }
            else if (!NPC.downedBoss3)
            {
                summonType = ModContent.TryFind("Fargowiltas", "SuspiciousSkull", out ModItem modItem) ? modItem.Type : ItemID.SkeletronMask;
                build = GetBuildText(
                    ItemID.EoCShield,
                    ItemID.LightningBoots,
                    ItemID.BalloonHorseshoeFart
                ) + GetBuildTextRandom(
                    2,
                    ItemID.CharmofMyths,
                    Main.rand.NextBool() ? ModContent.ItemType<GuttedHeart>() : ModContent.ItemType<DarkenedHeart>(),
                    ModContent.ItemType<QueenStinger>(),
                    ModContent.ItemType<ShadowEnchant>(),
                    ModContent.ItemType<IronEnchant>(),
                    ModContent.ItemType<TungstenEnchant>()
                );
            }
            else if (!NPC.downedDeerclops)
            {
                summonType = ModContent.TryFind("Fargowiltas", "DeerThing2", out ModItem modItem) ? modItem.Type : ItemID.DeerThing;
                build = GetBuildText(
                    ModContent.ItemType<JungleEnchant>(),
                    ItemID.LightningBoots,
                    ItemID.BalloonHorseshoeFart
                ) + GetBuildTextRandom(
                    2,
                    ItemID.HandWarmer,
                    ItemID.CharmofMyths,
                    ItemID.CrossNecklace,
                    ModContent.ItemType<DarkenedHeart>()
                );
            }
            else if (!FargoSoulsWorld.downedDevi)
            {
                summonType = ModContent.ItemType<DevisCurse>();
                build = GetBuildText(
                    ItemID.EoCShield,
                    ItemID.LightningBoots,
                    ItemID.BalloonHorseshoeFart,
                    ModContent.ItemType<NymphsPerfume>()
                ) + GetBuildTextRandom(
                    1,
                    ItemID.CharmofMyths,
                    ModContent.ItemType<DarkenedHeart>(),
                    ModContent.ItemType<QueenStinger>(),
                    ModContent.ItemType<IronEnchant>(),
                    ModContent.ItemType<TungstenEnchant>()
                );
            }
            else if (!Main.hardMode)
            {
                summonType = ModContent.TryFind("Fargowiltas", "FleshyDoll", out ModItem modItem) ? modItem.Type : ItemID.GuideVoodooDoll;
                build = GetBuildText(
                    ItemID.EoCShield,
                    ModContent.ItemType<ZephyrBoots>(),
                    ModContent.ItemType<SupremeDeathbringerFairy>()
                ) + GetBuildTextRandom(
                    2,
                    ItemID.CharmofMyths,
                    ItemID.CrossNecklace,
                    ModContent.ItemType<SparklingAdoration>(),
                    ModContent.ItemType<DarkenedHeart>(),
                    ModContent.ItemType<GuttedHeart>(),
                    ModContent.ItemType<MoltenEnchant>()
                );
                build += $"[i:{ModContent.Find<ModItem>("Fargowiltas", "DoubleObsidianInstabridge").Type}]";
            }
            else if (!NPC.downedQueenSlime)
            {
                summonType = ModContent.TryFind("Fargowiltas", "JellyCrystal", out ModItem modItem) ? modItem.Type : ItemID.QueenSlimeCrystal;
                build = GetBuildText(
                    ItemID.EoCShield,
                    ModContent.ItemType<ZephyrBoots>(),
                    ItemID.FrozenWings
                ) + GetBuildTextRandom(
                    3,
                    Main.rand.NextBool() ? ItemID.AnkhShield : ModContent.ItemType<DreadShell>(),
                    ModContent.ItemType<SparklingAdoration>(),
                    ModContent.ItemType<SupremeDeathbringerFairy>(),
                    ModContent.ItemType<TitaniumEnchant>(),
                    ModContent.ItemType<MoltenEnchant>(),
                    Main.rand.Next(new int[] { ItemID.WarriorEmblem, ItemID.RangerEmblem, ItemID.SorcererEmblem, ItemID.SummonerEmblem })
                );
            }
            else if (!NPC.downedMechBoss1)
            {
                summonType = ItemID.MechanicalWorm;
                build = GetBuildText(
                    ModContent.ItemType<ZephyrBoots>(),
                    Main.rand.Next(new int[] { ItemID.LeafWings, ItemID.FrozenWings, ModContent.ItemType<GelicWings>() })
                ) + GetBuildTextRandom(
                    4,
                    ItemID.EoCShield,
                    ItemID.CharmofMyths,
                    ModContent.ItemType<SupremeDeathbringerFairy>(),
                    ModContent.ItemType<FrostEnchant>(),
                    ModContent.ItemType<PalladiumEnchant>(),
                    ModContent.ItemType<DreadShell>(),
                    Main.rand.Next(new int[] { ItemID.WarriorEmblem, ItemID.RangerEmblem, ItemID.SorcererEmblem, ItemID.SummonerEmblem })
                );
            }
            else if (!NPC.downedMechBoss2)
            {
                summonType = ItemID.MechanicalEye;
                build = GetBuildText(
                    ModContent.ItemType<ZephyrBoots>(),
                    Main.rand.Next(new int[] { ItemID.LeafWings, ItemID.FrozenWings, ModContent.ItemType<GelicWings>() })
                ) + GetBuildTextRandom(
                    4,
                    ItemID.EoCShield,
                    ItemID.CharmofMyths,
                    ItemID.FrogLeg,
                    ModContent.ItemType<DreadShell>(),
                    ModContent.ItemType<BionomicCluster>(),
                    ModContent.ItemType<SupremeDeathbringerFairy>(),
                    ModContent.ItemType<CobaltEnchant>(),
                    ModContent.ItemType<PalladiumEnchant>(),
                    Main.rand.Next(new int[] { ItemID.WarriorEmblem, ItemID.RangerEmblem, ItemID.SorcererEmblem, ItemID.SummonerEmblem })
                );
            }
            else if (!NPC.downedMechBoss3)
            {
                summonType = ItemID.MechanicalSkull;
                build = GetBuildText(
                    ModContent.ItemType<ZephyrBoots>(),
                    Main.rand.Next(new int[] { ItemID.LeafWings, ItemID.FrozenWings, ModContent.ItemType<GelicWings>() })
                ) + GetBuildTextRandom(
                    4,
                    ItemID.EoCShield,
                    ItemID.CharmofMyths,
                    ItemID.FrogLeg,
                    ModContent.ItemType<DreadShell>(),
                    ModContent.ItemType<GuttedHeart>(),
                    ModContent.ItemType<SupremeDeathbringerFairy>(),
                    ModContent.ItemType<CobaltEnchant>(),
                    ModContent.ItemType<PalladiumEnchant>(),
                    ModContent.ItemType<MythrilEnchant>(),
                    Main.rand.Next(new int[] { ItemID.WarriorEmblem, ItemID.RangerEmblem, ItemID.SorcererEmblem, ItemID.SummonerEmblem })
                );
            }
            else if (!NPC.downedPlantBoss)
            {
                summonType = ModContent.TryFind("Fargowiltas", "PlanterasFruit", out ModItem modItem) ? modItem.Type : ItemID.PlanteraMask;
                build = GetBuildText(
                    ModContent.ItemType<AeolusBoots>(),
                    Main.rand.Next(new int[] { ItemID.FlameWings, ModContent.ItemType<GelicWings>() })
                ) + GetBuildTextRandom(
                    4,
                    Main.rand.Next(new int[] { ItemID.EoCShield, ModContent.ItemType<MonkEnchant>(), ModContent.ItemType<ChlorophyteEnchant>() }),
                    Main.rand.Next(new int[] { ItemID.AnkhShield, ModContent.ItemType<SupremeDeathbringerFairy>(), ModContent.ItemType<DubiousCircuitry>() }),
                    ItemID.CharmofMyths,
                    ItemID.CrossNecklace,
                    ModContent.ItemType<SparklingAdoration>(),
                    ModContent.ItemType<PureHeart>(),
                    ModContent.ItemType<MythrilEnchant>(),
                    ModContent.ItemType<HallowEnchant>(),
                    ItemID.AvengerEmblem
                );
                build += $"[i:{ModContent.Find<ModItem>("Fargowiltas", "CityBuster").Type}]";
            }
            else if (!NPC.downedGolemBoss)
            {
                summonType = ItemID.LihzahrdPowerCell;
                build = GetBuildText(
                    ModContent.ItemType<AeolusBoots>(),
                    ModContent.ItemType<DubiousCircuitry>(),
                    Main.rand.Next(new int[] { ItemID.SpookyWings, ItemID.FestiveWings, ItemID.Hoverboard })
                ) + GetBuildTextRandom(
                    3,
                    Main.rand.Next(new int[] { ItemID.Tabi, ModContent.ItemType<MonkEnchant>(), ModContent.ItemType<ChlorophyteEnchant>() }),
                    ItemID.CharmofMyths,
                    ItemID.WormScarf,
                    ItemID.AvengerEmblem,
                    ModContent.ItemType<PureHeart>(),
                    ModContent.ItemType<HallowEnchant>(),
                    ModContent.ItemType<LumpOfFlesh>()
                );
                build += $"[i:{ModContent.Find<ModItem>("Fargowiltas", "LihzahrdInstactuationBomb").Type}]";
            }
            else if (!FargoSoulsWorld.downedBetsy)
            {
                summonType = ModContent.TryFind("Fargowiltas", "BetsyEgg", out ModItem modItem) ? modItem.Type : ItemID.BossMaskBetsy;
                build = GetBuildText(
                    ModContent.ItemType<AeolusBoots>(),
                    ModContent.ItemType<LihzahrdTreasureBox>(),
                    Main.rand.NextBool() ? ItemID.SteampunkWings : ItemID.BeetleWings
                ) + GetBuildTextRandom(
                    3,
                    Main.rand.Next(new int[] { ItemID.Tabi, ModContent.ItemType<ShinobiEnchant>(), ModContent.ItemType<ChlorophyteEnchant>() }),
                    ModContent.ItemType<PureHeart>(),
                    ModContent.ItemType<BeetleEnchant>(),
                    ModContent.ItemType<SpectreEnchant>(),
                    ModContent.ItemType<SaucerControlConsole>(),
                    ModContent.ItemType<LumpOfFlesh>()
                );
            }
            else if (!NPC.downedFishron)
            {
                summonType = ModContent.TryFind("Fargowiltas", "TruffleWorm2", out ModItem modItem) ? modItem.Type : ItemID.TruffleWorm;
                build = GetBuildText(
                    ModContent.ItemType<AeolusBoots>(),
                    Main.rand.NextBool() ? ModContent.ItemType<DubiousCircuitry>() : ModContent.ItemType<LumpOfFlesh>(),
                    Main.rand.Next(new int[] { ItemID.SteampunkWings, ItemID.BetsyWings, ItemID.Hoverboard })
                ) + GetBuildTextRandom(
                    3,
                    Main.rand.Next(new int[] { ItemID.Tabi, ModContent.ItemType<ShinobiEnchant>(), ModContent.ItemType<ChlorophyteEnchant>() }),
                    ItemID.DestroyerEmblem,
                    ModContent.ItemType<PureHeart>(),
                    ModContent.ItemType<LihzahrdTreasureBox>(),
                    ModContent.ItemType<BetsysHeart>(),
                    ModContent.ItemType<BeetleEnchant>()
                );
            }
            else if (!NPC.downedEmpressOfLight)
            {
                summonType = ModContent.TryFind("Fargowiltas", "PrismaticPrimrose", out ModItem modItem) ? modItem.Type : ItemID.EmpressButterfly;
                build = GetBuildText(
                    ModContent.ItemType<AeolusBoots>(),
                    Main.rand.Next(new int[] { ItemID.BetsyWings, ItemID.FishronWings })
                ) + GetBuildTextRandom(
                    4,
                    Main.rand.Next(new int[] { ItemID.Tabi, ModContent.ItemType<ShinobiEnchant>(), ModContent.ItemType<ChlorophyteEnchant>() }),
                    Main.rand.Next(new int[] { ItemID.AnkhShield, ModContent.ItemType<DubiousCircuitry>(), ModContent.ItemType<LumpOfFlesh>() }),
                    ItemID.CharmofMyths,
                    ModContent.ItemType<BionomicCluster>(),
                    ModContent.ItemType<PureHeart>(),
                    ModContent.ItemType<MutantAntibodies>(),
                    ModContent.ItemType<BetsysHeart>(),
                    ModContent.ItemType<SparklingAdoration>()
                );
            }
            else if (!NPC.downedAncientCultist)
            {
                summonType = ModContent.TryFind("Fargowiltas", "CultistSummon", out ModItem modItem) ? modItem.Type : ItemID.BossMaskCultist;
                build = GetBuildText(
                    ModContent.ItemType<AeolusBoots>(),
                    Main.rand.NextBool() ? ItemID.BetsyWings : ItemID.FishronWings
                ) + GetBuildTextRandom(
                    4,
                    Main.rand.Next(new int[] { ItemID.Tabi, ModContent.ItemType<ShinobiEnchant>(), ModContent.ItemType<ChlorophyteEnchant>() }),
                    ItemID.CharmofMyths,
                    ItemID.DestroyerEmblem,
                    ModContent.ItemType<PureHeart>(),
                    ModContent.ItemType<DubiousCircuitry>(),
                    ModContent.ItemType<MutantAntibodies>(),
                    ModContent.ItemType<LihzahrdTreasureBox>(),
                    ModContent.ItemType<BetsysHeart>()
                );
            }
            else if (!NPC.downedMoonlord)
            {
                summonType = ModContent.TryFind("Fargowiltas", "CelestialSigil2", out ModItem modItem) ? modItem.Type : ItemID.CelestialSigil;
                build = GetBuildText(
                    ModContent.ItemType<GaiaHelmet>(),
                    ModContent.ItemType<GaiaPlate>(),
                    ModContent.ItemType<GaiaGreaves>()
                ) + " " + GetBuildText(
                    ModContent.ItemType<AeolusBoots>(),
                    ModContent.ItemType<ChaliceoftheMoon>(),
                    Main.rand.Next(new int[] { ItemID.BetsyWings, ItemID.FishronWings, ItemID.RainbowWings })
                ) + GetBuildTextRandom(
                    4,
                    Main.rand.Next(new int[] { ItemID.Tabi, ModContent.ItemType<ShinobiEnchant>(), ModContent.ItemType<ChlorophyteEnchant>() }),
                    Main.rand.NextBool() ? ItemID.AnkhShield : ModContent.ItemType<DubiousCircuitry>(),
                    ModContent.ItemType<PrecisionSeal>(),
                    ModContent.ItemType<MutantAntibodies>(),
                    ModContent.ItemType<BetsysHeart>(),
                    ModContent.ItemType<SparklingAdoration>()
                );
            }
            else if (!FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.CosmosChampion])
            {
                summonType = ModContent.ItemType<SigilOfChampions>();
                build = GetBuildText(
                    ModContent.ItemType<GaiaHelmet>(),
                    ModContent.ItemType<GaiaPlate>(),
                    ModContent.ItemType<GaiaGreaves>()
                ) + " " + GetBuildText(
                    ModContent.ItemType<SupersonicSoul>(),
                    ModContent.ItemType<FlightMasterySoul>(),
                    ModContent.ItemType<ColossusSoul>(),
                    Main.rand.Next(new int[] { ModContent.ItemType<BerserkerSoul>(), ModContent.ItemType<SnipersSoul>(), ModContent.ItemType<ArchWizardsSoul>(), ModContent.ItemType<ConjuristsSoul>() })
                ) + GetBuildTextRandom(
                    3,
                    ModContent.ItemType<NebulaEnchant>(),
                    ModContent.ItemType<PrecisionSeal>(),
                    ModContent.ItemType<HeartoftheMasochist>(),
                    ModContent.ItemType<TerraForce>(),
                    ModContent.ItemType<LifeForce>(),
                    ModContent.ItemType<SpiritForce>(),
                    ModContent.ItemType<EarthForce>()
                );
            }
            else if (!FargoSoulsWorld.downedAbom)
            {
                summonType = ModContent.ItemType<AbomsCurse>();
                build = GetBuildText(
                    ModContent.ItemType<CosmoForce>(),
                    ModContent.ItemType<SupersonicSoul>(),
                    ModContent.ItemType<FlightMasterySoul>(),
                    ModContent.ItemType<ColossusSoul>(),
                    ModContent.ItemType<UniverseCore>(),
                    Main.rand.Next(new int[] { ModContent.ItemType<BerserkerSoul>(), ModContent.ItemType<SnipersSoul>(), ModContent.ItemType<ArchWizardsSoul>(), ModContent.ItemType<ConjuristsSoul>() })
                ) + GetBuildTextRandom(
                    1,
                    ModContent.ItemType<HeartoftheMasochist>(),
                    ModContent.ItemType<PrecisionSeal>(),
                    ModContent.ItemType<SparklingAdoration>(),
                    ModContent.ItemType<TerraForce>(),
                    ModContent.ItemType<LifeForce>(),
                    ModContent.ItemType<EarthForce>(),
                    ModContent.ItemType<NatureForce>()
                );
            }
            else if (!FargoSoulsWorld.downedMutant)
            {
                summonType = ModContent.ItemType<AbominationnVoodooDoll>();
                build = GetBuildText(
                    ModContent.ItemType<TerrariaSoul>(),
                    ModContent.ItemType<MasochistSoul>(),
                    ModContent.ItemType<UniverseSoul>(),
                    ModContent.ItemType<DimensionSoul>(),
                    Main.rand.Next(new int[] { ModContent.ItemType<BerserkerSoul>(), ModContent.ItemType<SnipersSoul>(), ModContent.ItemType<ArchWizardsSoul>(), ModContent.ItemType<ConjuristsSoul>() }),
                    ModContent.ItemType<SparklingAdoration>(),
                    ModContent.ItemType<AbominableWand>()
                );
            }
            else
            {
                summonType = ModContent.ItemType<MutantsCurse>();
                build = GetBuildText(
                    ModContent.ItemType<MutantMask>(),
                    ModContent.ItemType<MutantBody>(),
                    ModContent.ItemType<MutantPants>()
                ) + " " + GetBuildText(
                    ModContent.ItemType<EternitySoul>(),
                    ModContent.ItemType<MasochistSoul>(),
                    ModContent.ItemType<UniverseSoul>(),
                    Main.rand.Next(new int[] { ModContent.ItemType<BerserkerSoul>(), ModContent.ItemType<SnipersSoul>(), ModContent.ItemType<ArchWizardsSoul>(), ModContent.ItemType<ConjuristsSoul>() }),
                    ModContent.ItemType<SparklingAdoration>(),
                    ModContent.ItemType<AbominableWand>(),
                    ModContent.ItemType<MutantEye>()
                );
            }

            build += $" [i:{summonType}]";

            if (Main.hardMode)
            {
                if (!player.inventory.Any(i => !i.IsAir && i.type == ModContent.ItemType<BionomicCluster>())
                    && !player.armor.Any(i => !i.IsAir && i.type == ModContent.ItemType<BionomicCluster>())
                    && !player.armor.Any(i => !i.IsAir && i.type == ModContent.ItemType<MasochistSoul>()))
                {
                    build += $" [i:{ModContent.ItemType<BionomicCluster>()}]";
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
                        build += Main.rand.NextBool()
                            ? $" [i:{omni1.Type}]"
                            : $" [i:{omni2.Type}]";
                    }
                }
            }

            return summonType;
        }

        public override bool? UseItem(Player player)
        {
            if (player.ItemTimeIsZero)
            {
                string dialogue = "";
                GetBossHelp(ref dialogue, player);
                if (player.whoAmI == Main.myPlayer)
                    Main.NewText(dialogue);

                SoundEngine.PlaySound(SoundID.Meowmere, player.Center);
            }
            return true;
        }
    }
}