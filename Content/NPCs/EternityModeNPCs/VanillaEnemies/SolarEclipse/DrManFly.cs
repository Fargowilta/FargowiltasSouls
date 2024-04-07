using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.SolarEclipse
{
    public class DrManFly : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.DrManFly);

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (FargoSoulsUtil.HostCheck)
            {
                for (int i = 0; i < 10; i++)
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11)),
                        ProjectileID.DrManFlyFlask, FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 4f / 2), 1f, Main.myPlayer);
            }
        }
    }
}
