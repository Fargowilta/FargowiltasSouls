using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies
{
    public class PossessedWeapons : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.EnchantedSword,
            NPCID.CrimsonAxe,
            NPCID.CursedHammer
        );

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.scale = 2f;
            npc.lifeMax *= 2;
            npc.defense *= 2;
            npc.knockBackResist = 0f;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            npc.position += npc.velocity / 2f;
            EModeGlobalNPC.Aura(npc, 400, BuffID.WitheredArmor, true, 119);
            EModeGlobalNPC.Aura(npc, 400, BuffID.WitheredWeapon, true, 14);
            if (npc.ai[0] == 2f) //spinning up
                npc.ai[1] += 6f * (1f - (float)npc.life / npc.lifeMax); //FINISH SPINNING FASTER
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            if (npc.type == NPCID.EnchantedSword)
                target.AddBuff(ModContent.BuffType<PurifiedBuff>(), 300);
            else if (npc.type == NPCID.CursedHammer)
                target.AddBuff(ModContent.BuffType<DefenselessBuff>(), 300);
            else if (npc.type == NPCID.CrimsonAxe)
                target.AddBuff(ModContent.BuffType<InfestedBuff>(), 300);
        }
    }
}
