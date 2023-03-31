using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class DungeonGuardianNecro : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_197";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dungeon Guardian");
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = 0;
            AIType = ProjectileID.Bullet;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1000;
        }

        public override void AI()
        {
            Projectile.rotation += 0.2f;

            const int aislotHomingCooldown = 0;
            int homingDelay = (int)Projectile.ai[1];
            const float desiredFlySpeedInPixelsPerFrame = 45;
            const float amountOfFramesToLerpBy = 15; // minimum of 1, please keep in full numbers even though it's  float!

            Projectile.ai[aislotHomingCooldown]++;
            if (Projectile.ai[aislotHomingCooldown] > homingDelay)
            {
                Projectile.ai[aislotHomingCooldown] = homingDelay; //cap this value 

                NPC n = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 1000, prioritizeBoss: true));
                if (n != null)
                {
                    Vector2 desiredVelocity = Projectile.DirectionTo(n.Center) * desiredFlySpeedInPixelsPerFrame;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / amountOfFramesToLerpBy);
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            crit = Main.player[Projectile.owner].GetModPlayer<FargoSoulsPlayer>().TerrariaSoul;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 50; i++)
            {
                Vector2 pos = new Vector2(Projectile.Center.X + Main.rand.Next(-20, 20), Projectile.Center.Y + Main.rand.Next(-20, 20));
                int dust = Dust.NewDust(pos, Projectile.width, Projectile.height, DustID.Blood, 0, 0, 100, default(Color), 2f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}

