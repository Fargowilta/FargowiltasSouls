using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Toggler;
using FargowiltasSouls.Items.Accessories.Enchantments;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Accessories.Forces
{
    public class EarthForce : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Force of Earth");

            Tooltip.SetDefault(
$"[i:{ModContent.ItemType<CobaltEnchant>()}] 25% chance for your projectiles to explode into shards\n" +
$"[i:{ModContent.ItemType<MythrilEnchant>()}] 20% increased weapon use speed\n" +
$"[i:{ModContent.ItemType<PalladiumEnchant>()}] Greatly increases life regeneration after striking an enemy\n" +
$"[i:{ModContent.ItemType<PalladiumEnchant>()}] You spawn an orb of damaging life energy every 80 life regenerated\n" +
$"[i:{ModContent.ItemType<OrichalcumEnchant>()}] Flower petals will cause extra damage to your target\n" +
$"[i:{ModContent.ItemType<OrichalcumEnchant>()}] Damaging debuffs deal 5x damage\n" +
$"[i:{ModContent.ItemType<AdamantiteEnchant>()}] One of your projectiles will split into 3 every 3/4 of a second\n" +
$"[i:{ModContent.ItemType<TitaniumEnchant>()}] Briefly become invulnerable after striking an enemy\n" +
"'Gaia's blessing shines upon you'");

            DisplayName.AddTranslation(GameCulture.Chinese, "大地之力");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"你的弹幕有25%几率爆裂成碎片
增加20%武器使用速度
攻击敌人后大幅增加生命恢复速度
你每恢复80点生命值便会生成一个伤害性的生命能量球
花瓣将落到被你攻击的敌人的身上以造成额外伤害
伤害性减益造成的伤害x5
每过3/4秒便会随机使你的一个弹幕分裂成三个
攻击敌人后会使你无敌一小段时间
'盖亚的祝福照耀着你'");*/
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

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            int index = 3;
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), $"[i:{ModContent.ItemType<CobaltEnchant>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.EarthForce.Cobalt")));
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), $"[i:{ModContent.ItemType<MythrilEnchant>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.EarthForce.Mythril")));
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), $"[i:{ModContent.ItemType<PalladiumEnchant>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.EarthForce.Palladium1")));
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), $"[i:{ModContent.ItemType<PalladiumEnchant>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.EarthForce.Palladium2")));
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), $"[i:{ModContent.ItemType<OrichalcumEnchant>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.EarthForce.Orichalcum1")));
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), $"[i:{ModContent.ItemType<OrichalcumEnchant>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.EarthForce.Orichalcum2")));
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), $"[i:{ModContent.ItemType<AdamantiteEnchant>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.EarthForce.Adamantite")));
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), $"[i:{ModContent.ItemType<TitaniumEnchant>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.EarthForce.Titanium")));
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), Language.GetTextValue("Mods.FargowiltasSouls.EarthForce.Addition")));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            modPlayer.EarthForce = true;
            //mythril
            if (player.GetToggleValue("Mythril"))
            {
                modPlayer.MythrilEnchant = true;
                if (!modPlayer.DisruptedFocus)
                    modPlayer.AttackSpeed += .2f;
            }
            //shards
            modPlayer.CobaltEnchant = true;
            //regen on hit, heals
            modPlayer.PalladiumEffect();
            //fireballs and petals
            modPlayer.OrichalcumEffect();
            //split
            modPlayer.AdamantiteEnchant = true;
            //shadow dodge
            modPlayer.TitaniumEffect();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(null, "CobaltEnchant");
            recipe.AddIngredient(null, "PalladiumEnchant");
            recipe.AddIngredient(null, "MythrilEnchant");
            recipe.AddIngredient(null, "OrichalcumEnchant");
            recipe.AddIngredient(null, "AdamantiteEnchant");
            recipe.AddIngredient(null, "TitaniumEnchant");

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));

            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
