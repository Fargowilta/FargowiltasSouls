using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class FleshLaser : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_100";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Red Laser");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.PurpleLaser);
            AIType = ProjectileID.PurpleLaser;
            Projectile.penetrate = 1;
            //Projectile.usesIDStaticNPCImmunity = true;
            //Projectile.idStaticNPCHitCooldown = 10;
            //Projectile.FargoSouls().noInteractionWithNPCImmunityFrames = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] == 1)
            {
                Main.instance.LoadProjectile(ProjectileID.PurpleLaser);
                Texture2D texture = TextureAssets.Projectile[ProjectileID.PurpleLaser].Value;
                FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor, texture);
                return false;
            }
            else return true;
        }
    }
}