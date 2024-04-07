using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Hallow
{
    public class Gastropod : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Gastropod);

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Slimed, 120);
            target.AddBuff(ModContent.BuffType<FusedBuff>(), 1800);
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (FargoSoulsUtil.HostCheck && npc.HasPlayerTarget)
            {
                Vector2 vel = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * 4f;
                for (int i = 0; i < 12; i++)
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel.RotatedBy(2 * Math.PI / 12 * i), ProjectileID.PinkLaser, FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 0.8f), 0f, Main.myPlayer);
            }
        }
    }
}
