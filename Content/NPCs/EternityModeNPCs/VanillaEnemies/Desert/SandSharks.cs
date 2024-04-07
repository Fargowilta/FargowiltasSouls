using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Desert
{
    public class SandSharks : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.SandShark,
            NPCID.SandsharkCorrupt,
            NPCID.SandsharkCrimson,
            NPCID.SandsharkHallow
        );

        public bool selfdestruct;
        public int deathTimer;

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            base.OnSpawn(npc, source);

            if (source is EntitySource_Parent parent && parent.Entity is Projectile projectile && projectile.type == ProjectileID.SandnadoHostile)
            {
                selfdestruct = true;
            }
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            if (Main.rand.NextBool(4))
            {
                int type = Main.rand.Next(NPCID.SandShark, NPCID.SandsharkHallow + 1);
                if (type != npc.type)
                    npc.Transform(type);
            }
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (selfdestruct && ++deathTimer > 4 * 60)
            {
                npc.life = 0;
                npc.HitEffect();
                npc.active = false;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
            }
        }
    }
}
