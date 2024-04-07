using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.FrostMoon
{
    public class Flocko : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Flocko);

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (FargoSoulsUtil.HostCheck)
            {
                for (int i = 0; i < 10; i++)
                {
                    Vector2 speed = new(Main.rand.Next(-1000, 1001), Main.rand.Next(-1000, 1001));
                    speed.Normalize();
                    speed *= Main.rand.NextFloat(9f);
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + 4 * speed, speed, ProjectileID.FrostShard, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(ModContent.BuffType<HypothermiaBuff>(), 300);
            target.AddBuff(BuffID.Frostburn, 180);
        }
    }
}
