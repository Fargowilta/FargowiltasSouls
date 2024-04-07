using FargowiltasSouls.Content.Buffs.Masomode;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Minions
{
    public class PoisonSeedPlanterasChild : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_276";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Poison Seed");
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 240;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 1)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 1)
                    Projectile.frame = 0;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<InfestedBuff>(), 360);
            target.AddBuff(BuffID.Venom, 360);
            target.AddBuff(BuffID.Poisoned, 360);
        }
    }
}