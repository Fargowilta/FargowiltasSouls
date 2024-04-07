using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles
{
    public class LihzahrdSpikyBallFriendly : ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.SpikyBallTrap);
            AIType = ProjectileID.SpikyBallTrap;
            Projectile.hostile = false;
            Projectile.trap = false;
            Projectile.DamageType = DamageClass.Melee;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Venom, 300);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Venom, 300);
        }
    }
}