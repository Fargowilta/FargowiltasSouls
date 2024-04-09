using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Jungle
{
    public class GiantTortoise : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.GiantTortoise);

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            if (Main.rand.NextBool(3) && npc.FargoSouls().CanHordeSplit)
                EModeGlobalNPC.Horde(npc, Main.rand.Next(2, 6));
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            npc.reflectsProjectiles =
                npc.ai[0] == 3f //spinning
                && npc.HasValidTarget //while near player or line of sight
                && (npc.Distance(Main.player[npc.target].Center) < 10 * 16 || Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0));
        }

        public override void SafeOnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            base.SafeOnHitByItem(npc, player, item, hit, damageDone);

            if (npc.type == NPCID.GiantTortoise)
                player.Hurt(PlayerDeathReason.ByCustomReason(Language.GetTextValue("Mods.FargowiltasSouls.DeathMessage.GiantTortoise", player.name)), hit.Damage / 2, 0);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(ModContent.BuffType<DefenselessBuff>(), 300);

            target.FargoSouls().AddBuffNoStack(ModContent.BuffType<StunnedBuff>(), 60);
            target.velocity = Vector2.Normalize(target.Center - npc.Center) * 30;
        }
    }
}
