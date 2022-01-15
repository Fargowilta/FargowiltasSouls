using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class ClownBomb : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_75";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Clown Bomb");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.HappyBomb);
            aiType = ProjectileID.HappyBomb;

            projectile.timeLeft = 300;
            projectile.tileCollide = true;
            cooldownSlot = 1;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.DarkRed;
        }

        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<FuseBomb>(), 300, 3f, Main.myPlayer);
        }
    }
}