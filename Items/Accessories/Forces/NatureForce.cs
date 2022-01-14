using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Items.Accessories.Enchantments;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Accessories.Forces
{
    public class NatureForce : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Force of Nature");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "自然之力");
           
            string tooltip =
$"[i:{ModContent.ItemType<CrimsonEnchant>()}] After taking a hit, regen is greatly increased until the hit is healed off\n" +
$"[i:{ModContent.ItemType<MoltenEnchant>()}] Nearby enemies are ignited\n" +
$"[i:{ModContent.ItemType<MoltenEnchant>()}] When you are hurt, you violently explode to damage nearby enemies\n" +
$"[i:{ModContent.ItemType<RainEnchant>()}] Spawns a miniature storm to follow you around\n" +
$"[i:{ModContent.ItemType<FrostEnchant>()}] Icicles will start to appear around you\n" +
$"[i:{ModContent.ItemType<SnowEnchant>()}] You have a small area around you that will slow projectiles to 1/2 speed\n" +
$"[i:{ModContent.ItemType<ChlorophyteEnchant>()}] Summons a ring of leaf crystals to shoot at nearby enemies\n" +
$"[i:{ModContent.ItemType<JungleEnchant>()}] Grants a double spore jump\n" +
$"[i:{ModContent.ItemType<ShroomiteEnchant>()}] Not moving puts you in stealth\n" +
$"[i:{ModContent.ItemType<ShroomiteEnchant>()}] All attacks gain trails of mushrooms\n" +
"'Tapped into every secret of the wilds'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"在你受到伤害后大幅增加你的生命恢复速度，直至你恢复的生命量等同于这次受到的伤害量
如果你在恢复前再次受伤则不会触发增加生命恢复的效果
引燃你附近的敌人
离你越近的敌人受到的伤害越高
你受到伤害时会剧烈爆炸并伤害附近的敌人
使你免疫潮湿减益
召唤一个微型风暴跟着你
你的周围会出现冰锥
一个可以将弹幕速度减半的光环环绕着你
召唤一圈叶状水晶射击附近的敌人
使你获得孢子二段跳能力
站定不动时使你进入隐身状态
处于隐身状态时攻击会留下更多蘑菇尾迹
'挖掘了荒野的每一个秘密'";
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
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            int index = 3;
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), $"[i:{ModContent.ItemType<CrimsonEnchant>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.NatureForce.Crimson")));
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), $"[i:{ModContent.ItemType<MoltenEnchant>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.NatureForce.Molten1")));
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), $"[i:{ModContent.ItemType<MoltenEnchant>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.NatureForce.Molten2")));
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), $"[i:{ModContent.ItemType<RainEnchant>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.NatureForce.Rain")));
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), $"[i:{ModContent.ItemType<FrostEnchant>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.NatureForce.Frost")));
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), $"[i:{ModContent.ItemType<SnowEnchant>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.NatureForce.Snow")));
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), $"[i:{ModContent.ItemType<ChlorophyteEnchant>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.NatureForce.Chlorophyte")));
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), $"[i:{ModContent.ItemType<JungleEnchant>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.NatureForce.Jungle")));
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), $"[i:{ModContent.ItemType<ShroomiteEnchant>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.NatureForce.Shroomite1")));
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), $"[i:{ModContent.ItemType<ShroomiteEnchant>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.NatureForce.Shroomite2")));
            tooltips.Insert(index++, new TooltipLine(Fargowiltas.Instance, "Line" + index.ToString(), Language.GetTextValue("Mods.FargowiltasSouls.NatureForce.Addition")));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            //
            modPlayer.NatureForce = true;
            //regen, pets
            modPlayer.CrimsonEffect(hideVisual);
            //inferno and explode
            modPlayer.MoltenEffect();
            //rain
            modPlayer.RainEffect();
            //icicles, pets
            modPlayer.FrostEffect(hideVisual);
            modPlayer.SnowEffect(hideVisual);
            //crystal and pet
            modPlayer.ChloroEffect(hideVisual);
            //spores
            modPlayer.JungleEnchant = true;
            //stealth, shrooms, pet
            modPlayer.ShroomiteEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(null, "CrimsonEnchant");
            recipe.AddIngredient(null, "MoltenEnchant");
            recipe.AddIngredient(null, "RainEnchant");
            recipe.AddIngredient(null, "FrostEnchant");
            recipe.AddIngredient(null, "ChlorophyteEnchant");
            recipe.AddIngredient(null, "ShroomiteEnchant");

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));

            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
