using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Projectiles.Masomode;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Jungle
{
    public class Moth : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Moth);

        public int Counter;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lifeMax *= 2;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            npc.position += npc.velocity;
            for (int i = 0; i < 2; i++)
            {
                int d = Dust.NewDust(npc.position, npc.width, npc.height, 70);
                Main.dust[d].scale += 1f;
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 5f;
            }
            if (++Counter > 6)
            {
                Counter = 0;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Main.rand.NextVector2Unit() * 12f,
                        ModContent.ProjectileType<MothDust>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 0.8f), 0f, Main.myPlayer);
            }
        }
    }
}
