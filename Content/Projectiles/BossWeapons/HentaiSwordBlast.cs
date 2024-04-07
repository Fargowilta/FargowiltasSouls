using FargowiltasSouls.Content.Buffs.Masomode;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class HentaiSwordBlast : Masomode.MoonLordSunBlast
    {
        public override string Texture => "Terraria/Images/Projectile_645";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 600);
            target.immune[Projectile.owner] = 1;
        }
    }
}

