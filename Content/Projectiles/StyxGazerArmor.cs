using Terraria;

namespace FargowiltasSouls.Content.Projectiles
{
    public class StyxGazerArmor : BossWeapons.StyxGazer
    {
        public override void AI()
        {
            Projectile.damage = FargoSoulsUtil.HighestDamageTypeScaling(Main.player[Projectile.owner], 666);

            base.AI();

            Main.player[Projectile.owner].itemTime = 0;
            Main.player[Projectile.owner].itemAnimation = 0;
            if (Main.player[Projectile.owner].reuseDelay < 17)
                Main.player[Projectile.owner].reuseDelay = 17;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.localNPCImmunity[target.whoAmI] >= 15)
                return false;
            return null;
        }
    }
}