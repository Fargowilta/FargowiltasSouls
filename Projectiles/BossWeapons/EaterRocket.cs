using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class EaterRocket : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eater Rocket");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.Bullet;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 1200;
            Projectile.extraUpdates = 1;

            //Projectile.penetrate = 99;
            //Projectile.usesLocalNPCImmunity = true;
            //Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            //dust!
            int dustId = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 2f), Projectile.width, Projectile.height + 5, DustID.PurpleCrystalShard, Projectile.velocity.X * 0.2f,
                Projectile.velocity.Y * 0.2f, 100, default(Color), 1f);
            Main.dust[dustId].noGravity = true;

            //if (Projectile.penetrate < 99 && Projectile.ai[1] != 1)
            //{
            //    Projectile.ai[1] = 1;
            //    Projectile.timeLeft = 10;
            //}
        }

        bool sweetspot;

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Player owner = Main.player[Projectile.owner];
            Vector2 middleOfSweetspot = owner.Center + owner.DirectionTo(target.Center) * 450;
            Vector2 targetPoint = FargoSoulsUtil.ClosestPointInHitbox(target.Hitbox, middleOfSweetspot);
            float dist = Vector2.Distance(targetPoint, owner.Center);

            if (dist > 300 && dist < 600)
            {
                damage = (int)(damage * 1.5);
                sweetspot = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            //dust
            for (int num468 = 0; num468 < 20; num468++)
            {
                int num469 = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.PurpleCrystalShard, -Projectile.velocity.X * 0.2f,
                    -Projectile.velocity.Y * 0.2f, 100, default(Color), 1.5f);
                Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity *= 2f;
                num469 = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.PurpleCrystalShard, -Projectile.velocity.X * 0.2f,
                    -Projectile.velocity.Y * 0.2f, 100);
                Main.dust[num469].velocity *= 1.5f;
            }

            if (sweetspot)
            {
                for (int i = 0; i < 40; i++)
                {
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptTorch, 0f, 0f, 100, new Color(), 2f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].noLight = true;
                    Main.dust[index2].velocity = Vector2.Normalize(Projectile.velocity) * 9f + Main.rand.NextVector2Circular(12f, 12f);
                    Main.dust[index2].velocity *= 2.5f;
                    Main.dust[index2].scale *= Main.rand.NextFloat(1.5f, 3f);
                }
            }

            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, 31, 0f, 0f, 100, default(Color), 3f);
                Main.dust[dust].velocity *= 1.4f;
                Main.dust[dust].noGravity = true;
            }

            for (int i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, 6, 0f, 0f, 100, default(Color), 3.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, 6, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[dust].velocity *= 3f;
            }

            float scaleFactor9 = 0.5f;
            for (int j = 0; j < 4; j++)
            {
                int gore = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                    default(Vector2),
                    Main.rand.Next(61, 64));

                Main.gore[gore].velocity *= scaleFactor9;
                Main.gore[gore].velocity += new Vector2(1, 1).RotatedBy(MathHelper.TwoPi / 4 * j);
            }

            if (Projectile.owner == Main.myPlayer)
            {
                int max = 2;
                for (int i = 0; i < max; i++)
                {
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10)), ProjectileID.TinyEater, Projectile.damage / 6, Projectile.knockBack / 6, Main.myPlayer);
                    if (p != Main.maxProjectiles)
                        Main.projectile[p].DamageType = DamageClass.Ranged;
                }
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

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = Color.White * Projectile.Opacity * 0.75f;
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