using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Jungle
{
	public class Snatchers : EModeNPCBehaviour
	{
		public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
			NPCID.Snatcher,
			NPCID.ManEater,
			NPCID.AngryTrapper
		);

		public int DashTimer;
		public int BiteTimer;
		public int BittenPlayer = -1;

		public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
		{
			base.SendExtraAI(npc, bitWriter, binaryWriter);

			binaryWriter.Write7BitEncodedInt(DashTimer);
			binaryWriter.Write7BitEncodedInt(BiteTimer);
			binaryWriter.Write7BitEncodedInt(BittenPlayer);
		}

		public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
		{
			base.ReceiveExtraAI(npc, bitReader, binaryReader);

			DashTimer = binaryReader.Read7BitEncodedInt();
			BiteTimer = binaryReader.Read7BitEncodedInt();
			BittenPlayer = binaryReader.Read7BitEncodedInt();
		}

		public override void SetDefaults(NPC npc)
		{
			base.SetDefaults(npc);

			npc.damage = (int)(2.0 / 3.0 * npc.damage);
		}

		public override void OnFirstTick(NPC npc)
		{
			base.OnFirstTick(npc);

			npc.buffImmune[BuffID.Poisoned] = true;
			npc.buffImmune[BuffID.Venom] = true;
		}

		public override void AI(NPC npc)
		{
			base.AI(npc);

			int dashTime = npc.type == NPCID.AngryTrapper ? 120 : 360;

			if (BittenPlayer != -1)
			{
				DashTimer = 0;

				Player victim = Main.player[BittenPlayer];
				if (BiteTimer > 0 && victim.active && !victim.ghost && !victim.dead
					&& (npc.Distance(victim.Center) < 160 || victim.whoAmI != Main.myPlayer)
					&& victim.FargoSouls().MashCounter < 20)
				{
					victim.AddBuff(ModContent.BuffType<GrabbedBuff>(), 2);
					victim.velocity = Vector2.Zero;
					npc.Center = victim.Center;
				}
				else
				{
					BittenPlayer = -1;
					BiteTimer = -90; //cooldown

					//retract towards home
					npc.velocity = 15f * npc.SafeDirectionTo(new Vector2(npc.ai[0] * 16, npc.ai[1] * 16));

					npc.netUpdate = true;
					NetSync(npc);
				}
			}
			else if (++DashTimer > dashTime && npc.Distance(new Vector2((int)npc.ai[0] * 16, (int)npc.ai[1] * 16)) < 1000 && npc.HasValidTarget)
			{
				DashTimer = 0;
				npc.velocity = 15f * Vector2.Normalize(Main.player[npc.target].Center - npc.Center);
			}

			if (DashTimer == dashTime - 30)
				NetSync(npc);

			if (BiteTimer < 0)
				BiteTimer++;
			if (BiteTimer > 0)
				BiteTimer--;
		}

		public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
		{
			base.ModifyHitPlayer(npc, target, ref modifiers);

			target.longInvince = true;
		}

		public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
		{
			base.OnHitPlayer(npc, target, hurtInfo);

			target.AddBuff(BuffID.Bleeding, 300);

			if (BittenPlayer == -1 && BiteTimer == 0)
			{
				BittenPlayer = target.whoAmI;
				BiteTimer = 360;
				NetSync(npc, false);
			}

			if (WorldSavingSystem.MasochistModeReal && npc.type == NPCID.ManEater && target.Male)
			{
				target.KillMe(PlayerDeathReason.ByCustomReason(Language.GetTextValue("Mods.FargowiltasSouls.DeathMessage.Snatchers", target.name)), 999999, 0);
			}
		}

		public override void OnKill(NPC npc)
		{
			Player player = FargoSoulsUtil.PlayerExists(npc.lastInteraction);
			int chance = player != null && player.FargoSouls().HasJungleRose ? 20 : 200;
			if (Main.rand.NextBool(chance))
			{
				Item.NewItem(npc.GetSource_Loot(), npc.Hitbox, ModContent.Find<ModItem>("Fargowiltas", "PlanterasFruit").Type);
			}
		}
	}
}
