using FargowiltasSouls.Core.Systems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Nature
{
    public class NatureRain : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_239";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Nature Rain");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.RainNimbus);
            AIType = ProjectileID.RainNimbus;
            CooldownSlot = 1;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.5f, 0.75f, 1f);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Wet, 300);
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(BuffID.Frostburn, 300);
        }
    }
}