using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class FrigidGemstone : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frigid Gemstone");
            Tooltip.SetDefault(@"Grants immunity to Frostburn
Your attacks summon Frostfireballs to attack your enemies
'A shard of ancient magical ice'");
            DisplayName.AddTranslation(GameCulture.Chinese, "寒玉");
            Tooltip.AddTranslation(GameCulture.Chinese, @"使你免疫霜冻减益
攻击会释放霜火球来攻击你的敌人
'一片远古魔法冰晶的碎块'");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Pink;
            item.value = Item.sellPrice(0, 4);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Frostburn] = true;
            if (player.GetToggleValue("MasoFrigid"))
            {
                FargoPlayer fargoPlayer = player.GetModPlayer<FargoPlayer>();
                fargoPlayer.FrigidGemstone = true;
                if (fargoPlayer.FrigidGemstoneCD > 0)
                    fargoPlayer.FrigidGemstoneCD--;
            }
        }
    }
}
