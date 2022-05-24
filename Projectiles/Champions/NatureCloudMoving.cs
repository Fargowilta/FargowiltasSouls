using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class NatureCloudMoving : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_237";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nature Cloud");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = 40;
            Projectile.tileCollide = false;

            CooldownSlot = 1;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.5f, 0.75f, 1f);

            Projectile.rotation = Projectile.rotation + Projectile.velocity.X * 0.02f;
            if (++Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 3)
                    Projectile.frame = 0;
            }
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<NatureCloudRaining>(), Projectile.damage, 0f, Main.myPlayer);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Wet, 300);
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(BuffID.Frostburn, 300);
        }
    }
}