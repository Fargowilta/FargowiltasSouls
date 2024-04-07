using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Dungeon
{
    public class DungeonSkeletons : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.BlueArmoredBones,
            NPCID.BlueArmoredBonesMace,
            NPCID.BlueArmoredBonesNoPants,
            NPCID.BlueArmoredBonesSword,
            NPCID.HellArmoredBones,
            NPCID.HellArmoredBonesMace,
            NPCID.HellArmoredBonesSpikeShield,
            NPCID.HellArmoredBonesSword,
            NPCID.RustyArmoredBonesAxe,
            NPCID.RustyArmoredBonesFlail,
            NPCID.RustyArmoredBonesSword,
            NPCID.RustyArmoredBonesSwordNoArmor,
            NPCID.SkeletonSniper,
            NPCID.TacticalSkeleton,
            NPCID.SkeletonCommando,
            NPCID.DiabolistRed,
            NPCID.DiabolistWhite,
            NPCID.Necromancer,
            NPCID.NecromancerArmored,
            NPCID.RaggedCaster,
            NPCID.RaggedCasterOpenCoat
        );

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.Bone));
        }
    }
}
