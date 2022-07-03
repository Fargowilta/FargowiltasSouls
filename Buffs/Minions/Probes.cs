using FargowiltasSouls.Projectiles.Minions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Minions
{
    public class Probes : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Probes");
            Description.SetDefault("The probes will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "探测器");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "探测器将会保护你");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoSoulsPlayer>().Probes = true;
            if (player.whoAmI == Main.myPlayer)
            {
                const int damage = 35;
                if (player.ownedProjectileCounts[ModContent.ProjectileType<Probe1>()] < 1)
                    FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<Probe1>(), damage, 9f, player.whoAmI);
                if (player.ownedProjectileCounts[ModContent.ProjectileType<Probe2>()] < 1)
                    FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<Probe2>(), damage, 9f, player.whoAmI, 0f, -1f);
            }
        }
    }
}