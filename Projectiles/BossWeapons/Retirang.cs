using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class Retirang : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Retirang");
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
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override bool PreAI()
        {
            if (Projectile.ai[0] == 1)
            {
                Projectile.ai[1]++;

                //stay in place
                Projectile.position = Projectile.oldPosition;
                Projectile.velocity = Vector2.Zero;
                Projectile.rotation += Projectile.direction * -0.4f;

                //fire lasers at cursor
                if (Projectile.ai[1] % 5 == 0)
                {
                    Vector2 cursor = Main.MouseWorld;
                    Vector2 velocity = Vector2.Normalize(cursor - Projectile.Center) * 20;
                    Player player = Main.player[Projectile.owner];

                    if (Projectile.owner == Main.myPlayer)
                    {
                        SoundEngine.PlaySound(SoundID.Item12, Projectile.Center);
                        int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<PrimeLaser>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                        if (p != Main.maxProjectiles)
                        {
                            Main.projectile[p].DamageType = DamageClass.Melee;
                        }
                    }
                }

                if (Projectile.ai[1] > 15)
                {
                    Projectile.ai[0] = 2;
                }

                return false;
            }

            return true;
        }

        public override void AI()
        {
            //travelling out
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[1]++;

                if (Projectile.ai[1] > 30)
                {
                    Projectile.ai[0] = 1;
                    Projectile.ai[1] = 0;
                    Projectile.netUpdate = true;
                }
            }
            //travel back to player
            else if (Projectile.ai[0] == 2)
            {
                Projectile.extraUpdates = 0;
                Projectile.velocity = Vector2.Normalize(Main.player[Projectile.owner].Center - Projectile.Center) * 15;

                //kill when back to player
                if (Projectile.Distance(Main.player[Projectile.owner].Center) <= 30)
                    Projectile.Kill();
            }

            //spin
            Projectile.rotation += Projectile.direction * -0.4f;

            //dust!
            int dustId = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 2f), Projectile.width, Projectile.height + 5, 60, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[dustId].noGravity = true;

            if (Projectile.ai[0] == 1)
            {
                Projectile.localAI[0] += 0.1f;
                Projectile.position += Projectile.DirectionTo(Main.player[Projectile.owner].Center) * Projectile.localAI[0];

                if (Projectile.Distance(Main.player[Projectile.owner].Center) <= Projectile.localAI[0])
                    Projectile.Kill();
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[0] = 1;
                Projectile.ai[1] = 0;
            }
            Projectile.tileCollide = false;

            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            /*if (!hitSomething)
            {
                hitSomething = true;
                if (Projectile.owner == Main.myPlayer)
                {
                    for (int k = 0; k < Main.maxNPCs; k++)
                    {
                        if (k == target.whoAmI)
                            continue;

                        NPC npc = Main.npc[k];
                        float distance = Vector2.Distance(npc.Center, Projectile.Center);

                        if ((distance < 500) && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                        {
                            Vector2 velocity = (npc.Center - Projectile.Center) * 20;

                            int p = Projectile.NewProjectile(Projectile.Center, velocity, ProjectileID.PurpleLaser, Projectile.damage, 0, Projectile.owner);
                            if (p != Main.maxProjectiles)
                            {
                                Main.Projectile[p].melee = true;
                                Main.Projectile[p].magic = false;
                            }

                            break;
                        }
                    }
                }
            }*/
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
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
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