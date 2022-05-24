using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Neck)]
    public class BetsysHeart : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Betsy's Heart");
            Tooltip.SetDefault("Grants immunity to Oozed, Withered Weapon, and Withered Armor" +
                "\nYour critical strikes inflict Betsy's Curse" +
                "\nPress the Fireball Dash key to perform a short invincible dash" +
                "\n'Lightly roasted, medium rare'");

            // DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "双足翼龙之心");
            // Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "使你免疫分泌物、枯萎武器和枯萎盔甲减益" +
            //     "\n攻击造成暴击时造成双足翼龙诅咒减益" +
            //     "\n按下'火球冲刺'键后会进行短距离无敌冲刺" +
            //     "\n'微烤，五分熟'");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(gold: 7);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.OgreSpit] = true;
            player.buffImmune[BuffID.WitheredWeapon] = true;
            player.buffImmune[BuffID.WitheredArmor] = true;
            player.GetModPlayer<FargoSoulsPlayer>().BetsysHeartItem = Item;
        }
    }
}
