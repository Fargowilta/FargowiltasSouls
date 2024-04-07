using Terraria;

namespace FargowiltasSouls.Content.Projectiles.Deathrays
{
    public abstract class AbomSpecialDeathray : BaseDeathray
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Deathrays/AbomSpecialDeathray";
        public AbomSpecialDeathray(int maxTime) : base(maxTime, sheeting: TextureSheeting.Horizontal) { }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Projectile.type] = 11;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] % Projectile.MaxUpdates == 0)
            {
                if (++Projectile.frameCounter > 2)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= Main.projFrames[Projectile.type])
                        Projectile.frame = 0;
                }
            }
        }
    }
}