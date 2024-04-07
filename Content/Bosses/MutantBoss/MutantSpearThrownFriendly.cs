using FargowiltasSouls.Content.Projectiles.BossWeapons;

namespace FargowiltasSouls.Content.Bosses.MutantBoss
{
    public class MutantSpearThrownFriendly : HentaiSpearThrown
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/BossWeapons/HentaiSpear";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = Terraria.ModLoader.DamageClass.Default;
        }
    }
}