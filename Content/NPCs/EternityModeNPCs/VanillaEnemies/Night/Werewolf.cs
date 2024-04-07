using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Night
{
	public class Werewolf : EModeNPCBehaviour
	{
		public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Werewolf);

		public override void AI(NPC npc)
		{
			base.AI(npc);

			EModeGlobalNPC.Aura(npc, 200, ModContent.BuffType<BerserkedBuff>(), false, 60);
		}

		public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
		{
			base.OnHitPlayer(npc, target, hurtInfo);

			target.AddBuff(BuffID.Rabies, 1800);
		}
	}
}
