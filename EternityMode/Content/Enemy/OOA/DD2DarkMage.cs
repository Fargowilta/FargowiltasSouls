using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.OOA
{
    public class DD2DarkMage : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.DD2DarkMageT1,
            NPCID.DD2DarkMageT3
        );

        public override void AI(NPC npc)
        {
            base.AI(npc);

            int radius = npc.type == NPCID.DD2DarkMageT1 ? 600 : 900;

            EModeGlobalNPC.Aura(npc, radius, ModContent.BuffType<Lethargic>(), false, 254);
            foreach (NPC n in Main.npc.Where(n => n.active && !n.friendly && n.type != npc.type && n.Distance(npc.Center) < radius))
            {
                n.GetGlobalNPC<EModeGlobalNPC>().PaladinsShield = true;
                if (Main.rand.NextBool())
                {
                    int d = Dust.NewDust(n.position, n.width, n.height, 254, 0f, -3f, 0, new Color(), 1.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].noLight = true;
                }
            }
        }
    }
}
