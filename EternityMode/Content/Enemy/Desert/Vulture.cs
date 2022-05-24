using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Projectiles.Masomode;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Desert
{
    public class Vulture : Shooters
    {
        public Vulture() : base(150, ModContent.ProjectileType<VultureFeather>(), 10, 1, DustID.Sand, 500) { }

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Vulture);

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (npc.ai[0] == 0f) //no attack until moving
                AttackTimer = 0;
        }
    }
}
