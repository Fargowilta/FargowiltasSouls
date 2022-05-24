using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class TerraLightningOrb : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_465";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Orb");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
            Projectile.scale = 0.5f;
            CooldownSlot = 1;
        }

        public override bool? CanDamage()
        {
            return Projectile.alpha == 0;
        }

        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;

            if (FargoSoulsUtil.NPCExists(Projectile.ai[0], ModContent.NPCType<NPCs.Champions.TerraChampion>()) != null)
            {
                Projectile.alpha -= 10;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;

                /*if (Projectile.Distance(Main.player[Main.npc[ai0].target].Center) > 600)
                {
                    Projectile.velocity = 3f * Projectile.DirectionTo(Main.player[Main.npc[ai0].target].Center);
                }*/
            }
            else
            {
                Projectile.alpha += 10;
                if (Projectile.alpha > 255)
                {
                    Projectile.alpha = 255;
                    Projectile.Kill();
                }
            }

            Projectile.timeLeft = 2;

            Lighting.AddLight(Projectile.Center, 0.4f, 0.85f, 0.9f);
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 3)
                    Projectile.frame = 0;
            }

            if (Main.rand.NextBool(3))
            {
                float num11 = (float)(Main.rand.NextDouble() * 1.0 - 0.5); //vanilla dust :echbegone:
                if ((double)num11 < -0.5)
                    num11 = -0.5f;
                if ((double)num11 > 0.5)
                    num11 = 0.5f;
                Vector2 vector21 = new Vector2((float)-Projectile.width * 0.2f * Projectile.scale, 0.0f).RotatedBy((double)num11 * 6.28318548202515, new Vector2()).RotatedBy((double)Projectile.velocity.ToRotation(), new Vector2());
                int index21 = Dust.NewDust(Projectile.Center - Vector2.One * 5f, 10, 10, 226, (float)(-(double)Projectile.velocity.X / 3.0), (float)(-(double)Projectile.velocity.Y / 3.0), 150, Color.Transparent, 0.7f);
                Main.dust[index21].position = Projectile.Center + vector21 * Projectile.scale;
                Main.dust[index21].velocity = Vector2.Normalize(Main.dust[index21].position - Projectile.Center) * 2f;
                Main.dust[index21].noGravity = true;
                float num1 = (float)(Main.rand.NextDouble() * 1.0 - 0.5);
                if ((double)num1 < -0.5)
                    num1 = -0.5f;
                if ((double)num1 > 0.5)
                    num1 = 0.5f;
                Vector2 vector2 = new Vector2((float)-Projectile.width * 0.6f * Projectile.scale, 0.0f).RotatedBy((double)num1 * 6.28318548202515, new Vector2()).RotatedBy((double)Projectile.velocity.ToRotation(), new Vector2());
                int index2 = Dust.NewDust(Projectile.Center - Vector2.One * 5f, 10, 10, 226, (float)(-(double)Projectile.velocity.X / 3.0), (float)(-(double)Projectile.velocity.Y / 3.0), 150, Color.Transparent, 0.7f);
                Main.dust[index2].velocity = Vector2.Zero;
                Main.dust[index2].position = Projectile.Center + vector2 * Projectile.scale;
                Main.dust[index2].noGravity = true;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<LivingWasteland>(), 600);
                target.AddBuff(ModContent.BuffType<LightningRod>(), 600);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 255f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}