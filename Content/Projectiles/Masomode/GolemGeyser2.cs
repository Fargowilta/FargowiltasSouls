using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class GolemGeyser2 : ModProjectile
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
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], NPCID.Golem);
            if (npc == null)
            {
                Projectile.Kill();
                return;
            }

            Tile tile = Framing.GetTileSafely(Projectile.Center);

            if (Projectile.ai[1] == 0) //spawned, while in ground tile
            {
                Projectile.position.Y -= 16; //go up

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

                    /*if (Projectile.timeLeft > 5)
                        Projectile.timeLeft = 5;
                    Projectile.extraUpdates = 0;
                    Projectile.position.Y += 16;
                    //make warning dusts
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 8f);
                    Main.dust[d].velocity *= 3f;*/
                }
                else //if in air, go down
                {
                    Projectile.position.Y += 16;
                }
            }

            /*if (Projectile.timeLeft <= 120) //about to erupt, make more dust
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);*/

            /*NPC golem = Main.npc[ai0];
            if (golem.GetGlobalNPC<NPCs.FargoSoulsGlobalNPC>().Counter == 2 && FargoSoulsUtil.HostCheck) //when golem does second stomp, erupt
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.UnitY * 8, ProjectileID.GeyserTrap, Projectile.damage, 0f, Main.myPlayer);
                Projectile.Kill();
                return;
            }*/
        }

        public override void OnKill(int timeLeft)
        {
            if (FargoSoulsUtil.HostCheck)
            {
                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, -Vector2.UnitY, ModContent.ProjectileType<GolemDeathraySmall>(), Projectile.damage, 0f, Main.myPlayer);
                //Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.UnitY * 8, ProjectileID.GeyserTrap, Projectile.damage, 0f, Main.myPlayer);
            }
        }
    }
}