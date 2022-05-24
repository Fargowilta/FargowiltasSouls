using FargowiltasSouls.EternityMode.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Dungeon
{
    public class BoneLee : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.BoneLee);

        public override void ModifyHitByAnything(NPC npc, Player player, ref int damage, ref float knockback, ref bool crit)
        {
            base.ModifyHitByAnything(npc, player, ref damage, ref knockback, ref crit);

            if (Main.rand.NextBool(10) && npc.HasPlayerTarget && player.whoAmI == npc.target && player.active && !player.dead && !player.ghost)
            {
                bool doTheTeleport = true;

                Vector2 teleportTarget = player.Center;
                float offset = 100f * -player.direction;
                teleportTarget.X += offset;
                teleportTarget.Y -= 50f;
                if (!Collision.CanHit(teleportTarget, 1, 1, player.position, player.width, player.height))
                {
                    teleportTarget.X -= offset * 2f;
                    if (!Collision.CanHit(teleportTarget, 1, 1, player.position, player.width, player.height))
                        doTheTeleport = false;
                }

                if (doTheTeleport)
                {
                    FargoSoulsUtil.GrossVanillaDodgeDust(npc);
                    damage = 0;
                    npc.Center = teleportTarget;
                    npc.netUpdate = true;
                    FargoSoulsUtil.GrossVanillaDodgeDust(npc);
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Obstructed, 60);
            target.velocity.X = npc.velocity.Length() * npc.direction;
        }
    }
}
