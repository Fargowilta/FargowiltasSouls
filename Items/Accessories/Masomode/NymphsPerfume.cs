using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class NymphsPerfume : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nymph's Perfume");
            Tooltip.SetDefault(@"Grants immunity to Lovestruck, Stinky, and Hexed
You respawn with more life
Your attacks occasionally produce hearts
'The scent is somewhat overpowering'");
            DisplayName.AddTranslation(GameCulture.Chinese, "染血女神的香水");
            Tooltip.AddTranslation(GameCulture.Chinese, @"免疫热恋和恶臭
攻击偶尔会生成心
'气味有点太浓了'");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Pink;
            item.value = Item.sellPrice(0, 4);
        }

        public override void UpdateInventory(Player player)
        {
            FargoPlayer fargoPlayer = player.GetModPlayer<FargoPlayer>();
            fargoPlayer.NymphsPerfumeRespawn = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Lovestruck] = true;
            player.buffImmune[mod.BuffType("Lovestruck")] = true;
            player.buffImmune[mod.BuffType("Hexed")] = true;
            player.buffImmune[BuffID.Stinky] = true;
            FargoPlayer fargoPlayer = player.GetModPlayer<FargoPlayer>();
            fargoPlayer.NymphsPerfume = true;
            fargoPlayer.NymphsPerfumeRespawn = true;
            if (fargoPlayer.NymphsPerfumeCD > 0)
                fargoPlayer.NymphsPerfumeCD--;
        }
    }
}
