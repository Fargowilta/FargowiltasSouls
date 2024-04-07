using FargowiltasSouls.Content.Items.Weapons.Challengers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.ChallengerItems
{
    public class RollingSnowball : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_166";

        int width;
        int height;

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.SnowBallFriendly);
            Projectile.aiStyle = -1;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;

            width = Projectile.width = 14;
            height = Projectile.height = 14;

            Projectile.netImportant = true;

            Projectile.FargoSouls().DeletionImmuneRank = 2;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Projectile.Distance(FargoSoulsUtil.ClosestPointInHitbox(targetHitbox, Projectile.Center)) < Math.Min(Projectile.width, Projectile.height) / 2;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.dead || !player.active || player.HeldItem.type != ModContent.ItemType<SnowballStaff>() || !player.channel || !player.CheckMana(player.HeldItem.mana))
            {
                Projectile.Kill();
                return;
            }

            if (player.whoAmI == Main.myPlayer)
            {
                Vector2 direction = Main.MouseWorld - player.Center;
                player.ChangeDir(Math.Sign(direction.X));

                if (player.controlUseTile)
                {
                    if (Projectile.localAI[1] <= 0)
                    {
                        Projectile.localAI[1] = 60;

                        Projectile.velocity.X = Math.Abs(Projectile.velocity.X) * Math.Sign(direction.X);

                        Dusts();
                        Projectile.Bottom = player.Bottom + Projectile.width / 2 * Vector2.UnitX * player.direction;
                        Dusts();
                        Projectile.netUpdate = true;
                    }
                }

                Projectile.localAI[1] -= 1f;

                if (Projectile.localAI[1] == 0)
                {
                    for (int i = 0; i < 30; i++)
                    {
                        int d = Dust.NewDust(player.position, player.width, player.height, DustID.GemSapphire, 0, 0, 0, default, 2f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity *= 4f;
                    }
                }
            }

            if (Projectile.velocity.X != 0)
                Projectile.spriteDirection = Projectile.direction = Math.Sign(Projectile.velocity.X);

            float roll = Projectile.velocity.X / (MathHelper.Pi * width);
            Projectile.rotation += roll;
            if (Projectile.velocity.Y == 0)
            {
                Projectile.velocity.X *= 0.98f;

                const float cap = 15f;
                Projectile.scale = Math.Min(Projectile.scale + Math.Abs(roll) * 0.2f, cap);
            }

            Projectile.velocity.Y += 0.4f;
            if (Projectile.velocity.Y > 16)
                Projectile.velocity.Y = 16;

            if (Projectile.Colliding(Projectile.Hitbox, player.Hitbox))
            {
                float x = Math.Abs(Projectile.Center.X - FargoSoulsUtil.ClosestPointInHitbox(player, Projectile.Center).X);
                float ratio = 1f - x / (Projectile.width / 2);
                float IdleAccel = 0.6f * ratio; //makes it easier to "walk" the snowball
                Projectile.velocity.X += IdleAccel * (Projectile.Center.X < player.Center.X ? -1 : 1);
                Projectile.velocity.Y += IdleAccel * (Projectile.Center.Y < player.Center.Y ? -1 : 1);
            }

            Projectile.position = Projectile.Bottom;
            Projectile.width = (int)(width * Projectile.scale);
            Projectile.height = (int)(height * Projectile.scale);
            Projectile.Bottom = Projectile.position;

            Projectile.timeLeft = 2;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            Dusts();
        }

        private void Dusts()
        {
            int max = (int)(10 * Projectile.scale);
            for (int i = 0; i < max; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SnowBlock, Scale: 1f + Projectile.scale / 10f);
                Main.dust[d].velocity *= 0.8f;
                Main.dust[d].noGravity = true;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            width = 20;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X && Math.Abs(oldVelocity.X) > 1)
            {
                Projectile.velocity.X = -oldVelocity.X * 0.5f;
                Projectile.netUpdate = true;
            }
            if (Projectile.velocity.Y != oldVelocity.Y && Math.Abs(oldVelocity.Y) > 4)
            {
                Projectile.velocity.Y = -oldVelocity.Y * 0.5f;
                Projectile.netUpdate = true;
            }
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float speedModifier = Math.Abs(Projectile.velocity.X) / 3f;
            if (speedModifier < 0.1f)
                speedModifier = 0.1f;
            if (speedModifier > 1.5f)
                speedModifier = 1.5f;
            modifiers.FinalDamage *= (float)Math.Sqrt(Projectile.scale) * speedModifier;
            modifiers.Knockback *= Math.Max(1f, Math.Abs(Projectile.velocity.X) * 1.5f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            float ratio = 1f - Math.Abs(Projectile.velocity.X) / 4f;
            if (ratio < 0)
                ratio = 0;
            target.immune[Projectile.owner] = 10 + (int)(50 * ratio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = SpriteEffects.None;

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}