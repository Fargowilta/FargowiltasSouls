using FargowiltasSouls.ItemDropRules.Conditions;
using System;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon
{
    public class PatreonGlobalNPC : GlobalNPC
    {
        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            if (SoulConfig.Instance.PatreonRoomba && type == NPCID.Steampunker)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<Gittle.RoombaPet>());
                shop.item[nextSlot].shopCustomPrice = 50000;
                nextSlot++;
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            string patreonDrop = FargoSoulsUtil.IsChinese() ? "捐赠者掉落" : "Patreon Drop";
            string patreonDescription = $"[i:{ModContent.ItemType<Gittle.RoombaPet>()}]{patreonDrop}";

            void AddPatreonDrop(Func<bool> condition, int item, int chanceDenominator = 1, string extra = default)
            {
                string description = patreonDescription;
                if (extra != default)
                    description += " " + extra;
                RuntimeDropCondition dropCondition = new RuntimeDropCondition(condition, description);
                npcLoot.Add(ItemDropRule.ByCondition(dropCondition, item, chanceDenominator));
            }

            switch (npc.type)
            {
                case NPCID.BrainofCthulhu:
                    AddPatreonDrop(
                        () => SoulConfig.Instance.PatreonCrimetroid,
                        ModContent.ItemType<Shucks.CrimetroidEgg>(),
                        25);
                    break;

                case NPCID.Golem:
                    AddPatreonDrop(
                        () => SoulConfig.Instance.PatreonOrb,
                        ModContent.ItemType<Daawnz.ComputationOrb>(),
                        10);
                    break;

                case NPCID.Squid:
                    AddPatreonDrop(
                        () => SoulConfig.Instance.PatreonCrimetroid,
                        ModContent.ItemType<Sam.SquidwardDoor>(),
                        50);
                    break;

                case NPCID.KingSlime:
                    AddPatreonDrop(
                        () => SoulConfig.Instance.PatreonCrimetroid && FargoSoulsWorld.EternityMode,
                        ModContent.ItemType<Catsounds.MedallionoftheFallenKing>(),
                        100,
                        FargoSoulsUtil.IsChinese() ? "在永恒模式" : "in Eternity Mode");
                    break;

                case NPCID.Dryad:
                    string PatreonPlantDropCondition = FargoSoulsUtil.IsChinese() ? "在血月的丛林" : " in Jungle on Blood Moon";
                    npcLoot.Add(ItemDropRule.ByCondition(
                        new PatreonPlantDropCondition(patreonDescription + $"{PatreonPlantDropCondition}"),
                        ModContent.ItemType<LaBonez.PiranhaPlantVoodooDoll>()));
                    break;

                case NPCID.MoonLordCore:
                    AddPatreonDrop(
                        () => SoulConfig.Instance.PatreonDevious && FargoSoulsWorld.EternityMode,
                        ModContent.ItemType<DevAesthetic.DeviousAestheticus>(),
                        20,
                        FargoSoulsUtil.IsChinese() ? "在永恒模式" : "in Eternity Mode");
                    break;

                case NPCID.SkeletronPrime:
                    AddPatreonDrop(
                        () => SoulConfig.Instance.PatreonPrime && FargoSoulsWorld.EternityMode,
                        ModContent.ItemType<Purified.PrimeStaff>(),
                        20,
                        FargoSoulsUtil.IsChinese() ? "在永恒模式" : "in Eternity Mode");
                    break;

                default:
                    break;
            }
        }
    }
}
