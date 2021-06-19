using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Items.Accessories.Enchantments;

namespace FargowiltasSouls.Items.Accessories.Forces
{
    public class WillForce : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Force of Will");
           
            DisplayName.AddTranslation(GameCulture.Chinese, "意志之力");
            
            string tooltip =
@"Your attacks inflict Midas
Press the Gold hotkey to be encased in a Golden Shell
You will not be able to move or attack, but will be immune to all damage
20% chance for enemies to drop 5x loot
Spears will rain down on struck enemies
Double tap down to create a localized rain of arrows
Increases the effectiveness of healing sources by 50%
Greatly enhances Ballista and Explosive Traps effectiveness
Effects of Greedy Ring
'A mind of unbreakable determination'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"攻击会造成迈达斯减益
按下“金身”键后会将你包裹在一个黄金壳中
被包裹时你无法移动或攻击，但你免疫所有伤害
敌人死亡时有20%的几率获得五倍的战利品
长矛将倾泄在被攻击的敌人身上
双击“下”键后令箭雨倾泄在光标位置
增加50%受治疗量
大幅强化弩车和爆炸陷阱的效果
拥有贪婪戒指的效果
“坚不可摧的决心”";
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
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            //super bleed on all
            modPlayer.WillForce = true;
            //midas, greedy ring, pet, zhonyas
            modPlayer.GoldEffect(hideVisual);
            //loot multiply
            modPlayer.PlatinumEnchant = true;
            //javelins and pets
            modPlayer.GladiatorEffect(hideVisual);
            //wizard bonuses if somehow wearing only other enchants and not forces
            modPlayer.WizardEnchant = true;
            //arrow rain, celestial shell, pet
            modPlayer.RedRidingEffect(hideVisual);
            modPlayer.HuntressEffect();
            //immune frame kill, pet
            modPlayer.ValhallaEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "GoldEnchant");
            recipe.AddIngredient(null, "PlatinumEnchant");
            recipe.AddIngredient(null, "GladiatorEnchant");
            recipe.AddIngredient(ModContent.ItemType<WizardEnchant>());
            recipe.AddIngredient(null, "RedRidingEnchant");
            recipe.AddIngredient(null, "ValhallaKnightEnchant");

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));

            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
