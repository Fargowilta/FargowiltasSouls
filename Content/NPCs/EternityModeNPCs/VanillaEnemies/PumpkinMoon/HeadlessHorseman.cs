using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.PumpkinMoon
{
    public class HeadlessHorseman : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.HeadlessHorseman);

        public int Counter;

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (++Counter > 360)
            {
                Counter = 0;
                if (FargoSoulsUtil.HostCheck && npc.HasPlayerTarget && npc.Distance(Main.player[npc.target].Center) < 800)
                {
                    Vector2 vel = (Main.player[npc.target].Center - npc.Center) / 60f;
                    if (vel.Length() < 12f)
                        vel = Vector2.Normalize(vel) * 12f;
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel, ModContent.ProjectileType<HorsemansBlade>(),
                        FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage, 0.8f), 0f, Main.myPlayer, npc.target);
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.FargoSouls().AddBuffNoStack(BuffID.Cursed, 30);
            target.AddBuff(ModContent.BuffType<LivingWastelandBuff>(), 600);
        }
    }
}
