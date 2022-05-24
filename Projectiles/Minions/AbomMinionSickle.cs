using FargowiltasSouls.Buffs.Masomode;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class AbomMinionSickle : AbomBoss.AbomSickle3
    {
        public override string Texture => "FargowiltasSouls/Projectiles/AbomBoss/AbomSickle";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;

            Projectile.timeLeft = 180;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 5;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().noInteractionWithNPCImmunityFrames = true;

            CooldownSlot = -1;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit) { }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<MutantNibble>(), 600);
            target.AddBuff(BuffID.ShadowFlame, 600);
        }
    }
}
