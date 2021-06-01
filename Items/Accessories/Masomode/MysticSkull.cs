using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.DataStructures;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class MysticSkull : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mystic Skull");
            Tooltip.SetDefault(@"Works in your inventory
Grants immunity to Suffocation
10% reduced magic damage
Automatically use mana potions when needed
'The quietly muttering head of a defeated foe'");
            DisplayName.AddTranslation(GameCulture.Chinese, "神秘头骨");
            Tooltip.AddTranslation(GameCulture.Chinese, @"放置在背包中即可生效
使你免疫窒息减益
降低10%魔法伤害
在需要时自动使用魔力药水
'一位被打败的敌人的低声细语的头骨'");

            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(4, 7));
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
            player.buffImmune[BuffID.Suffocation] = true;
            player.magicDamage -= 0.1f;
            player.manaFlower = true;
        }
    }
}
