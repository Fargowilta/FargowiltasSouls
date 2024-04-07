using FargowiltasSouls.Content.Buffs.Masomode;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class HentaiSphereRing : Bosses.MutantBoss.MutantSphereRing
    {
        public override string Texture => "Terraria/Images/Projectile_454";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.FargoSouls().CanSplit = false;
            Projectile.FargoSouls().TimeFreezeImmune = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            base.AI();
            if (Projectile.timeLeft % Projectile.MaxUpdates == 0)
                Projectile.position += Main.player[Projectile.owner].position - Main.player[Projectile.owner].oldPosition;

            if (Projectile.owner == Main.myPlayer && Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<HentaiSpearWand>()] < 1)
            {
                Projectile.Kill();
                return;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 600);
            target.immune[Projectile.owner] = 1;
        }
    }
}