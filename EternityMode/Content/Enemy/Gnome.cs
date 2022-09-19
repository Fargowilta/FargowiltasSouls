using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy
{
    public class Gnome : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Gnome);

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            if (Main.rand.NextBool(3))
                EModeGlobalNPC.Horde(npc, Main.rand.Next(2, 10));
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(Terraria.ModLoader.ModContent.BuffType<Buffs.Masomode.Unlucky>(), 60 * 30);
        }
    }
}
