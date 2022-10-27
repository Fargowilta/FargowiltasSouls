using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Items.Accessories.Masomode;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy
{
    public class EarlyBirdEnemies : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.WyvernHead,
            NPCID.WyvernBody,
            NPCID.WyvernBody2,
            NPCID.WyvernBody3,
            NPCID.WyvernLegs,
            NPCID.WyvernTail,
            NPCID.Mimic,
            NPCID.IceMimic,
            NPCID.Medusa,
            NPCID.PigronCorruption,
            NPCID.PigronCrimson,
            NPCID.PigronHallow,
            NPCID.IchorSticker,
            NPCID.SeekerHead,
            NPCID.AngryNimbus,
            NPCID.RedDevil,
            NPCID.MushiLadybug,
            NPCID.AnomuraFungus,
            NPCID.ZombieMushroom,
            NPCID.ZombieMushroomHat,
            NPCID.IceGolem,
            NPCID.SandElemental
        );

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            if (!Main.hardMode)
            {
                npc.defense /= 2;

                if (npc.type == NPCID.IceGolem || npc.type == NPCID.SandElemental)
                {
                    npc.lifeMax = (int)Math.Round(npc.lifeMax * 0.4);
                    npc.defense /= 2;
                }
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            switch (npc.type)
            {
                case NPCID.Medusa:
                    npcLoot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == ItemID.MedusaHead && FargoSoulsUtil.LockEarlyBirdDrop(npcLoot, rule));
                    break;

                case NPCID.WyvernHead:
                    npcLoot.RemoveWhere(rule => rule is DropBasedOnExpertMode drop && drop.ruleForNormalMode is CommonDrop drop2 && drop2.itemId == ItemID.SoulofFlight && FargoSoulsUtil.LockEarlyBirdDrop(npcLoot, rule));
                    break;

                case NPCID.PigronHallow:
                case NPCID.PigronCorruption:
                case NPCID.PigronCrimson:
                    npcLoot.RemoveWhere(rule => rule is ItemDropWithConditionRule drop && drop.condition is Conditions.DontStarveIsUp && drop.itemId == ItemID.HamBat && FargoSoulsUtil.LockEarlyBirdDrop(npcLoot, rule));
                    npcLoot.RemoveWhere(rule => rule is ItemDropWithConditionRule drop && drop.condition is Conditions.DontStarveIsNotUp && drop.itemId == ItemID.HamBat && FargoSoulsUtil.LockEarlyBirdDrop(npcLoot, rule));
                    break;

                case NPCID.RedDevil:
                    npcLoot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == ItemID.UnholyTrident && FargoSoulsUtil.LockEarlyBirdDrop(npcLoot, rule));
                    FargoSoulsUtil.AddEarlyBirdDrop(npcLoot, ItemDropRule.Common(ItemID.DemonScythe, 3));
                    break;

                case NPCID.IchorSticker:
                    npcLoot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == ItemID.Ichor && FargoSoulsUtil.LockEarlyBirdDrop(npcLoot, rule));
                    FargoSoulsUtil.AddEarlyBirdDrop(npcLoot, ItemDropRule.OneFromOptions(1, ItemID.TheUndertaker, ItemID.TheRottedFork, ItemID.CrimsonRod, ItemID.CrimsonHeart, ItemID.PanicNecklace));
                    break;

                case NPCID.SeekerHead:
                    npcLoot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == ItemID.CursedFlame && FargoSoulsUtil.LockEarlyBirdDrop(npcLoot, rule));
                    FargoSoulsUtil.AddEarlyBirdDrop(npcLoot, ItemDropRule.OneFromOptions(1, ItemID.BallOHurt, ItemID.BandofStarpower, ItemID.Musket, ItemID.ShadowOrb, ItemID.Vilethorn));
                    break;

                case NPCID.Mimic:
                    npcLoot.RemoveWhere(rule => rule is OneFromOptionsDropRule drop && drop.dropIds.Contains(ItemID.DualHook) && FargoSoulsUtil.LockEarlyBirdDrop(npcLoot, rule));
                    FargoSoulsUtil.AddEarlyBirdDrop(npcLoot, ItemDropRule.OneFromOptions(1, ItemID.TitanGlove, ItemID.PhilosophersStone, ItemID.CrossNecklace, ItemID.DualHook));
                    break;

                case NPCID.IceMimic:
                    npcLoot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == ItemID.ToySled && FargoSoulsUtil.LockEarlyBirdDrop(npcLoot, rule));
                    FargoSoulsUtil.AddEarlyBirdDrop(npcLoot, ItemDropRule.OneFromOptions(1, ItemID.TitanGlove, ItemID.PhilosophersStone, ItemID.CrossNecklace, ItemID.DualHook));
                    break;

                case NPCID.AngryNimbus:
                    npcLoot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == ItemID.NimbusRod && FargoSoulsUtil.LockEarlyBirdDrop(npcLoot, rule));
                    FargoSoulsUtil.AddEarlyBirdDrop(npcLoot, ItemDropRule.Common(ItemID.FloatingIslandFishingCrate));
                    break;

                case NPCID.DuneSplicerHead:
                    FargoSoulsUtil.AddEarlyBirdDrop(npcLoot, ItemDropRule.Common(ItemID.SandstorminaBottle, 3));
                    FargoSoulsUtil.AddEarlyBirdDrop(npcLoot, ItemDropRule.Common(ItemID.OasisCrate));
                    break;

                case NPCID.IceGolem:
                    npcLoot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == ItemID.FrostCore && FargoSoulsUtil.LockEarlyBirdDrop(npcLoot, rule));
                    break;

                case NPCID.SandElemental:
                    npcLoot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == ItemID.AncientBattleArmorMaterial && FargoSoulsUtil.LockEarlyBirdDrop(npcLoot, rule));
                    break;

                default: break;
            }
        }
    }
}
