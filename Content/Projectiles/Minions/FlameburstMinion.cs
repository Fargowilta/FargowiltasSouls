using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Minions
{
    public class FlameburstMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Flameburst Minion");
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 44;
            Projectile.height = 30;
            Projectile.timeLeft = 900;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.whoAmI == Main.myPlayer && (player.dead || !player.FargoSouls().DarkArtistEnchantActive || !player.HasEffect<DarkArtistMinion>()))
            {
                Projectile.Kill();
                return;
            }

            Projectile.netUpdate = true; // Please sync ech

            //pulsation mumbo jumbo
            Projectile.position.X = (int)Projectile.position.X;
            Projectile.position.Y = (int)Projectile.position.Y;
            float num395 = Main.mouseTextColor / 200f - 0.35f;
            num395 *= 0.2f;
            Projectile.scale = num395 + 0.95f;

            //charging above the player
            if (Projectile.ai[0] == 0)
            {
                //float above player
                Projectile.position.X = player.Center.X - Projectile.width / 2;
                Projectile.position.Y = player.Center.Y - Projectile.height / 2 + player.gfxOffY - 50f;

                //rotate towards and face mouse
                const float rotationModifier = 0.08f;

                if (player.whoAmI == Main.myPlayer)
                {
                    if (Main.MouseWorld.X > Projectile.Center.X)
                    {
                        Projectile.spriteDirection = 1;

                        Projectile.rotation = Projectile.rotation.AngleLerp(
                            (new Vector2(Main.MouseWorld.X, Main.MouseWorld.Y) - Projectile.Center).ToRotation(), rotationModifier);
                    }
                    else
                    {
                        Projectile.spriteDirection = -1;

                        //absolute fuckery so it faces the right direction
                        Vector2 target = new Vector2(Main.MouseWorld.X - (Main.MouseWorld.X - Projectile.Center.X) * 2, Main.MouseWorld.Y - (Main.MouseWorld.Y - Projectile.Center.Y) * 2) - Projectile.Center;

                        Projectile.rotation = Projectile.rotation.AngleLerp(target.ToRotation(), rotationModifier);
                    }
                }



                //attack as sentry 
                int attackRate = 60;
                Projectile.ai[1] += 1f;

                if (player.controlUseItem && Projectile.ai[1] >= attackRate)
                {
                    Vector2 velocity = Vector2.Normalize(Main.MouseWorld - Projectile.Center) * 10;

                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<MegaFlameburst>(), FargoSoulsUtil.HighestDamageTypeScaling(player, 85), 4, Projectile.owner, Projectile.whoAmI);
                    SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot, Projectile.Center);

                    Projectile.ai[1] = 0f;
                }
            }
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            const int num226 = 12;
            for (int i = 0; i < num226; i++)
            {
                Vector2 vector6 = Vector2.UnitX.RotatedBy(Projectile.rotation) * 6f;
                vector6 = vector6.RotatedBy((i - (num226 / 2 - 1)) * 6.28318548f / num226, default) + Projectile.Center;
                Vector2 vector7 = vector6 - Projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, DustID.FlameBurst, 0f, 0f, 0, default, 1.5f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].velocity = vector7;
            }
        }
    }
}