using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Forces
{
    [AutoloadEquip(EquipType.Shield)]
    public class TerraForce : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terra Force");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "泰拉之力");
            
            string tooltip =
@"Attacks have a chance to shock enemies with lightning
Sets your critical strike chance to 10%
Every crit will increase it by 5% up to double your current critical strike chance
Getting hit drops your crit back down
Right Click to guard with your shield
You attract items from a larger range
150% increased sword size
Every quarter second a projectile will be doubled in size
Attacks may inflict enemies with Lead Poisoning
Grants lava mobility and immunity to fire and lava
Your attacks spawn explosions
'The land lends its strength'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =@"攻击有几率释放闪电击打敌人
将你的基础暴击率设为10%
每次暴击时都会增加5%暴击率，暴击率的最大值为你当前最大暴击率数值x2
被击中后会将暴击率恢复为正常值
右键进行盾牌格挡
扩大你的拾取范围
剑的尺寸增加150%
每过0.25秒便会使一个弹幕的尺寸翻倍
攻击有几率造成铅中毒减益
你可以在岩浆中正常移动和游泳
你的攻击会引发爆炸
“大地赐予他力量”";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Purple;
            item.value = 600000;
            //item.shieldSlot = 5;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            //lightning
            modPlayer.CopperEnchant = true;
            //crit effect improved
            modPlayer.TerraForce = true;
            //crits
            modPlayer.TinEffect();
            //lead poison
            modPlayer.LeadEnchant = true;
            //tungsten
            modPlayer.TungstenEnchant = true;
            //lava immune (obsidian)
            modPlayer.ObsidianEffect();

            if (player.GetToggleValue("IronS"))
            {
                //shield
                modPlayer.IronEffect();
            }
            //magnet
            if (player.GetToggleValue("IronM", false))
            {
                modPlayer.IronEnchant = true;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(null, "CopperEnchant");
            recipe.AddIngredient(null, "TinEnchant");
            recipe.AddIngredient(null, "IronEnchant");
            recipe.AddIngredient(null, "LeadEnchant");
            recipe.AddIngredient(null, "TungstenEnchant");
            recipe.AddIngredient(null, "ObsidianEnchant");

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));

            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
