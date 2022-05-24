using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Minions
{
    public class SuperFlocko : ModBuff
    {
        public override string Texture => "FargowiltasSouls/Buffs/PlaceholderBuff";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Super Flocko");
            Description.SetDefault("The super Flocko will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "超级圣诞雪灵");
            Description.AddTranslation((int)GameCulture.CultureName.Chinese, "超级圣诞雪灵将会保护你");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoSoulsPlayer>().SuperFlocko = true;
            if (player.whoAmI == Main.myPlayer)
            {
                const int damage = 45;
                if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Minions.SuperFlocko>()] < 1)
                    FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, new Vector2(0f, -10f), ModContent.ProjectileType<Projectiles.Minions.SuperFlocko>(), damage, 4f, player.whoAmI);
            }
        }
    }
}