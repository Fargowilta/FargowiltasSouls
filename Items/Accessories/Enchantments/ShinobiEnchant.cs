using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class ShinobiEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Shinobi Infiltrator Enchantment");

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "渗透忍者魔石");

            string tooltip =
@"Dash into any walls, to teleport through them to the next opening
Allows the ability to dash
Double tap a direction
Throw a smoke bomb to teleport to it and gain the First Strike Buff
Using the Rod of Discord will also grant this buff
Greatly enhances Lightning Aura effectiveness
'Village Hidden in the Wall'";
            Tooltip.SetDefault(tooltip);

            //             string tooltip_ch =
            // @"冲进墙壁时会直接穿过去
            // 使你获得冲刺能力
            // 双击'左'或'右'键进行冲刺
            // 扔出烟雾弹后会将你传送至其落点的位置并使你获得先发制人增益
            // 使用混沌传送杖也会获得先发制人增益
            // 大幅强化闪电光环的效果
            // '藏匿于墙中的村庄'";
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);

        }

        protected override Color nameColor => new Color(147, 91, 24);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Yellow;
            Item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            //tele thru wall
            modPlayer.ShinobiEffect(hideVisual);
            //monk dash mayhem
            modPlayer.MonkEffect();
            //ninja, smoke bombs, pet
            //modPlayer.NinjaEffect(hideVisual); //destroy
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.MonkAltHead)
            .AddIngredient(ItemID.MonkAltShirt)
            .AddIngredient(ItemID.MonkAltPants)
            .AddIngredient(null, "NinjaEnchant")
            .AddIngredient(null, "MonkEnchant")
            .AddIngredient(ItemID.PsychoKnife)
            //chain guiottine
            //code 2
            //flower pow
            //stynger
            //.AddIngredient(ItemID.DD2PetGato);

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
