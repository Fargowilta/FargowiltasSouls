using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class GolemGeyser : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.EmptyTexture;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Geyser");
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
            Projectile.extraUpdates = 14;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Tile tile = Framing.GetTileSafely(Projectile.Center);

            if (Projectile.ai[1] == 0) //spawned, while in ground tile
            {
                Projectile.position.Y += 16; //go down

                if (!(tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && tile.TileType != TileID.Platforms && tile.TileType != TileID.PlanterBox)) //if reached air tile
                {
                    Projectile.ai[1] = 1;
                    Projectile.netUpdate = true;
                }
            }
            else //has exited ground tiles and reached air tiles, now stop the next time you reach a ground tile
            {
                if (tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && tile.TileType != TileID.Platforms && tile.TileType != TileID.PlanterBox) //if inside solid tile, go back down
                {
                    Projectile.Kill();
                    return;
                }
                else //if in air, go up
                {
                    Projectile.position.Y -= 16;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (FargoSoulsUtil.HostCheck)
            {
                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.UnitY, ModContent.ProjectileType<GolemDeathraySmall>(), Projectile.damage, 0f, Main.myPlayer);
                //Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.UnitY * 8, ProjectileID.GeyserTrap, Projectile.damage, 0f, Main.myPlayer);
            }
        }
    }
}