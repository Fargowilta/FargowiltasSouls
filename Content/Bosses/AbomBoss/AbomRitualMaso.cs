using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.AbomBoss
{
    public class AbomRitualMaso : BaseArena
    {
        private const float realRotation = -MathHelper.Pi / 180f;

        public AbomRitualMaso() : base(realRotation, 1100f, ModContent.NPCType<AbomBoss>(), 87) { }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Abominationn Seal");
        }

        protected override void Movement(NPC npc)
        {
            Projectile.velocity = npc.Center - Projectile.Center;
            if (npc.ai[0] != 8) //snaps directly to abom when preparing for p2 attack
                Projectile.velocity /= 40f;

            rotationPerTick = realRotation;
        }

        public override void AI()
        {
            base.AI();
            Projectile.rotation -= 1f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 0, 0) * Projectile.Opacity * (targetPlayer == Main.myPlayer ? 1f : 0.15f);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<Buffs.Boss.AbomFangBuff>(), 300);
                //player.AddBuff(ModContent.BuffType<Unstable>(), 240);
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.BerserkedBuff>(), 120);
            }
            target.AddBuff(BuffID.Bleeding, 600);
        }
    }
}