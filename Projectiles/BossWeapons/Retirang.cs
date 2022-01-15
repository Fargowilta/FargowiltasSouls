using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class Retirang : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Retirang");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.friendly = true;
            projectile.light = 0.4f;

            projectile.width = 50;
            projectile.height = 50;
            projectile.penetrate = -1;
            projectile.aiStyle = -1;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override bool PreAI()
        {
            if (projectile.ai[0] == 1)
            {
                projectile.ai[1]++;

                //stay in place
                projectile.position = projectile.oldPosition;
                projectile.velocity = Vector2.Zero;
                projectile.rotation += projectile.direction * -0.4f;

                //fire lasers at cursor
                if (projectile.ai[1] % 5 == 0)
                {
                    Vector2 cursor = Main.MouseWorld;
                    Vector2 velocity = Vector2.Normalize(cursor - projectile.Center) * 20;
                    Player player = Main.player[projectile.owner];

                    if (projectile.owner == Main.myPlayer)
                    {
                        Main.PlaySound(SoundID.Item12, projectile.Center);
                        int p = Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<PrimeLaser>(), projectile.damage, projectile.knockBack, projectile.owner);
                        if (p != Main.maxProjectiles)
                        {
                            Main.projectile[p].magic = false;
                            Main.projectile[p].melee = true;
                        }
                    }
                }

                if (projectile.ai[1] > 15)
                {
                    projectile.ai[0] = 2;
                }

                return false;
            }

            return true;
        }

        public override void AI()
        {
            //travelling out
            if (projectile.ai[0] == 0)
            {
                projectile.ai[1]++;

                if (projectile.ai[1] > 30)
                {
                    projectile.ai[0] = 1;
                    projectile.ai[1] = 0;
                    projectile.netUpdate = true;
                }
            }
            //travel back to player
            else if (projectile.ai[0] == 2)
            {
                projectile.extraUpdates = 0;
                projectile.velocity = Vector2.Normalize(Main.player[projectile.owner].Center - projectile.Center) * 15;

                //kill when back to player
                if (projectile.Distance(Main.player[projectile.owner].Center) <= 30)
                    projectile.Kill();
            }

            //spin
            projectile.rotation += projectile.direction * -0.4f;

            //dust!
            int dustId = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 2f), projectile.width, projectile.height + 5, 60, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[dustId].noGravity = true;

            if (projectile.ai[0] == 1)
            {
                projectile.localAI[0] += 0.1f;
                projectile.position += projectile.DirectionTo(Main.player[projectile.owner].Center) * projectile.localAI[0];

                if (projectile.Distance(Main.player[projectile.owner].Center) <= projectile.localAI[0])
                    projectile.Kill();
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.ai[0] == 0)
            {
                projectile.ai[0] = 1;
                projectile.ai[1] = 0;
            }
            projectile.tileCollide = false;

            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            /*if (!hitSomething)
            {
                hitSomething = true;
                if (projectile.owner == Main.myPlayer)
                {
                    for (int k = 0; k < Main.maxNPCs; k++)
                    {
                        if (k == target.whoAmI)
                            continue;

                        NPC npc = Main.npc[k];
                        float distance = Vector2.Distance(npc.Center, projectile.Center);

                        if ((distance < 500) && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
                        {
                            Vector2 velocity = (npc.Center - projectile.Center) * 20;

                            int p = Projectile.NewProjectile(projectile.Center, velocity, ProjectileID.PurpleLaser, projectile.damage, 0, projectile.owner);
                            if (p != Main.maxProjectiles)
                            {
                                Main.projectile[p].melee = true;
                                Main.projectile[p].magic = false;
                            }

                            break;
                        }
                    }
                }
            }*/
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            //smaller tile hitbox
            width = 22;
            height = 22;
            return true;
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

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}