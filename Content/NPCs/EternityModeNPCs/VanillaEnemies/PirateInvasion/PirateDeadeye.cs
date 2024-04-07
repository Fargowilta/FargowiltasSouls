using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.PirateInvasion
{
    public class PirateDeadeye : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.PirateDeadeye);

        public override void AI(NPC npc)
        {
            base.AI(npc);

            //num3 = 40, num5 = 14f, num8 = 0f, damage = 20/25, num12 = 550f?
            if (npc.ai[2] > 0f && npc.ai[1] <= 25f)
            {
                if (FargoSoulsUtil.HostCheck)
                {
                    Vector2 speed = Main.player[npc.target].Center - npc.Center;
                    speed.X += Main.rand.Next(-20, 21);
                    speed.Y += Main.rand.Next(-20, 21);
                    speed.Normalize();
                    speed *= 14f;

                    int damage = Main.expertMode ? 20 : 25;
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed, ProjectileID.MeteorShot, damage, 0f, Main.myPlayer);
                }
                SoundEngine.PlaySound(SoundID.Item11, npc.Center);
                npc.ai[2] = 0f;
                npc.ai[1] = 0f;
                npc.netUpdate = true;
            }
        }
    }
}
