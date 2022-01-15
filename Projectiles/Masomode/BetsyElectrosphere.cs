using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class BetsyElectrosphere : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_443";

        public int p = -1;
        public int timer;
        public float rotation;
        public Vector2 spawn;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Sky Dragon's Fury");
            Main.projFrames[projectile.type] = Main.projFrames[ProjectileID.Electrosphere];
        }

        public override void SetDefaults()
        {
            projectile.width = 80;
            projectile.height = 80;
            projectile.aiStyle = 77;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 240;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 1;
        }

        public override bool PreAI()
        {
            if (projectile.timeLeft == 240)
            {
                spawn.X = projectile.ai[0];
                spawn.Y = projectile.ai[1];
                rotation = projectile.velocity.ToRotation();
                timer = Main.rand.Next(60);
            }
            return true;
        }

        public override void AI()
        {
            projectile.velocity = Vector2.Zero;

            if (p == -1)
            {
                p = Player.FindClosest(projectile.Center, 0, 0);
            }
            else
            {
                if (projectile.Distance(Main.player[p].Center) < 600)
                    projectile.velocity = 2f * projectile.DirectionTo(Main.player[p].Center);

                if (++timer > 60)
                {
                    timer = 0;
                    if (Main.player[p].Distance(spawn) > projectile.Distance(spawn) && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(projectile.Center, rotation.ToRotationVector2().RotatedByRandom(MathHelper.ToRadians(30)),
                            ModContent.ProjectileType<BetsyFury2>(), projectile.damage, 0f, Main.myPlayer, p);
                    }
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            //target.AddBuff(BuffID.OnFire, 600);
            //target.AddBuff(BuffID.Ichor, 600);
            target.AddBuff(BuffID.WitheredArmor, Main.rand.Next(60, 300));
            target.AddBuff(BuffID.WitheredWeapon, Main.rand.Next(60, 300));
            target.AddBuff(BuffID.Electrified, 300);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 128) * projectile.Opacity;
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

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}