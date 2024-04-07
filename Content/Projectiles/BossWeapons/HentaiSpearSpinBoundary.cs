using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class HentaiSpearSpinBoundary : HentaiSpearSpin
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/BossWeapons/HentaiSpear";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Ranged;
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
            if (Projectile.owner == Main.myPlayer && (!player.controlUseTile || player.altFunctionUse != 2 || player.controlUp && player.controlDown))
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

            if (Projectile.ai[2] == 0)
            {
                if (++Projectile.localAI[0] > 2)
                {
                    SoundEngine.PlaySound(SoundID.Item12, Projectile.Center);
                    Projectile.localAI[0] = 0;
                    Projectile.localAI[1] += 2f * (float)Math.PI / 4 / 360 * ++Projectile.ai[1] * player.direction;
                    if (Projectile.localAI[1] > (float)Math.PI)
                        Projectile.localAI[1] -= (float)Math.PI * 2;
                    if (Projectile.localAI[1] < (float)Math.PI)
                        Projectile.localAI[1] += (float)Math.PI * 2;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, new Vector2(0, -9f).RotatedBy(Projectile.localAI[1] + Math.PI / 3 * i),
                                ModContent.ProjectileType<PhantasmalEyeBoundary>(), Projectile.damage, Projectile.knockBack / 2, Projectile.owner);
                        }
                    }
                }
            }
            else
            {
                //Projectile.localAI[1] += Projectile.direction;

                if (++Projectile.localAI[0] > 5)
                {
                    SoundEngine.PlaySound(SoundID.Item84, Projectile.Center);
                    Projectile.localAI[0] = -5;
                    for (int j = -1; j <= 1; j += 2)
                    {
                        const int max = 8;
                        float rotation = 2f * (float)Math.PI / max;
                        int type = ModContent.ProjectileType<HentaiSphereOkuu>();
                        const float speed = 10;
                        int damage = Projectile.damage;
                        float offset = MathHelper.ToRadians(60) * Projectile.localAI[1] / 240;
                        for (int i = 0; i < max; i++)
                        {
                            Vector2 vel = speed * Vector2.UnitY.RotatedBy(rotation * i + offset);
                            if (Projectile.owner == Main.myPlayer)
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, type, damage, Projectile.knockBack / 2, Projectile.owner, j, speed);
                        }
                    }
                }
            }
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