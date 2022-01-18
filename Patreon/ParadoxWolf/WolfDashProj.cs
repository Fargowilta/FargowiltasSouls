using FargowiltasSouls.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.ParadoxWolf
{
    public class WolfDashProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Wolf Dash");
        }

        public override void SetDefaults()
        {
            projectile.width = Player.defaultHeight;
            projectile.height = Player.defaultHeight;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 20; //
            projectile.penetrate = -1;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;

            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (!player.active || player.dead)
            {
                projectile.Kill();
                return;
            }

            player.GetModPlayer<PatreonPlayer>().WolfDashing = true;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = player.GetModPlayer<FargoPlayer>().StardustEnchant;

            player.Center = projectile.Center;
            projectile.spriteDirection = -projectile.direction;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false; //dont kill proj when hits tiles
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects effects;

            if (projectile.spriteDirection < 0)
            {
                effects = SpriteEffects.FlipHorizontally;
            }
            else
            {
                effects = SpriteEffects.None;
            }

            //Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);

                spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, effects, 0f);
            }
            return true;
        }
    }
}