using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    [LegacyName("EaterStaff")]
    public class EaterRocketJr : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Eater Rocket");
            //ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            //ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 19;
            Projectile.height = 19;
            Projectile.scale = 1f;
            Projectile.aiStyle = -1;
            AIType = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 1200;

            //Projectile.penetrate = 99;
            //Projectile.usesLocalNPCImmunity = true;
            //Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            //dust!
            int dustId = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 2f), Projectile.width, Projectile.height + 5, DustID.CorruptionThorns, Projectile.velocity.X * 0.2f,
                Projectile.velocity.Y * 0.2f, 100, default, 1f);
            Main.dust[dustId].noGravity = true;

            Projectile.velocity.Y += 0.11f;
            Projectile.rotation += (MathHelper.TwoPi / 8f) * (Projectile.velocity.X / 40f);
        }

        bool sweetspot;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player owner = Main.player[Projectile.owner];

            int closeDistance = 300;
            int farDistance = owner.FargoSouls().RockeaterDistance;
            int midDistance = (closeDistance + farDistance) / 2;

            Vector2 middleOfSweetspot = owner.Center + owner.SafeDirectionTo(target.Center) * midDistance;
            Vector2 targetPoint = FargoSoulsUtil.ClosestPointInHitbox(target.Hitbox, middleOfSweetspot);
            float dist = Vector2.Distance(targetPoint, owner.Center);

            if (dist > closeDistance && dist < farDistance)
            {
                modifiers.FinalDamage *= 1.5f;
                sweetspot = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            //dust
            for (int num468 = 0; num468 < 20; num468++)
            {
                int num469 = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.CorruptionThorns, -Projectile.velocity.X * 0.2f,
                    -Projectile.velocity.Y * 0.2f, 100, default, 1.5f);
                Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity *= 2f;
            }

            if (sweetspot)
            {
                for (int i = 0; i < 40; i++)
                {
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptionThorns, 0f, 0f, 100, default, 2f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].noLight = true;
                    Main.dust[index2].velocity = Vector2.Normalize(Projectile.velocity) * 9f + Main.rand.NextVector2Circular(5f, 5f);
                    Main.dust[index2].velocity *= 1.25f;
                    Main.dust[index2].scale *= Main.rand.NextFloat(1.5f, 1.25f);
                }
            }

            Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);

            for (int i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, DustID.CorruptionThorns, 0f, 0f, 100, default, 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 4f;
                dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, DustID.CorruptionThorns, 0f, 0f, 100, default, 1.5f);
                Main.dust[dust].velocity *= 2f;
            }

            /*
            float scaleFactor9 = 0.5f;
            for (int j = 0; j < 4; j++)
            {
                int gore = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                    default,
                    Main.rand.Next(61, 64));

                Main.gore[gore].velocity *= scaleFactor9;
                Main.gore[gore].velocity += new Vector2(1, 1).RotatedBy(MathHelper.TwoPi / 4 * j);
            }
            */


            if (Projectile.owner == Main.myPlayer)
            {
                int max = 1;
                for (int i = 0; i < max; i++)
                {
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10)), ProjectileID.TinyEater, Projectile.damage / 2, Projectile.knockBack / 6, Main.myPlayer);
                    if (p != Main.maxProjectiles)
                        Main.projectile[p].DamageType = DamageClass.Ranged;
                }
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = TextureAssets.Projectile[Projectile.type].Value;
            int num156 = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}