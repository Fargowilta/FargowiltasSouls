using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class BigMimicExplosion : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_696";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Explosion");
            Main.projFrames[projectile.type] = Main.projFrames[ProjectileID.DD2ExplosiveTrapT3Explosion];
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.DD2ExplosiveTrapT3Explosion);
            aiType = ProjectileID.DD2ExplosiveTrapT3Explosion;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.usesLocalNPCImmunity = false;
            projectile.localNPCHitCooldown = 0;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 1;
        }

        public override bool CanDamage()
        {
            return projectile.frame == 1 || projectile.frame == 2;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Masomode.Midas>(), 600);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}