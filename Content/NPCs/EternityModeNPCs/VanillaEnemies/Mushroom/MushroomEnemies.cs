using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Mushroom
{
    public class MushroomEnemies : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.FungiBulb,
            NPCID.GiantFungiBulb,
            NPCID.AnomuraFungus,
            NPCID.MushiLadybug,
            NPCID.SporeBat,
            NPCID.ZombieMushroom,
            NPCID.ZombieMushroomHat,
            NPCID.SporeSkeleton,
            NPCID.FungoFish
        );

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (FargoSoulsUtil.HostCheck && Main.hardMode && Main.rand.NextBool())
            {
                if (NPC.CountNPCS(NPCID.FungiSpore) < 24)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        FargoSoulsUtil.NewNPCEasy(npc.GetSource_Death(), npc.Center, NPCID.FungiSpore,
                            velocity: 0.5f * new Vector2(Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5)));
                    }
                }
                else if (npc.type != NPCID.SporeBat)
                {
                    FargoSoulsUtil.NewNPCEasy(npc.GetSource_Death(), npc.Center, NPCID.SporeBat,
                        velocity: 0.5f * new Vector2(Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5)));
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Poisoned, 300);
        }

    }
}
