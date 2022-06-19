using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.AbomBoss
{
    public class AbomRitualMaso : BaseArena
    {
        private const float realRotation = -MathHelper.Pi / 180f;

        public AbomRitualMaso() : base(realRotation, 1100f, ModContent.NPCType<NPCs.AbomBoss.AbomBoss>(), 87) { }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Abominationn Seal");
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

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            base.OnHitPlayer(player, damage, crit);

            if (FargoSoulsWorld.EternityMode)
            {
                player.AddBuff(ModContent.BuffType<Buffs.Boss.AbomFang>(), 300);
                //player.AddBuff(ModContent.BuffType<Unstable>(), 240);
                player.AddBuff(ModContent.BuffType<Buffs.Masomode.Berserked>(), 120);
            }
            player.AddBuff(BuffID.Bleeding, 600);
        }
    }
}