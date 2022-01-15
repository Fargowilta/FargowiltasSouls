using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class SilverSword : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("SilverSword");
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.CloneDefaults(ProjectileID.DeadlySphere);
            aiType = ProjectileID.DeadlySphere;
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 18000;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.minionSlots = 0;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            if (player.whoAmI == Main.myPlayer && (!modPlayer.SilverEnchant || !player.GetToggleValue("Silver")))
            {
                projectile.Kill();
                return;
            }

            //dust!
            int dustId = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 2f), projectile.width, projectile.height + 5, DustID.SilverCoin, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default(Color), 1f);
            Main.dust[dustId].noGravity = true;
            int dustId3 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 2f), projectile.width, projectile.height + 5, DustID.SilverCoin,
                projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default(Color), 1f);
            Main.dust[dustId3].noGravity = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.X != oldVelocity.X) projectile.velocity.X = oldVelocity.X;
            if (projectile.velocity.Y != oldVelocity.Y) projectile.velocity.Y = oldVelocity.Y;
            return false;
        }
    }
}