using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class FossilBone : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_21";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Fossil Bone");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.Bone);
            projectile.aiStyle = -1;
            projectile.timeLeft = 900;
            projectile.tileCollide = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            projectile.rotation += 0.2f;

            projectile.velocity *= .95f;

            if (projectile.velocity.Length() < 0.1)
            {
                projectile.velocity = Vector2.Zero;
            }

            if (projectile.velocity == Vector2.Zero && player.Hitbox.Intersects(projectile.Hitbox))
            {
                player.GetModPlayer<FargoPlayer>().HealPlayer(15);
                projectile.Kill();
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.position += projectile.velocity;
            projectile.velocity = Vector2.Zero;
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.SandyBrown;
        }

        public override void Kill(int timeLeft)
        {
            const int max = 16;
            for (int i = 0; i < max; i++)
            {
                Vector2 vector6 = Vector2.UnitY * 5f;
                vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + projectile.Center;
                Vector2 vector7 = vector6 - projectile.Center;
                int d = Dust.NewDust(vector6 + vector7, 0, 0, DustID.Dirt, 0f, 0f, 0, default(Color), 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = vector7;
            }
        }
    }
}