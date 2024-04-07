using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class BigMimicExplosion : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_696";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Explosion");
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.DD2ExplosiveTrapT3Explosion];
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.DD2ExplosiveTrapT3Explosion);
            AIType = ProjectileID.DD2ExplosiveTrapT3Explosion;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.usesLocalNPCImmunity = false;
            Projectile.localNPCHitCooldown = 0;
            Projectile.FargoSouls().DeletionImmuneRank = 1;
        }

        public override bool? CanDamage()
        {
            return Projectile.frame == 1 || Projectile.frame == 2;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Masomode.MidasBuff>(), 600);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}