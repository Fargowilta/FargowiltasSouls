using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class GelicWingSpike : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_920";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crystal Spike");
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.QueenSlimeMinionBlueSpike];
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.QueenSlimeMinionBlueSpike);
            AIType = ProjectileID.QueenSlimeMinionBlueSpike;
            Projectile.tileCollide = true;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.timeLeft = 300;

            Projectile.penetrate = 2;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.FargoSouls().noInteractionWithNPCImmunityFrames = true;

            if (ModLoader.TryGetMod("Fargowiltas", out Mod fargo))
                fargo.Call("LowRenderProj", Projectile);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.timeLeft = 0;
        }
    }
}