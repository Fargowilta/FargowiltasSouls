using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon.Volknet.Projectiles
{
    public class PlasmaProj : ModProjectile
    {

        public override void SetStaticDefaults()
        {

            // DisplayName.SetDefault("Plasma Bolt");
            //DisplayName.AddTranslation(GameCulture.Chinese, "等离子束");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.SaucerLaser;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.alpha = 255;
            Projectile.timeLeft = 360;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 0.3f;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 2;
        }

        public override void OnKill(int timeLeft)
        {
            int num = Main.rand.Next(3, 7);
            for (int index1 = 0; index1 < num; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.Center - Projectile.velocity / 2f, 0, 0, DustID.ChlorophyteWeapon, 0.0f, 0.0f, 100, new Color(), 2.1f);
                Dust dust = Main.dust[index2];
                dust.velocity *= 2f;
                Main.dust[index2].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.WitheredArmor, 600);
            target.AddBuff(BuffID.Electrified, 600);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rectangle = texture2D13.Bounds;
            Vector2 origin2 = rectangle.Size() / 2f;
            Color color27 = Projectile.GetAlpha(lightColor);
            for (int i = 1; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero || Projectile.oldPos[i - 1] == Projectile.oldPos[i])
                    continue;
                Vector2 offset = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
                int length = (int)offset.Length();
                offset.Normalize();
                const int step = 3;
                Color color28 = color27;
                color28 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                for (int j = 0; j < length; j += step)
                {
                    Vector2 value5 = Projectile.oldPos[i] + offset * j;
                    Main.spriteBatch.Draw(texture2D13, value5 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color28, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.FlipHorizontally, 0f);
                }
            }
            //Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float slotsUsed = 0;

            Main.projectile.Where(x => x.active && x.owner == Main.player[Projectile.owner].whoAmI && x.minionSlots > 0).ToList().ForEach(x => { slotsUsed += x.minionSlots; });
        }
    }
}