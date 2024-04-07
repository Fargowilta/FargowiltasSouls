using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies
{
	public class EatersAndCrimeras : Shooters
	{
		public EatersAndCrimeras() : base(420, ModContent.ProjectileType<CursedFlameHostile2>(), 8f, 0.8f, 75, 600, 45) { }

		public override NPCMatcher CreateMatcher() =>
			new NPCMatcher().MatchTypeRange(
				NPCID.EaterofSouls,
				NPCID.BigEater,
				NPCID.LittleEater,
				NPCID.Crimera,
				NPCID.BigCrimera,
				NPCID.LittleCrimera
			);

		public override void OnFirstTick(NPC npc)
		{
			base.OnFirstTick(npc);

			if (NPC.downedBoss2 && Main.rand.NextBool(5) && npc.FargoSouls().CanHordeSplit)
				EModeGlobalNPC.Horde(npc, 5);
		}

		public override void AI(NPC npc)
		{
			base.AI(npc);

			if (npc.noTileCollide && Collision.SolidCollision(npc.Center, 0, 0)) //in a wall
			{
				int d = Dust.NewDust(npc.position, npc.width, npc.height, npc.type == NPCID.EaterofSouls ? 27 : 6, npc.velocity.X, npc.velocity.Y);
				Main.dust[d].noGravity = true;

				npc.position -= npc.velocity / 2f;

				if (AttackTimer > AttackThreshold - 120)
					AttackTimer = AttackThreshold - 120;
			}

			if (npc.type != NPCID.EaterofSouls)
				AttackTimer = 0;
		}

		public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
		{
			base.OnHitPlayer(npc, target, hurtInfo);

			target.AddBuff(BuffID.Weak, 300);
		}
	}
}
