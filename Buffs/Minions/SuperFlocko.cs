using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Minions
{
    public class SuperFlocko : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Super Flocko");
            //Description.SetDefault("The super Flocko will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "超级圣诞雪灵");
            //Description.AddTranslation(GameCulture.Chinese, "超级圣诞雪灵将会保护你");
        }

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "FargowiltasSouls/Buffs/PlaceholderBuff";
            return true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().SuperFlocko = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.ownedProjectileCounts[mod.ProjectileType("SuperFlocko")] < 1)
                    Projectile.NewProjectile(player.Center, new Vector2(0f, -10f), mod.ProjectileType("SuperFlocko"), 0, 4f, player.whoAmI);
            }
        }
    }
}