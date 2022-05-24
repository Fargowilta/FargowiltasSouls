using FargowiltasSouls.EternityMode.NPCMatching;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy.PumpkinMoon
{
    public class Splinterling : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Hellhound);

        public int Counter;

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (++Counter >= 300)
            {
                Counter = 0;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center.X, npc.Center.Y, Main.rand.Next(-3, 4), Main.rand.Next(-5, 0),
                            Main.rand.Next(326, 329), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                    }
                }
            }
        }

        public override bool CheckDead(NPC npc)
        {
            if (!NPC.downedPlantBoss)
            {
                npc.active = false;
                if (npc.DeathSound != null)
                    SoundEngine.PlaySound(npc.DeathSound.Value, npc.Center);
                return false;
            }

            return base.CheckDead(npc);
        }
    }
}
