using FargowiltasSouls.Common.Graphics.Particles;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class ClingerFlame : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_482";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cursed Flames");
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.ClingerStaff];
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.ClingerStaff);
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.timeLeft = 10 * 60;
            Projectile.FargoSouls().DeletionImmuneRank = 1;
        }

        public override bool? CanDamage()
        {
            return Projectile.localAI[0] >= 60;
        }

        public override void AI()
        {
            if (++Projectile.localAI[0] < 60)
            {
                int dust = Dust.NewDust(Projectile.Bottom, 0, 0, DustID.CursedTorch, 0f, 0f, 100, default, 2f);
                Main.dust[dust].velocity.Y -= 2f;
                Main.dust[dust].velocity *= 2f;
                if (Main.rand.NextBool(4))
                {
                    Main.dust[dust].scale += 0.5f;
                    Main.dust[dust].noGravity = true;
                }

                Projectile.velocity = Vector2.Zero;

                for (int i = 0; i < 2; i++)
                {
                    Tile tile = Framing.GetTileSafely(Projectile.Bottom); //zip to an acceptable location
                    Projectile.position.Y += tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]) ? -16 : 16;
                }
            }
            else
            {
                int max = (int)(Projectile.width * Projectile.height * 0.0045f);
                for (int i = 0; i < max; i++)
                {
                    Vector2 pos = Main.rand.NextVector2FromRectangle(Projectile.Hitbox);
                    Particle p = new ExpandingBloomParticle(pos, new(Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-6, 6)), new(96, 248, 2), Vector2.One * 0.5f, Vector2.Zero, 10);
                    p.Spawn();
                    /*
                    p.Velocity *= 0.5f;
                    p.Velocity.Y -= 0.5f;
                    p.Position.X += 6f;
                    */
                    /*
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, 0.0f, 0.0f, 100, default, 1f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 0.5f;
                    Main.dust[d].velocity.Y -= 0.5f;
                    Main.dust[d].scale = 1.4f;
                    Main.dust[d].position.X += 6f;
                    Main.dust[d].position.Y -= 2f;
                    */
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.CursedInferno, 180);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
    }
}