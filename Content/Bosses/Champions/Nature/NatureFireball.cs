using FargowiltasSouls.Content.Bosses.Champions.Will;
using FargowiltasSouls.Core.Systems;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Bosses.Champions.Nature
{
    public class NatureFireball : WillFireball
    {
        public override string Texture => "Terraria/Images/Projectile_711";

        public override void SetDefaults()
        {
            base.SetDefaults();
            CooldownSlot = 1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
            if (!Projectile.tileCollide && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                Projectile.tileCollide = true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(BuffID.Burning, 300);
            target.AddBuff(BuffID.OnFire, 300);
        }
    }
}