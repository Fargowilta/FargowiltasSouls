using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Night
{
    public class HoppinJack : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.HoppinJack);

        public int Counter;

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (npc.life < npc.lifeMax && ++Counter >= 20 && npc.velocity.X != 0)
            {
                Counter = 0;
                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center.X, npc.position.Y, Main.rand.Next(-3, 4), Main.rand.Next(-4, 0), Main.rand.Next(326, 329), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 0.8f), 0f, Main.myPlayer);
            }
        }
    }
}
