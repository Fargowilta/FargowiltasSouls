using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Projectiles.Masomode;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Cavern
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
