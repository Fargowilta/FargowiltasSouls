using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class ClownBomb : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_75";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Clown Bomb");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.HappyBomb);
            AIType = ProjectileID.HappyBomb;

            Projectile.timeLeft = 300;
            Projectile.tileCollide = true;
            CooldownSlot = 1;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.DarkRed;
        }

        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center,
                Vector2.Zero, ModContent.ProjectileType<FusedExplosion>(), Projectile.damage * 4, Projectile.knockBack, Main.myPlayer);
        }
    }
}