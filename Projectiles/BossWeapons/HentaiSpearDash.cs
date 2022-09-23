using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class HentaiSpearDash : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/HentaiSpear";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Penetrator");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 30;
            Projectile.alpha = 0;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.extraUpdates = 1;

            Projectile.localNPCHitCooldown = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
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

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.Center = Projectile.Center;
            /*if (Projectile.timeLeft > 1) //trying to avoid wallclipping
                player.position += Projectile.velocity;*/
            player.velocity = Projectile.velocity * .5f;
            player.direction = Projectile.velocity.X > 0 ? 1 : -1;

            player.controlLeft = false;
            player.controlRight = false;
            player.controlJump = false;
            player.controlDown = false;
            player.controlUseItem = false;
            player.controlUseTile = false;
            player.controlHook = false;
            player.controlMount = false;

            player.heldProj = Projectile.whoAmI;

            if (player.mount.Active)
                player.mount.Dismount(player);

            player.immune = true;
            player.immuneTime = 2;
            player.hurtCooldowns[0] = 2;
            player.hurtCooldowns[1] = 2;

            //dust!
            int dustId = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 15, Projectile.velocity.X * 0.2f,
                Projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[dustId].noGravity = true;
            int dustId3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 15, Projectile.velocity.X * 0.2f,
                Projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[dustId3].noGravity = true;

            if (--Projectile.localAI[0] < 0)
            {
                Projectile.localAI[0] = 3;
                if (Projectile.owner == Main.myPlayer)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PhantasmalSphere>(), Projectile.damage, Projectile.knockBack / 2, Projectile.owner);
            }

            if (Projectile.velocity != Vector2.Zero)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = Player.defaultWidth;
            height = Player.defaultHeight;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity;
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.position + new Vector2(Main.rand.Next(target.width), Main.rand.Next(target.height)), Vector2.Zero, ModContent.ProjectileType<PhantasmalBlast>(), Projectile.damage, 0f, Projectile.owner);
            target.AddBuff(ModContent.BuffType<Buffs.Masomode.CurseoftheMoon>(), 600);
        }

        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 2)
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