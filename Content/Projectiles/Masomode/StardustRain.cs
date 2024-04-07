using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class StardustRain : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_539";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cosmic Invader");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;

            Projectile.hide = true;

            Projectile.FargoSouls().DeletionImmuneRank = 2;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (++Projectile.ai[0] > 2)
            {
                Projectile.ai[0] = 0;

                const int spacing = 160;

                if (FargoSoulsUtil.HostCheck)
                {
                    for (int i = -1; i <= 1; i++)
                    {
                        if (i == 0)
                            continue;
                        Vector2 spawnPos = Projectile.Center;
                        spawnPos.X += spacing * Projectile.ai[1] * i;
                        Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), spawnPos, Vector2.UnitY * 7f, ProjectileID.StardustJellyfishSmall, Projectile.damage, 0f, Main.myPlayer, 210);
                    }
                }

                if (++Projectile.ai[1] > 1600 / spacing)
                {
                    Projectile.Kill();
                }
            }
        }
    }
}