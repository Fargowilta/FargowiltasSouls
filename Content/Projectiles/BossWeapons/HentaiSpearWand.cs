using FargowiltasSouls.Content.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class HentaiSpearWand : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/BossWeapons/HentaiSpear";

        private int syncTimer;
        private Vector2 mousePos;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("The Penetrator");
            /*ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;*/
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.alpha = 0;
            Projectile.FargoSouls().CanSplit = false;
            Projectile.FargoSouls().TimeFreezeImmune = true;

            Projectile.netImportant = true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
                return true;

            float length = 200;
            float dummy = 0f;
            Vector2 offset = length / 2 * Projectile.scale * (Projectile.rotation - MathHelper.ToRadians(135f)).ToRotationVector2();
            Vector2 end = Projectile.Center - offset;
            Vector2 tip = Projectile.Center + offset;

            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), end, tip, 8f * Projectile.scale, ref dummy))
                return true;

            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(mousePos.X);
            writer.Write(mousePos.Y);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Vector2 buffer;
            buffer.X = reader.ReadSingle();
            buffer.Y = reader.ReadSingle();
            if (Projectile.owner != Main.myPlayer)
            {
                mousePos = buffer;
            }
        }

        public override void AI()
        {
            //dust!
            /*int dustId = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 15, Projectile.velocity.X * 0.2f,
                Projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[dustId].noGravity = true;
            int dustId3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 15, Projectile.velocity.X * 0.2f,
                Projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[dustId3].noGravity = true;*/

            Player player = Main.player[Projectile.owner];
            if (Projectile.owner == Main.myPlayer && Projectile.localAI[0] > 5
                && player.ownedProjectileCounts[ModContent.ProjectileType<HentaiSpearBigDeathray>()] < 1)
            {
                Projectile.Kill();
                return;
            }

            if (player.dead || player.ghost || !player.active)
            {
                Projectile.Kill();
                return;
            }

            if (player.HeldItem.type == ModContent.ItemType<Items.Weapons.FinalUpgrades.HentaiSpear>())
            {
                Projectile.damage = Main.player[Projectile.owner].GetWeaponDamage(Main.player[Projectile.owner].HeldItem);
                Projectile.CritChance = player.GetWeaponCrit(player.HeldItem);
                Projectile.knockBack = Main.player[Projectile.owner].GetWeaponKnockback(Main.player[Projectile.owner].HeldItem, Main.player[Projectile.owner].HeldItem.knockBack);
            }

            if (Projectile.localAI[0]++ == 0)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Normalize(Projectile.velocity), ModContent.ProjectileType<HentaiSpearBigDeathray>(),
                      Projectile.damage, Projectile.knockBack, player.whoAmI, 0, Projectile.identity);
                }
            }

            Vector2 ownerMountedCenter = player.RotatedRelativePoint(player.MountedCenter);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            Projectile.Center = ownerMountedCenter;
            Projectile.timeLeft = 2;

            if (Projectile.owner == Main.myPlayer)
            {
                mousePos = Main.MouseWorld;

                if (++syncTimer > 20)
                {
                    syncTimer = 0;
                    Projectile.netUpdate = true;
                }
            }

            const float lerp = 0.06f;
            Projectile.velocity = Vector2.Lerp(Vector2.Normalize(Projectile.velocity),
                Vector2.Normalize(mousePos - player.MountedCenter), lerp); //slowly move towards direction of cursor
            Projectile.velocity.Normalize();

            Projectile.position += Projectile.velocity * 164 * 1.3f / 4f; //offset by part of spear's length

            float extrarotate = Projectile.direction * player.gravDir < 0 ? MathHelper.Pi : 0;
            float itemrotate = Projectile.direction < 0 ? MathHelper.Pi : 0;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135);
            Projectile.position -= Projectile.velocity;
            player.itemRotation = Projectile.velocity.ToRotation() + itemrotate;
            player.itemRotation = MathHelper.WrapAngle(player.itemRotation);
            player.ChangeDir(Math.Sign(Projectile.velocity.X));
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
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

            /*Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                
            
            (texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
            }*/

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}