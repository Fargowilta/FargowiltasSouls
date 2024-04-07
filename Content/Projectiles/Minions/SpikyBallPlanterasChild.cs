using FargowiltasSouls.Content.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Minions
{
    public class SpikyBallPlanterasChild : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_277";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spiky Ball");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = 14;
            AIType = ProjectileID.SpikyBallTrap;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = oldVelocity.X * -0.95f;
            if (Projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y > 1f)
                Projectile.velocity.Y = oldVelocity.Y * -0.95f;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<InfestedBuff>(), 360);
            target.AddBuff(BuffID.Venom, 360);
            target.AddBuff(BuffID.Poisoned, 360);
        }
    }
}