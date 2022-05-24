using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class EarthChainBlast : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_687";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chain Blast");
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.LunarFlare];
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.aiStyle = -1;
            //AIType = ProjectileID.LunarFlare;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            //Projectile.extraUpdates = 5;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.alpha = 0;

            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 1;
        }

        public override bool? CanDamage()
        {
            return Projectile.frame == 3 || Projectile.frame == 4;
        }

        public override void AI()
        {
            if (Projectile.position.HasNaNs())
            {
                Projectile.Kill();
                return;
            }
            /*Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 229, 0f, 0f, 0, new Color(), 1f)];
            dust.position = Projectile.Center;
            dust.velocity = Vector2.Zero;
            dust.noGravity = true;
            dust.noLight = true;*/

            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame--;
                    Projectile.Kill();
                    return;
                }
                if (Projectile.frame == 3)
                    Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().GrazeCD = 0;
            }
            //if (++Projectile.ai[0] > Main.projFrames[Projectile.type] * 3) Projectile.Kill();

            if (++Projectile.localAI[1] == 8 && Projectile.ai[1] > 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.ai[1]--;

                Vector2 baseDirection = Projectile.ai[0].ToRotationVector2();
                float random = MathHelper.ToRadians(15);
                if (Projectile.ai[1] > 2)
                {
                    for (int i = -1; i <= 1; i++) //split into more explosions
                    {
                        if (i == 0)
                            continue;
                        Vector2 offset = Projectile.width * 1.25f * baseDirection.RotatedBy(MathHelper.ToRadians(60) * i + Main.rand.NextFloat(-random, random));
                        Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center + offset, Vector2.Zero, Projectile.type,
                            Projectile.damage, 0f, Projectile.owner, Projectile.ai[0], Projectile.ai[1]);
                    }
                }
                else
                {
                    Vector2 offset = Projectile.width * 2.25f * baseDirection.RotatedBy(Main.rand.NextFloat(-random, random));
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center + offset, Vector2.Zero, Projectile.type,
                        Projectile.damage, 0f, Projectile.owner, Projectile.ai[0], Projectile.ai[1]);
                }
            }

            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                SoundEngine.PlaySound(SoundID.Item88, Projectile.Center);

                Projectile.position = Projectile.Center;
                Projectile.scale = Main.rand.NextFloat(1f, 3f);
                Projectile.width = (int)(Projectile.width * Projectile.scale);
                Projectile.height = (int)(Projectile.height * Projectile.scale);
                Projectile.Center = Projectile.position;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(BuffID.Burning, 300);
                target.AddBuff(ModContent.BuffType<Lethargic>(), 300);
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 2; ++i)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 31, 0f, 0f, 100, new Color(), 1.5f);
            /*for (int i = 0; i < 4; ++i)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 229, 0f, 0f, 0, new Color(), 2.5f);
                Main.dust[d].velocity *= 3f;
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
                d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 229, 0f, 0f, 100, new Color(), 1.5f);
                Main.dust[d].velocity *= 2f;
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
            }*/
            if (Main.rand.NextBool(8))
            {
                int i2 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position + new Vector2(Projectile.width * Main.rand.Next(100) / 100f, Projectile.height * Main.rand.Next(100) / 100f) - Vector2.One * 10f, new Vector2(), Main.rand.Next(61, 64), 1f);
                Main.gore[i2].velocity *= 0.3f;
                Main.gore[i2].velocity.X += Main.rand.Next(-10, 11) * 0.05f;
                Main.gore[i2].velocity.Y += Main.rand.Next(-10, 11) * 0.05f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Color color = Color.White;
            if (Projectile.ai[1] > 3)
                color = Color.Lerp(new Color(255, 255, 255, 0), new Color(255, 95, 46, 50), (7 - Projectile.ai[1]) / 4);

            else
                color = Color.Lerp(new Color(255, 95, 46, 50), new Color(150, 35, 0, 100), (3 - Projectile.ai[1]) / 3);

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color,
                Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}

