using FargowiltasSouls.Content.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Souls
{
    public class FossilBone : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_21";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fossil Bone");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Bone);
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 900;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.rotation += 0.2f;

            bool slowdown = true;
            if (++Projectile.localAI[2] > 90f)
            {
                int p = player.whoAmI;
                if (p != -1 && p != Main.maxPlayers && Main.player[p].active && !Main.player[p].dead && !Main.player[p].ghost)
                {
                    if (Main.player[p].Distance(Projectile.Center) < 16 * 5)
                    {
                        slowdown = false;
                        Projectile.velocity = Projectile.SafeDirectionTo(Main.player[p].Center) * 9f;
                        Projectile.timeLeft++;

                        if (Projectile.Colliding(Projectile.Hitbox, Main.player[p].Hitbox))
                        {
                            player.FargoSouls().HealPlayer(20);
                            Projectile.Kill();
                            FargoGlobalItem.OnRetrievePickup(player);
                            return;
                        }
                    }
                }
            }

            if (slowdown)
                Projectile.velocity *= 0.95f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.position += Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.SandyBrown;
        }

        public override void OnKill(int timeLeft)
        {
            const int max = 16;
            for (int i = 0; i < max; i++)
            {
                Vector2 vector6 = Vector2.UnitY * 5f;
                vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + Projectile.Center;
                Vector2 vector7 = vector6 - Projectile.Center;
                int d = Dust.NewDust(vector6 + vector7, 0, 0, DustID.Dirt, 0f, 0f, 0, default, 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = vector7;
            }
        }
    }
}