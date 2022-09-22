using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Patreon.Volknet.Projectiles
{
    public static class NPCUtils
    {

        public static bool AnyBosses()
        {
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && (npc.boss || (npc.type >= NPCID.EaterofWorldsBody && npc.type <= NPCID.EaterofWorldsTail)))
                {
                    return true;
                }
            }
            return false;
        }



        public static bool AnyProj(int type)
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == type)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool AnyProj(int type, int owner)
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == type && proj.owner == owner)
                {
                    return true;
                }
            }
            return false;
        }

    }
}