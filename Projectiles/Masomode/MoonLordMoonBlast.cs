using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class MoonLordMoonBlast : MoonLordSunBlast
    {
        public override string Texture => "Terraria/Projectile_645";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            //DisplayName.SetDefault("Moon Blast");
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Masomode.CurseoftheMoon>(), 360);
        }
    }
}

