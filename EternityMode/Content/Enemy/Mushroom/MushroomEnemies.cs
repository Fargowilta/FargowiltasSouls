using FargowiltasSouls.EternityMode.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Mushroom
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

            if (Main.netMode != NetmodeID.MultiplayerClient && Main.hardMode && Main.rand.NextBool())
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

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Poisoned, 300);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.GlowingMushroom, 1, 1, 5));
            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.MushroomGrassSeeds, 5));
            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.TruffleWorm, 20));
        }
    }
}
