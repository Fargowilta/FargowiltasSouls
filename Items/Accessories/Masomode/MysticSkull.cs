using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

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
Increases pickup range for mana stars
'The quietly muttering head of a defeated foe'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "神秘头骨");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'被打败敌人的喃喃自语的脑袋'
            // 放在物品栏中即可生效
            // 免疫窒息
            // 减少10%魔法伤害
            // 需要时自动使用魔力药水");

            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 7));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 4);
        }

        void Effects(Player player)
        {
            player.buffImmune[BuffID.Suffocation] = true;
            player.manaMagnet = true;
            if (player.GetToggleValue("ManaFlower", false))
            {
                player.GetDamage(DamageClass.Magic) -= 0.1f;
                player.manaFlower = true;
            }
        }

        public override void UpdateInventory(Player player) => Effects(player);

        public override void UpdateVanity(Player player) => Effects(player);

        public override void UpdateAccessory(Player player, bool hideVisual) => Effects(player);
    }
}