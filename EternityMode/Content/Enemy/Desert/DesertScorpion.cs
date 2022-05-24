using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Desert
{
    public class DesertScorpion : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.DesertScorpionWalk,
            NPCID.DesertScorpionWall
        );

        public int Counter;

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (npc.type == NPCID.DesertScorpionWall && ++Counter > 240)
            {
                Counter = 0;

                if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget)
                {
                    Vector2 vel = npc.DirectionTo(Main.player[npc.target].Center) * 14;
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel, ModContent.ProjectileType<VenomSpit>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, Main.myPlayer);
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 300);
        }
    }
}
