using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class LunarCultistIceSpike : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Spike");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 90;
            Projectile.penetrate = -1;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().noInteractionWithNPCImmunityFrames = true;

            if (ModLoader.TryGetMod("Fargowiltas", out Mod fargo))
                fargo.Call("LowRenderProj", Projectile);
        }

        public override void AI()
        {
            Projectile.alpha += Projectile.timeLeft > 20 ? -10 : 10;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
            if (Projectile.alpha > 255)
                Projectile.alpha = 255;

            Projectile.position.X += Projectile.ai[0];
            Projectile.position.Y += Projectile.ai[1];

            int index3 = Dust.NewDust(Projectile.Center + Utils.RandomVector2(Main.rand, -8f, 8f) / 2f, 8, 8, 197, 0.0f, 0.0f, 100, Color.Transparent, 1f);
            Main.dust[index3].noGravity = true;

            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, 0.3f, 0.75f, 0.9f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 240);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 100) * Projectile.Opacity * 0.6f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rectangle = texture2D13.Bounds;
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}