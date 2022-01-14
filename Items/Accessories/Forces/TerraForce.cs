using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Toggler;
using FargowiltasSouls.Items.Accessories.Enchantments;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Accessories.Forces
{
    [AutoloadEquip(EquipType.Shield)]
    public class TerraForce : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Terra Force");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "泰拉之力");
            
            string tooltip =
$"[i:{ModContent.ItemType<CopperEnchant>()}] Attacks have a chance to spawn lightning and explosions\n" +
$"[i:{ModContent.ItemType<TinEnchant>()}] Sets your critical strike chance to 10%\n" +
$"[i:{ModContent.ItemType<TinEnchant>()}] Every crit will increase it by 5% up to double your current crit chance\n" +
$"[i:{ModContent.ItemType<IronEnchant>()}] Right Click to guard with your shield\n" +
$"[i:{ModContent.ItemType<IronEnchant>()}] You attract items from a larger range\n" +
$"[i:{ModContent.ItemType<LeadEnchant>()}] Attacks may inflict enemies with Lead Poisoning\n" +
$"[i:{ModContent.ItemType<TungstenEnchant>()}] 150% increased sword size\n" +
$"[i:{ModContent.ItemType<TungstenEnchant>()}] Every quarter second a projectile will be doubled in size\n" +
$"[i:{ModContent.ItemType<ObsidianEnchant>()}]Grants immunity to fire and lava\n" +
$"[i:{ModContent.ItemType<ObsidianEnchant>()}]While standing in lava or lava wet, your attacks spawn explosions\n" +
"'The land lends its strength'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =@"攻击有几率释放闪电击打敌人
将你的基础暴击率设为10%
每次暴击时都会增加5%暴击率，增加的暴击率的最大值为你当前最大暴击率数值x2
被击中后会降低暴击率
右键进行盾牌格挡
扩大你的拾取范围
增加150%剑的尺寸
每过1/4秒便会使一个弹幕的尺寸翻倍
攻击有几率造成铅中毒减益
使你免疫火与岩浆并获得在岩浆中的机动性
你的攻击会引发爆炸
'大地赐予它力量'";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);*/
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

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            int index = 3;
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), $"[i:{ModContent.ItemType<CopperEnchant>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.TerraForce.Copper")));
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), $"[i:{ModContent.ItemType<TinEnchant>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.TerraForce.Tin1")));
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), $"[i:{ModContent.ItemType<TinEnchant>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.TerraForce.Tin2")));
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), $"[i:{ModContent.ItemType<IronEnchant>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.TerraForce.Iron1")));
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), $"[i:{ModContent.ItemType<IronEnchant>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.TerraForce.Iron2")));
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), $"[i:{ModContent.ItemType<LeadEnchant>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.TerraForce.Lead")));
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), $"[i:{ModContent.ItemType<TungstenEnchant>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.TerraForce.Tungsten1")));
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), $"[i:{ModContent.ItemType<TungstenEnchant>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.TerraForce.Tungsten2")));
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), $"[i:{ModContent.ItemType<ObsidianEnchant>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.TerraForce.Obsidian1")));
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), $"[i:{ModContent.ItemType<ObsidianEnchant>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.TerraForce.Obsidian2")));
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), Language.GetTextValue("Mods.FargowiltasSouls.TerraForce.Addition")));
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
