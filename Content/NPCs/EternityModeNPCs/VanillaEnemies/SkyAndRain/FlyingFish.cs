using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.SkyAndRain
{
    public class FlyingFish : Shooters
    {
        public FlyingFish() : base(70, ProjectileID.WaterStream, 10, 1, DustID.Water, 250) { }

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.FlyingFish);

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);


            if (Main.rand.NextBool(4) && npc.FargoSouls().CanHordeSplit && WorldSavingSystem.DownedAnyBoss)
                EModeGlobalNPC.Horde(npc, Main.rand.Next(1, 5));

        }
        public override bool SafePreAI(NPC npc)
        {
            return base.SafePreAI(npc);
        }
    }
}
