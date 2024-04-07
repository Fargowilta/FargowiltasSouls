using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.ChallengerItems
{
    public class TheLightning : LightningArc
    {
        public override string Texture => "Terraria/Images/Projectile_466";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;

            Projectile.usesIDStaticNPCImmunity = false;
            Projectile.idStaticNPCHitCooldown = 0;
            Projectile.FargoSouls().noInteractionWithNPCImmunityFrames = false;
        }

        float collideHeight;

        public override bool PreAI()
        {
            if (collideHeight == 0)
            {
                collideHeight = Projectile.ai[1];
                Projectile.ai[1] = Main.rand.Next(80);
                Projectile.netUpdate = true;
            }
            return base.PreAI();
        }

        public override void AI()
        {
            base.AI();

            if (Projectile.Center.Y > collideHeight)
                Projectile.tileCollide = true;
        }
    }
}