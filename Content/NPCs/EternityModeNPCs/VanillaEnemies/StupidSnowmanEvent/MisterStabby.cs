using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.StupidSnowmanEvent
{
    public class MisterStabby : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.MisterStabby);

        public bool WasHit;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.Opacity /= 5;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (!WasHit)
            {
                npc.position.X += npc.velocity.X / 2;
            }
        }

        public override void OnHitByAnything(NPC npc, Player player, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitByAnything(npc, player, hit, damageDone);

            if (!WasHit)
            {
                WasHit = true;
                npc.Opacity *= 5;
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(ModContent.BuffType<MarkedforDeathBuff>(), 300);
            target.AddBuff(ModContent.BuffType<HypothermiaBuff>(), 300);
        }
    }
}
