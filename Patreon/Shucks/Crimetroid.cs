using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.Shucks
{
    public class Crimetroid : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
        }

        public override bool MinionContactDamage() => true;

        int lifestealCD;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            PatreonPlayer modPlayer = player.GetModPlayer<PatreonPlayer>();

            if (!player.active || player.dead || player.ghost)
            {
                modPlayer.Crimetroid = false;
            }

            if (modPlayer.Crimetroid)
            {
                Projectile.timeLeft = 2;
            }

            if (--lifestealCD < 0)
                lifestealCD = 0;

            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }

            float dist = Projectile.Distance(player.Top);
            if (dist > 400)
                Projectile.tileCollide = false;
            if (dist > 2000)
                Projectile.Center = player.Center;

            Vector2 focus = player.Top;
            if (dist < 60)
            {
                Projectile.localAI[0] = 0;

                focus = Projectile.Center + Projectile.velocity;
                if (focus == Projectile.Center)
                    focus.Y -= 1;
                if (!Projectile.tileCollide)
                    Projectile.tileCollide = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, player.position, player.width, player.height);
            }
            else if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = MathHelper.ToRadians(Main.rand.NextFloat(45));
                if (Main.rand.NextBool())
                    Projectile.localAI[0] *= -1;
            }

            Projectile.localAI[0] *= 0.99f;

            float maxSpeed = 8f;
            if (Projectile.localAI[1] > 0)
            {
                Projectile.localAI[1]--;
                maxSpeed *= 0.5f;
            }
            if (!player.velocity.HasNaNs() && player.velocity != Vector2.Zero)
                maxSpeed += player.velocity.Length() / 2;

            Vector2 change = Projectile.DirectionTo(focus) * maxSpeed;
            if (Projectile.localAI[0] != 0)
            {
                change = change.RotatedBy(Projectile.localAI[0]);
            }
            const float increment = 22;
            Projectile.velocity = (Projectile.velocity * (increment - 1) + change) / increment;

            Projectile.rotation = Projectile.velocity.X * .05f;
            if (Math.Abs(Projectile.rotation) > MathHelper.ToRadians(75))
                Projectile.rotation = MathHelper.ToRadians(75) * Math.Sign(Projectile.rotation);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = true;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.localAI[1] = 15;
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X * 0.75f;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y * 0.75f;
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.owner == Main.myPlayer && lifestealCD == 0)
            {
                lifestealCD = 60;
                Projectile.vampireHeal(1, Projectile.Center, target);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, effects, 0);


            Texture2D texture2D14 = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Patreon/Shucks/CrimetroidJelly", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            const float jellyOpacity = 0.5f;
            Main.EntitySpriteDraw(texture2D14, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26 * jellyOpacity, Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}