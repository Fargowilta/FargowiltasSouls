using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class ClingerFlame : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_482";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Cursed Flames");
            Main.projFrames[projectile.type] = Main.projFrames[ProjectileID.ClingerStaff];
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.ClingerStaff);
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.magic = false;
            projectile.timeLeft = 10 * 60;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 1;
        }

        public override bool CanDamage()
        {
            return projectile.localAI[0] >= 60;
        }

        public override void AI()
        {
            if (++projectile.localAI[0] < 60)
            {
                int dust = Dust.NewDust(projectile.Bottom, 0, 0, 75, 0f, 0f, 100, default, 2f);
                Main.dust[dust].velocity.Y -= 2f;
                Main.dust[dust].velocity *= 2f;
                if (Main.rand.NextBool(4))
                {
                    Main.dust[dust].scale += 0.5f;
                    Main.dust[dust].noGravity = true;
                }

                projectile.velocity = Vector2.Zero;

                for (int i = 0; i < 2; i++)
                {
                    Tile tile = Framing.GetTileSafely(projectile.Bottom); //zip to an acceptable location
                    projectile.position.Y += tile.nactive() && (Main.tileSolid[tile.type] || Main.tileSolidTop[tile.type]) ? -16 : 16;
                }
            }
            else
            {
                int max = (int)(projectile.width * projectile.height * 0.0045f);
                for (int i = 0; i < max; i++)
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 75, 0.0f, 0.0f, 100, default, 1f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 0.5f;
                    Main.dust[d].velocity.Y -= 0.5f;
                    Main.dust[d].scale = 1.4f;
                    Main.dust[d].position.X += 6f;
                    Main.dust[d].position.Y -= 2f;
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.CursedInferno, 180);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough);
        }
    }
}