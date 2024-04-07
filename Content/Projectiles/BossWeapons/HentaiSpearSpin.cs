using FargowiltasSouls.Content.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class HentaiSpearSpin : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/BossWeapons/HentaiSpear";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("The Penetrator");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 214;
            Projectile.height = 214;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.alpha = 0;
            Projectile.timeLeft = 45;
            Projectile.FargoSouls().CanSplit = false;
            Projectile.FargoSouls().TimeFreezeImmune = true;
        }

        public override void AI()
        {
            //dust!
            int dustId = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.MagicMirror, Projectile.velocity.X * 0.2f,
                Projectile.velocity.Y * 0.2f, 100, default, 2f);
            Main.dust[dustId].noGravity = true;
            int dustId3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.MagicMirror, Projectile.velocity.X * 0.2f,
                Projectile.velocity.Y * 0.2f, 100, default, 2f);
            Main.dust[dustId3].noGravity = true;

            Player player = Main.player[Projectile.owner];
            if (Projectile.owner == Main.myPlayer && (!player.controlUseItem || player.controlUp && player.controlDown))
            {
                Projectile.Kill();
                return;
            }

            if (player.dead || !player.active)
            {
                Projectile.Kill();
                return;
            }

            Vector2 ownerMountedCenter = player.RotatedRelativePoint(player.MountedCenter);
            Projectile.direction = player.direction;
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2; //15;
            player.itemAnimation = 2; //15;
            player.reuseDelay = 10;
            //player.itemAnimationMax = 15;
            Projectile.Center = ownerMountedCenter;
            Projectile.timeLeft = 2;

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
            Projectile.rotation += (float)Math.PI / 6.85f * player.direction;
            Projectile.ai[0] += MathHelper.Pi / 45;
            Projectile.velocity = Projectile.rotation.ToRotationVector2();
            Projectile.position -= Projectile.velocity;
            player.itemRotation = Projectile.rotation;
            player.itemRotation = MathHelper.WrapAngle(player.itemRotation);

            if (++Projectile.localAI[0] > 10) //6 if set duration?
            {
                Projectile.localAI[0] = 0;
                SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
                if (Projectile.owner == Main.myPlayer && !Main.LocalPlayer.controlUseTile)
                {
                    Vector2 speed = -Vector2.UnitY.RotatedByRandom(Math.PI / 2) * Main.rand.NextFloat(9f, 12f);
                    float ai1 = Main.rand.Next(30, 60);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position + Main.rand.NextVector2Square(0f, Projectile.width),
                        speed, ModContent.ProjectileType<PhantasmalEyeHoming>(), Projectile.damage, Projectile.knockBack / 2, Projectile.owner, -1, ai1);
                }
            }

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].hostile && Main.projectile[i].damage > 0
                    && Projectile.Colliding(Projectile.Hitbox, Main.projectile[i].Hitbox)
                    && FargoSoulsUtil.CanDeleteProjectile(Main.projectile[i]))
                {
                    if (Projectile.owner == Main.myPlayer)
                    {
                        //Vector2 offset = Main.projectile[i].Center - Main.player[Projectile.owner].Center;
                        //Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<Souls.IronParry>(), 0, 0f, Main.myPlayer, offset.X, offset.Y);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Main.projectile[i].Center, Vector2.Zero, ModContent.ProjectileType<Souls.IronParry>(), 0, 0f, Main.myPlayer);
                    }

                    Main.projectile[i].hostile = false;
                    Main.projectile[i].friendly = true;
                    Main.projectile[i].owner = player.whoAmI;

                    // Turn away
                    Main.projectile[i].velocity = Main.projectile[i].DirectionFrom(player.Center) * Main.projectile[i].velocity.Length();

                    // Don't know if this will help but here it is
                    Main.projectile[i].netUpdate = true;
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            int clampedX = projHitbox.Center.X - targetHitbox.Center.X;
            int clampedY = projHitbox.Center.Y - targetHitbox.Center.Y;

            if (Math.Abs(clampedX) > targetHitbox.Width / 2)
                clampedX = targetHitbox.Width / 2 * Math.Sign(clampedX);
            if (Math.Abs(clampedY) > targetHitbox.Height / 2)
                clampedY = targetHitbox.Height / 2 * Math.Sign(clampedY);

            int dX = projHitbox.Center.X - targetHitbox.Center.X - clampedX;
            int dY = projHitbox.Center.Y - targetHitbox.Center.Y - clampedY;

            return Math.Sqrt(dX * dX + dY * dY) <= Projectile.width / 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 1; //balanceing

            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.position + new Vector2(Main.rand.Next(target.width), Main.rand.Next(target.height)),
                    Vector2.Zero, ModContent.ProjectileType<PhantasmalBlast>(), Projectile.damage, Projectile.knockBack * 3f, Projectile.owner);
            }
            target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 600);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 0.1f)
            {
                Player player = Main.player[Projectile.owner];
                Texture2D glow = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/BossWeapons/HentaiSpearSpinGlow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Color color27 = Color.Lerp(new Color(51, 255, 191, 210), Color.Transparent, (float)Math.Cos(Projectile.ai[0]) / 3 + 0.3f);
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                float scale = Projectile.scale - (float)Math.Cos(Projectile.ai[0]) / 5;
                scale *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                int max0 = Math.Max((int)i - 1, 0);
                Vector2 center = Vector2.Lerp(Projectile.oldPos[(int)i], Projectile.oldPos[max0], 1 - i % 1);
                float smoothtrail = i % 1 * (float)Math.PI / 6.85f;
                bool withinangle = Projectile.rotation > -Math.PI / 2 && Projectile.rotation < Math.PI / 2;
                if (withinangle && player.direction == 1)
                    smoothtrail *= -1;
                else if (!withinangle && player.direction == -1)
                    smoothtrail *= -1;

                center += Projectile.Size / 2;

                Vector2 offset = (Projectile.Size / 4).RotatedBy(Projectile.oldRot[(int)i] - smoothtrail * -Projectile.direction);

                Main.EntitySpriteDraw(
                    glow,
                    center - offset - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                    null,
                    color27,
                    Projectile.rotation,
                    glow.Size() / 2,
                    scale * 0.4f,
                    SpriteEffects.None,
                    0);
            }

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}