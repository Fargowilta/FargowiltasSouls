using FargowiltasSouls.Content.Bosses.AbomBoss;
using FargowiltasSouls.Content.Patreon.Catsounds;
using FargowiltasSouls.Content.Patreon.Daawnz;
using FargowiltasSouls.Content.Patreon.DemonKing;
using FargowiltasSouls.Content.Patreon.DevAesthetic;
using FargowiltasSouls.Content.Patreon.Gittle;
using FargowiltasSouls.Content.Patreon.LaBonez;
using FargowiltasSouls.Content.Patreon.Purified;
using FargowiltasSouls.Content.Patreon.Sam;
using FargowiltasSouls.Content.Patreon.Shucks;
using FargowiltasSouls.Core;
using FargowiltasSouls.Core.ItemDropRules.Conditions;
using FargowiltasSouls.Core.Systems;
using System;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon
{
    public class PatreonGlobalNPC : GlobalNPC
    {
        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == NPCID.Steampunker)
                shop.Add(new Item(ModContent.ItemType<RoombaPet>()) { shopCustomPrice = Item.buyPrice(copper: 50000) }, new Condition("Mods.FargowiltasSouls.Conditions.RoombaPetSold", () => SoulConfig.Instance.PatreonRoomba));
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            void AddPatreonDrop(Func<bool> condition, int item, int chanceDenominator = 1, string extraKey = default)
            {
                string description = Language.GetTextValue("Mods.FargowiltasSouls.Conditions.Patreon");
                if (extraKey != default)
                    description = Language.GetTextValue($"Mods.FargowiltasSouls.Conditions.{extraKey}");
                RuntimeDropCondition dropCondition = new(condition, description);
                npcLoot.Add(ItemDropRule.ByCondition(dropCondition, item, chanceDenominator));
            }
            switch (npc.type)
            {
                case NPCID.BrainofCthulhu:
                    AddPatreonDrop(
                        () => SoulConfig.Instance.PatreonCrimetroid,
                        ModContent.ItemType<CrimetroidEgg>(),
                        25);
                    break;

                case NPCID.Golem:
                    AddPatreonDrop(
                        () => SoulConfig.Instance.PatreonOrb,
                        ModContent.ItemType<ComputationOrb>(),
                        10);
                    break;

                case NPCID.Squid:
                    AddPatreonDrop(
                        () => SoulConfig.Instance.PatreonDoor,
                        ModContent.ItemType<SquidwardDoor>(),
                        50);
                    break;

                case NPCID.KingSlime:
                    AddPatreonDrop(
                        () => SoulConfig.Instance.PatreonKingSlime && WorldSavingSystem.EternityMode,
                        ModContent.ItemType<MedallionoftheFallenKing>(),
                        100,
                        "PatreonEMode");
                    break;

                case NPCID.Dryad:
                    npcLoot.Add(ItemDropRule.ByCondition(
                        new PatreonPlantDropCondition(Language.GetTextValue("Mods.FargowiltasSouls.Conditions.PatreonPlant")),
                        ModContent.ItemType<PiranhaPlantVoodooDoll>()));
                    break;

                case NPCID.MoonLordCore:
                    AddPatreonDrop(
                        () => SoulConfig.Instance.PatreonDevious && WorldSavingSystem.EternityMode,
                        ModContent.ItemType<DeviousAestheticus>(),
                        20,
                        "PatreonEMode");
                    break;

                case NPCID.SkeletronPrime:
                    AddPatreonDrop(
                        () => SoulConfig.Instance.PatreonPrime && WorldSavingSystem.EternityMode,
                        ModContent.ItemType<PrimeStaff>(),
                        20,
                        "PatreonEMode");
                    break;

                default:
                    break;

            }
            if (npc.type == ModContent.NPCType<AbomBoss>())
            {
                AddPatreonDrop(
                        () => SoulConfig.Instance.PatreonFishron,
                        ModContent.ItemType<StaffOfUnleashedOcean>(),
                        25);
            }
        }
    }
}
