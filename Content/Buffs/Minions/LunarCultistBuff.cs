using FargowiltasSouls.Content.Projectiles.Minions;

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Minions
{
    public class LunarCultistBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lunar Cultist");
            // Description.SetDefault("The Lunar Cultist will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "拜月教徒");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "拜月教徒将会保护你");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.FargoSouls().LunarCultist = true;
            const int damage = 80;
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<LunarCultist>()] < 1)
                FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<LunarCultist>(), damage, 2f, player.whoAmI, -1f);
        }
    }
}