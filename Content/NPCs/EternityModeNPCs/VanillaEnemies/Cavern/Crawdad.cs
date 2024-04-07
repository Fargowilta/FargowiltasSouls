using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.NPCMatching;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Cavern
{
    public class Crawdad : Shooters
    {
        public Crawdad() : base(10, ModContent.ProjectileType<BubbleHostile>(), 6, 1, DustID.Water, 100, 0) { }

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.Crawdad,
            NPCID.Crawdad2
        );
    }
}
