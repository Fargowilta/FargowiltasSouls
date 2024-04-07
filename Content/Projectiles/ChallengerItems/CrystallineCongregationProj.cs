using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.ChallengerItems
{

    public class CrystallineCongregationProj : ModProjectile
    {
        private int RotDirect = 1;

        private bool home = true;

        private bool homingonMouse;

        private bool chosenDirection;

        private Player player;

        private int RealDamage;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crystal Ball");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = 0;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            AIType = 14;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1200;
            Projectile.scale = 1f;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.alpha = 255;
                Projectile.rotation = Main.rand.Next(100);
                RotDirect = Main.rand.NextBool(2) ? -1 : 1;
                player = Main.player[Projectile.owner];
                Projectile.ai[1] = 0;
                RealDamage = Projectile.damage;
            }
            Lighting.AddLight(Projectile.Center, torchID: TorchID.Purple);
            Projectile.alpha -= 17;
            Projectile.rotation += 0.2f * RotDirect;
            if (Main.rand.NextBool(10))
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleCrystalShard,
                    Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, new Color(), 1f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity.X *= 0.5f;
                Main.dust[index2].velocity.Y *= 0.5f;
            }
            Projectile.tileCollide = Projectile.alpha <= 0; //dont collide while fading in

            Projectile.velocity = Projectile.velocity * 1.008f;
            //homing ai
            if (true) //placeholder
            {
                Vector2 vectorToIdlePosition;
                float speed;
                float inertia = 5f;
                if ((player.Center - Projectile.Center).Length() < 50)
                {
                    Projectile.friendly = false;
                    Projectile.tileCollide = false;
                }
                if (player.channel && !player.noItems && !player.CCed || !((player.Center - Projectile.Center).Length() < 50)) //homing on player while mouse held
                {
                    vectorToIdlePosition = player.Center - Projectile.Center;
                    speed = 24f;
                }
                else //shoot towards mouse
                {
                    Projectile.friendly = true;
                    Projectile.tileCollide = true;
                    Projectile.penetrate = 1;
                    Projectile.maxPenetrate = 1;
                    vectorToIdlePosition = Main.MouseWorld - Projectile.Center;
                    speed = 18f;
                    SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, player.Center);
                    Projectile.timeLeft = 60;
                    homingonMouse = true;
                    home = false;
                }
                float num = vectorToIdlePosition.Length();
                if (num < 200f && homingonMouse)
                {
                    home = false;
                }
                if (num > 20f && home)
                {
                    vectorToIdlePosition.Normalize();
                    vectorToIdlePosition *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1f) + vectorToIdlePosition) / inertia;
                }
                else if (Projectile.velocity == Vector2.Zero)
                {
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
                if (!home && homingonMouse && !chosenDirection)
                {
                    double rotationrad = MathHelper.ToRadians(Main.rand.Next(-10, 10));
                    vectorToIdlePosition.Normalize();
                    vectorToIdlePosition = vectorToIdlePosition.RotatedBy(rotationrad) * speed;
                    Projectile.velocity = vectorToIdlePosition;
                    chosenDirection = true;
                }
            }
            Projectile.ai[0] += 1f;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
            for (int i = 0; i < 20; i++)
            {
                int index2 = Dust.NewDust(Projectile.position - new Vector2(Projectile.width, Projectile.height), Projectile.width * 2, Projectile.height * 2, DustID.PurpleCrystalShard,
                        Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, new Color(), 1f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity.X *= 0.5f;
                Main.dust[index2].velocity.Y *= 0.5f;
                base.OnKill(timeLeft);
            }
        }
        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 610 - Main.mouseTextColor * 2) * Projectile.Opacity;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = Color.DeepPink * Projectile.Opacity * 0.5f;
                color27.A = color26.A;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}
