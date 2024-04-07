using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Desert
{
    public class Mummies : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.Mummy,
            NPCID.BloodMummy,
            NPCID.DarkMummy,
            NPCID.LightMummy
        );

        public override void AI(NPC npc)
        {
            base.AI(npc);

            EModeGlobalNPC.Aura(npc, 500, BuffID.Slow, false, 0);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.FargoSouls().AddBuffNoStack(BuffID.Webbed, 60);
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (Main.rand.NextBool(5))
            {
                for (int i = 0; i < 4; i++)
                    FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, NPCID.WallCreeper, velocity: new Vector2(Main.rand.NextFloat(-5f, 5f), -Main.rand.NextFloat(10f)));
            }
        }
    }
}
