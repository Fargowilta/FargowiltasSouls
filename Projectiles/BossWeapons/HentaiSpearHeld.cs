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
            //DisplayName.SetDefault("The Penetrator");
            /*ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;*/
        }

        public override void SetDefaults()
        {
            projectile.width = 58;
            projectile.height = 58;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.scale = 1.3f;
            projectile.alpha = 0;
            projectile.thrown = true;
            projectile.hide = true;

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;
        }

        public override void AI()
        {
            projectile.hide = false;
            projectile.timeLeft = 2;
            projectile.ai[0]++;

            Player player = Main.player[projectile.owner];
            projectile.Center = player.Center;
            player.itemAnimation = useTime;
            player.itemTime = useTime;
            player.phantasmTime = useTime;
            player.heldProj = projectile.whoAmI;

            if (player.whoAmI == Main.myPlayer)
            {
                projectile.netUpdate = true; //for mp sync
                projectile.velocity = player.DirectionTo(Main.MouseWorld) * projectile.velocity.Length();

                if (player.altFunctionUse != 2) //released right click or switched to left click
                    projectile.Kill();
            }

            player.direction = projectile.velocity.X > 0 ? 1 : -1;
            player.itemRotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);

            if (++projectile.localAI[0] > useTime / 2) //charging up dusts
            {
                projectile.localAI[0] = 0;
                const int maxDust = 36;
                for (int i = 0; i < maxDust; i++)
                {
                    Vector2 spawnPos = player.Center;
                    spawnPos += 50f * Vector2.Normalize(projectile.velocity).RotatedBy((i - (maxDust / 2 - 1)) * 6.28318548f / maxDust);
                    Vector2 speed = player.Center - spawnPos;
                    int num228 = Dust.NewDust(spawnPos, 0, 0, 15, 0f, 0f, 100, default(Color), 2f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].velocity = speed * .1f;
                }
            }

            //dust!
            /*int dustId = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 2f), projectile.width / 2, projectile.height + 5, 15, projectile.velocity.X * 0.2f,
                projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[dustId].noGravity = true;
            int dustId3 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 2f), projectile.width / 2, projectile.height + 5, 15, projectile.velocity.X * 0.2f,
                projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[dustId3].noGravity = true;

            if (--projectile.localAI[0] < 0)
            {
                projectile.localAI[0] = 3;
                if (projectile.owner == Main.myPlayer)
                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, mod.ProjectileType("PhantasmalSphere"), projectile.damage, projectile.knockBack / 2, projectile.owner);
            }

            if (projectile.velocity != Vector2.Zero)
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);*/
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
            {
                int damage = (int)(projectile.damage * (1f + projectile.ai[0] / useTime));
                Projectile.NewProjectile(projectile.Center, projectile.velocity, mod.ProjectileType("HentaiSpearThrown"), damage, projectile.knockBack, projectile.owner);
            }
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            /*Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i += 2)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, SpriteEffects.None, 0f);
            }*/

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}