using FargowiltasSouls.Content.Projectiles.BossWeapons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.AbomBoss
{
    public class AbomBlast : PhantasmalBlast
    {
        public override string Texture => "FargowiltasSouls/Content/Bosses/AbomBoss/AbomBlast";

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 300);
            target.AddBuff(ModContent.BuffType<Buffs.Masomode.MutantNibbleBuff>(), 300);
            //target.immune[projectile.owner] = Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<BossWeapons.StyxGazer>()] > 0 ? 1 : 3;
        }
    }
}

