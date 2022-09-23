using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class HentaiSpearHeld : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/HentaiSpear";

        public const int useTime = 90;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Penetrator");
            /*ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;*/
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 0;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.hide = true;

            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
                return true;

            float length = 200;
            float dummy = 0f;
            Vector2 offset = length / 2 * Projectile.scale * (Projectile.rotation - MathHelper.ToRadians(135f)).ToRotationVector2();
            Vector2 end = Projectile.Center - offset;
            Vector2 tip = Projectile.Center + offset;

            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), end, tip, 8f * Projectile.scale, ref dummy))
                return true;

            return false;
        }

        public override void AI()
        {
            Projectile.hide = false;
            Projectile.timeLeft = 2;
            Projectile.ai[0]++;

            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center;
            player.itemAnimation = useTime;
            player.itemTime = useTime;
            player.phantasmTime = useTime;
            player.heldProj = Projectile.whoAmI;

            if (player.whoAmI == Main.myPlayer)
            {
                Projectile.netUpdate = true; //for mp sync
                Projectile.velocity = player.DirectionTo(Main.MouseWorld) * Projectile.velocity.Length();

                if (player.altFunctionUse != 2) //released right click or switched to left click
                    Projectile.Kill();
            }

            player.direction = Projectile.velocity.X > 0 ? 1 : -1;
            player.itemRotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);

            if (++Projectile.localAI[0] > useTime / 2) //charging up dusts
            {
                Projectile.localAI[0] = 0;
                const int maxDust = 36;
                for (int i = 0; i < maxDust; i++)
                {
                    Vector2 spawnPos = player.Center;
                    spawnPos += 50f * Vector2.Normalize(Projectile.velocity).RotatedBy((i - (maxDust / 2 - 1)) * 6.28318548f / maxDust);
                    Vector2 speed = player.Center - spawnPos;
                    int num228 = Dust.NewDust(spawnPos, 0, 0, 15, 0f, 0f, 100, default(Color), 2f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].velocity = speed * .1f;
                }
            }

            //dust!
            /*int dustId = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 2f), Projectile.width / 2, Projectile.height + 5, 15, Projectile.velocity.X * 0.2f,
                Projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[dustId].noGravity = true;
            int dustId3 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 2f), Projectile.width / 2, Projectile.height + 5, 15, Projectile.velocity.X * 0.2f,
                Projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[dustId3].noGravity = true;

            if (--Projectile.localAI[0] < 0)
            {
                Projectile.localAI[0] = 3;
                if (Projectile.owner == Main.myPlayer)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PhantasmalSphere>(), Projectile.damage, Projectile.knockBack / 2, Projectile.owner);
            }

            if (Projectile.velocity != Vector2.Zero)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);*/
        }

        public override void Kill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                int damage = (int)(Projectile.damage * (1f + Projectile.ai[0] / useTime));
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<HentaiSpearThrown>(), damage, Projectile.knockBack, Projectile.owner);
            }
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            /*Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 2)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
            }*/

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}