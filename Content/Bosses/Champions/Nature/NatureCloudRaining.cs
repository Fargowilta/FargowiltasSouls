using FargowiltasSouls.Core.Systems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Nature
{
    public class NatureCloudRaining : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_238";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Nature Cloud");
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 28;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;

            Projectile.scale = 1.5f;
            CooldownSlot = 1;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.5f, 0.75f, 1f);

            if (++Projectile.ai[0] > 8)
            {
                Projectile.ai[0] = 0;

                if (FargoSoulsUtil.HostCheck)
                {
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.position.X + 14 + Main.rand.Next(Projectile.width - 28),
                        Projectile.position.Y + Projectile.height + 4, 0f, 5f,
                        ModContent.ProjectileType<NatureRain>(), Projectile.damage, 0f, Main.myPlayer);
                }
            }

            if (++Projectile.ai[1] > 600)
            {
                Projectile.alpha += 5;
                if (Projectile.alpha > 255)
                {
                    Projectile.alpha = 255;
                    Projectile.Kill();
                }
            }

            if (++Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 5)
                    Projectile.frame = 0;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Wet, 300);
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(BuffID.Frostburn, 300);
        }
    }
}