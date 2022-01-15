using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class GrowingPumpkin : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Pumpkin");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.penetrate = 1;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            projectile.tileCollide = true;
        }
        
        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.5f, 0.5f, 0.5f);

            projectile.ai[0]++;

            Player player = Main.player[projectile.owner];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            if (!modPlayer.WizardEnchant && !modPlayer.LifeForce)
            {
                projectile.velocity.Y = projectile.velocity.Y + 0.2f;
                if (projectile.velocity.Y > 16f)
                {
                    projectile.velocity.Y = 16f;
                }
            }

            //bonus damage if fully grown
            projectile.damage = modPlayer.HighestDamageTypeScaling(projectile.frame == 4 ? 50 : 15);

            if (projectile.frame != 4)
            {
                projectile.frameCounter++;
                if (projectile.frameCounter >= 60)
                {
                    projectile.frameCounter = 0;
                    projectile.frame = (projectile.frame + 1) % 5;

                    //dust
                    if (projectile.frame == 4)
                    {
                        const int max = 16;
                        for (int i = 0; i < max; i++)
                        {
                            Vector2 vector6 = Vector2.UnitY * 5f;
                            vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + projectile.Center;
                            Vector2 vector7 = vector6 - projectile.Center;
                            int d = Dust.NewDust(vector6 + vector7, 0, 0, DustID.Grass, 0f, 0f, 0, default(Color), 1.5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity = vector7;
                        }
                    }
                }
            }
            else
            {
                if (player.Hitbox.Intersects(projectile.Hitbox))
                {
                    int heal = 25;

                    if (modPlayer.LifeForce || modPlayer.WizardEnchant)
                    {
                        heal *= 2;
                    }

                    player.GetModPlayer<FargoPlayer>().HealPlayer(heal);
                    Main.PlaySound(SoundID.Item2, player.Center);
                    projectile.Kill();
                }
            }

            projectile.width = (int)(32 * projectile.scale);
            projectile.height = (int)(32 * projectile.scale); //make it not float when shrinking

            if (projectile.ai[0] > 1800) //make projectile shrink and disappear after 30 seconds instead of lasting forever
                projectile.scale -= 0.01f;

            if(projectile.scale <= 0)
                projectile.Kill();
        }

        private void SpawnFire(FargoPlayer modPlayer)
        {
            int damage = 50;

            if (modPlayer.LifeForce || modPlayer.WizardEnchant)
            {
                damage *= 2;
            }

            //leave some fire behind
            Projectile[] fires = FargoSoulsUtil.XWay(5, projectile.Center, ModContent.ProjectileType<PumpkinFlame>(), 3, modPlayer.HighestDamageTypeScaling(damage), 0);
            Main.PlaySound(SoundID.Item74, projectile.Center);

        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = false;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.position += projectile.velocity;
            projectile.velocity = Vector2.Zero;
            return false;
        }

        public override void Kill(int timeLeft)
        {
            //dont do fire explosion on death if it dies from scale thing or isnt full grown
            if (projectile.scale <= 0 || projectile.frame != 4)
                return;

            Player player = Main.player[projectile.owner];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            SpawnFire(modPlayer);
            const int max = 16;
            for (int i = 0; i < max; i++)
            {
                Vector2 vector6 = Vector2.UnitY * 5f;
                vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + projectile.Center;
                Vector2 vector7 = vector6 - projectile.Center;
                int d = Dust.NewDust(vector6 + vector7, 0, 0, DustID.Grass, 0f, 0f, 0, default(Color), 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = vector7;
            }
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

            if (projectile.ai[0] % 8 < 4)
                color26.A = 0;

            SpriteEffects effects = SpriteEffects.None;

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}