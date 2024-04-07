using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class Spazmarang : ModProjectile
    {
        bool hitSomething = false;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spazmarang");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.light = 0.4f;

            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.penetrate = 1;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            //travelling out
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[1]++;

                if (Projectile.ai[1] > 20)
                {
                    Projectile.ai[0] = 1;
                    Projectile.netUpdate = true;
                }
            }
            //travel back to player
            else
            {
                Projectile.extraUpdates = 0;
                Projectile.velocity = Vector2.Normalize(Main.player[Projectile.owner].Center - Projectile.Center) * 45;

                //kill when back to player
                if (Projectile.Distance(Main.player[Projectile.owner].Center) <= 30)
                    Projectile.Kill();
            }

            //spin
            Projectile.rotation += Projectile.direction * -0.8f;

            //dust!
            int dustId = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 2f), Projectile.width, Projectile.height + 5, DustID.CursedTorch, Projectile.velocity.X * 0.2f,
                Projectile.velocity.Y * 0.2f, 100, default, 2f);
            Main.dust[dustId].noGravity = true;
        }

        private void spawnFire()
        {
            if (!hitSomething)
            {
                hitSomething = true;
                if (Projectile.owner == Main.myPlayer)
                {
                    SoundEngine.PlaySound(SoundID.Item74, Projectile.Center);
                    FargoSoulsUtil.XWay(12, Projectile.GetSource_FromThis(), Projectile.Center, ModContent.ProjectileType<EyeFireFriendly>(), 3, Projectile.damage / 2, Projectile.knockBack);
                }
                Projectile.ai[0] = 1;
                Projectile.penetrate = 4;
                Projectile.netUpdate = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, 120);
            spawnFire();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            spawnFire();
            Projectile.tileCollide = false;
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            //smaller tile hitbox
            width = 22;
            height = 22;
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = TextureAssets.Projectile[Projectile.type].Value;
            int num156 = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}