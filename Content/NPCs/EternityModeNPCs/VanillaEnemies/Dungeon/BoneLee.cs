using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Dungeon
{
    public class BoneLee : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.BoneLee);

        public override void ModifyHitByAnything(NPC npc, Player player, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitByAnything(npc, player, ref modifiers);

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
                    modifiers.Null();
                    npc.Center = teleportTarget;
                    npc.netUpdate = true;
                    FargoSoulsUtil.GrossVanillaDodgeDust(npc);
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Obstructed, 60);
            target.velocity.X = npc.velocity.Length() * npc.direction;
        }
    }
}
