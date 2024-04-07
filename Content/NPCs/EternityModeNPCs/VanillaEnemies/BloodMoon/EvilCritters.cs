using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.BloodMoon
{
	public class EvilCritters : EModeNPCBehaviour
	{
		public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
			NPCID.CorruptBunny,
			NPCID.CrimsonBunny,
			NPCID.CorruptGoldfish,
			NPCID.CrimsonGoldfish,
			NPCID.CorruptPenguin,
			NPCID.CrimsonPenguin
		);

		public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
		{
			base.OnHitPlayer(npc, target, hurtInfo);

			target.AddBuff(ModContent.BuffType<SqueakyToyBuff>(), 120);
		}
	}
}
