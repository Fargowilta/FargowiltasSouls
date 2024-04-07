using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Dungeon
{
    public class ArmoredBones : AngryBones
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.RustyArmoredBonesAxe,
            NPCID.RustyArmoredBonesFlail,
            NPCID.RustyArmoredBonesSword,
            NPCID.RustyArmoredBonesSwordNoArmor,
            NPCID.BlueArmoredBones,
            NPCID.BlueArmoredBonesMace,
            NPCID.BlueArmoredBonesNoPants,
            NPCID.BlueArmoredBonesSword,
            NPCID.HellArmoredBones,
            NPCID.HellArmoredBonesMace,
            NPCID.HellArmoredBonesSpikeShield,
            NPCID.HellArmoredBonesSword
        );

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            if (Main.rand.NextBool(5))
                npc.Transform(Main.rand.Next(NPCID.RustyArmoredBonesAxe, NPCID.HellArmoredBonesSword + 1));
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(ModContent.BuffType<BloodthirstyBuff>(), 180);
        }
    }
}
