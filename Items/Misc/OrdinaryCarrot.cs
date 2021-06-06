using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Misc
{
    public class OrdinaryCarrot : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ordinary Carrot");
            Tooltip.SetDefault(@"Increases night vision
Minor improvements to all stats
1 minute duration
Right click to increase view range while in inventory
'Plucked from the face of a defeated foe'");
            DisplayName.AddTranslation(GameCulture.Chinese, "普通的胡萝卜");
            Tooltip.AddTranslation(GameCulture.Chinese,@"增强夜视效果
所有属性小幅度增加
1分钟持续时间
在背包中时，按住右键以缩放视域
'从一位被打败的敌人的脸上拔下来的'");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 30;
            item.rare = ItemRarityID.LightRed;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 17;
            item.useTime = 17;
            item.consumable = true;
            item.UseSound = SoundID.Item2;
            item.value = Item.sellPrice(0, 0, 10, 0);
        }

        public override void UpdateInventory(Player player)
        {
            if (player.GetToggleValue("MasoCarrot", false))
                player.scope = true;
        }

        public override bool UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.AddBuff(BuffID.NightOwl, 3600);
                player.AddBuff(BuffID.WellFed, 3600);
            }
            return true;
        }
    }
}
