using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Souls
{
    public class RainUmbrella : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 72;
            Projectile.height = 58;
            Projectile.timeLeft = 900;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        private bool firstTick = true;
        private int reflectHP = 200;
        private int getReflectHP(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            int hp = 200;

            if (modPlayer.ForceEffect<RainEnchant>())
            {
                hp = 400;
            }

            return hp;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (player.whoAmI == Main.myPlayer && (player.dead || !player.HasEffect<RainUmbrellaEffect>()))
            {
                Projectile.Kill();
                return;
            }
            else
            {
                Projectile.timeLeft = 2;
            }

            if (firstTick)
            {
                const int num226 = 12;
                for (int i = 0; i < num226; i++)
                {
                    Vector2 vector6 = Vector2.UnitX.RotatedBy(Projectile.rotation) * 6f;
                    vector6 = vector6.RotatedBy((i - (num226 / 2 - 1)) * 6.28318548f / num226, default) + Projectile.Center;
                    Vector2 vector7 = vector6 - Projectile.Center;
                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, DustID.Water, 0f, 0f, 0, default, 1.5f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].velocity = vector7;
                }

                reflectHP = getReflectHP(player);
                firstTick = false;
            }

            //pulsation mumbo jumbo
            Projectile.position.X = (int)Projectile.position.X;
            Projectile.position.Y = (int)Projectile.position.Y;
            float num395 = Main.mouseTextColor / 200f - 0.35f;
            num395 *= 0.2f;
            Projectile.scale = num395 + 0.95f;

            //float above player
            Projectile.position.X = player.Center.X - Projectile.width / 2;
            Projectile.position.Y = player.Center.Y - Projectile.height / 2 + player.gfxOffY - 50f;


            //reflect all projectiles coming from above ONLY
            const int focusRadius = 40;

            //rain dripping off umbrella!!
            //Main.NewText(Main.raining);

            if (Main.raining)
            {
                Tile currentTile = Framing.GetTileSafely(player.Center);
                if (currentTile.WallType == WallID.None)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        Vector2 offset = new();
                        double angle = Main.rand.NextDouble() * Math.PI + (Math.PI / 2);
                        offset.X += (float)(Math.Sin(angle) * focusRadius);
                        offset.Y += (float)(Math.Cos(angle) * focusRadius) + Projectile.height / 2 - 4;
                        Dust dust = Main.dust[Dust.NewDust(
                            Projectile.Center + offset - new Vector2(4, 4), 0, 0,
                            DustID.Water, 0, 0, 100, Scale: 0.75f)];
                        //dust.velocity = player.velocity;
                        //dust.noGravity = true;
                    }
                }
            }

            Main.projectile.Where(x => x.active && x.hostile && x.damage > 0 && Vector2.Distance(x.Center, Projectile.Center) <= focusRadius + Math.Min(x.width, x.height) / 2 && ProjectileLoader.CanDamage(x) != false && ProjectileLoader.CanHitPlayer(x, player) && FargoSoulsUtil.CanDeleteProjectile(x)).ToList().ForEach(x =>
            {
                //float angleTo = Math.Abs(Projectile.Center.AngleTo(x.Center));
                //float angleFrom = Math.Abs(Projectile.Center.AngleFrom(x.Center));
                bool above = Projectile.Center.Y > x.Center.Y;

                //Main.NewText(angleTo + " " + angleFrom + " " + above);

                if (above && x.FargoSouls().canUmbrellaReflect)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        int dustId = Dust.NewDust(new Vector2(x.position.X, x.position.Y + 2f), x.width, x.height + 5, DustID.Water, x.velocity.X * 0.2f, x.velocity.Y * 0.2f, 100);
                        Main.dust[dustId].noGravity = true;
                    }

                    // Set ownership
                    x.hostile = false;
                    x.friendly = true;
                    x.owner = player.whoAmI;

                    // Turn around
                    x.velocity *= -1f;

                    // Flip sprite
                    if (x.Center.X > player.Center.X)
                    {
                        x.direction = 1;
                        x.spriteDirection = 1;
                    }
                    else
                    {
                        x.direction = -1;
                        x.spriteDirection = -1;
                    }

                    // Don't know if this will help but here it is
                    x.netUpdate = true;

                    reflectHP -= x.damage;

                    x.damage *= 2;

                    if (modPlayer.ForceEffect<RainEnchant>())
                    {
                        x.damage *= 3;
                    }

                    if (reflectHP <= 0)
                    {
                        Projectile.Kill();
                    }
                }
                else
                {
                    x.FargoSouls().canUmbrellaReflect = false;
                }
            });
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (reflectHP < (getReflectHP(Main.player[Projectile.owner]) / 4))
            {
                return new Color(150, 0, 0, 150); // Color.Red;
            }

            return base.GetAlpha(lightColor);
        }

        public override void OnKill(int timeLeft)
        {
            const int num226 = 12;
            for (int i = 0; i < num226; i++)
            {
                Vector2 vector6 = Vector2.UnitX.RotatedBy(Projectile.rotation) * 6f;
                vector6 = vector6.RotatedBy((i - (num226 / 2 - 1)) * 6.28318548f / num226, default) + Projectile.Center;
                Vector2 vector7 = vector6 - Projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, DustID.Water, 0f, 0f, 0, default, 1.5f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].velocity = vector7;
            }

            Main.player[Projectile.owner].AddBuff(ModContent.BuffType<RainCDBuff>(), 900);
        }
    }
}