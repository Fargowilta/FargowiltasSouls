using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.BloodMoon
{
    public class Drippler : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Drippler);

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(ModContent.BuffType<RottingBuff>(), 600);
            target.AddBuff(ModContent.BuffType<AnticoagulationBuff>(), 600);
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (Main.rand.NextBool(3) && FargoSoulsUtil.HostCheck)
            {
                for (int i = 0; i < 4; i++)
                {
                    FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, NPCID.DemonEye,
                        velocity: new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-3, 3)));
                }
            }
        }
    }
}
