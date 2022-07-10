using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class MoonLordSunBlast : Champions.EarthChainBlast
    {
        public override string Texture => "Terraria/Images/Projectile_687";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Sun Blast");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 70;
            Projectile.height = 70;
            CooldownSlot = 1;
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

            if (Projectile.localAI[1] == 0)
            {
                SoundEngine.PlaySound(SoundID.Item88, Projectile.Center);
                Projectile.position = Projectile.Center;
                Projectile.scale = Main.rand.NextFloat(1.5f, 4f); //ensure no gaps
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                Projectile.width = (int)(Projectile.width * Projectile.scale);
                Projectile.height = (int)(Projectile.height * Projectile.scale);
                Projectile.Center = Projectile.position;
            }

            if (++Projectile.localAI[1] == 6 && Projectile.ai[1] > 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.ai[1]--;

                Vector2 baseDirection = Projectile.ai[0].ToRotationVector2();
                float random = MathHelper.ToRadians(15);

                if (Projectile.localAI[0] != 2f)
                {
                    //spawn stationary blasts
                    float stationaryPersistence = Math.Min(5, Projectile.ai[1]); //stationaries always count down from this
                    int p = Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center + Main.rand.NextVector2Circular(20, 20), Vector2.Zero, Projectile.type,
                        Projectile.damage, 0f, Projectile.owner, Projectile.ai[0], stationaryPersistence);
                    if (p != Main.maxProjectiles)
                        Main.projectile[p].localAI[0] = 1f; //only make more stationaries, don't propagate forward
                }

                //propagate forward
                if (Projectile.localAI[0] != 1f)
                {
                    //10f / 7f is to compensate for shrunken hitbox
                    float length = Projectile.width / Projectile.scale * 10f / 7f;
                    Vector2 offset = length * baseDirection.RotatedBy(Main.rand.NextFloat(-random, random));
                    int p = Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center + offset, Vector2.Zero, Projectile.type,
                          Projectile.damage, 0f, Projectile.owner, Projectile.ai[0], Projectile.ai[1]);
                    if (p != Main.maxProjectiles)
                        Main.projectile[p].localAI[0] = Projectile.localAI[0];
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Burning, 120);
            target.AddBuff(BuffID.OnFire, 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Color color = new Color(255, 255, 255, 100);
            //color = Color.Lerp(new Color(255, 95, 46, 50), new Color(150, 35, 0, 100), (4 - Projectile.ai[1]) / 4);

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color,
                Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}

