using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Forces
{
    public class NatureForce : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Force of Nature");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "自然之力");
           
            string tooltip =
@"After taking a hit, regen is greatly increased until the hit is healed off
If you take another hit before it's healed, you lose the heal in addition to normal damage
Nearby enemies are ignited
The closer they are to you the more damage they take
When you are hurt, you violently explode to damage nearby enemies
Grants immunity to Wet
Spawns a miniature storm to follow you around
Icicles will start to appear around you
You have a small area around you that will slow projectiles to 1/2 speed
Summons a ring of leaf crystals to shoot at nearby enemies
Grants a double spore jump
Not moving puts you in stealth
While in stealth, all attacks gain trails of mushrooms
'Tapped into every secret of the wilds'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"在你受到伤害后大幅增加你的生命恢复速度，直至你恢复的生命量等同于这次受到的伤害量的一半
如果你在完成恢复前再次受伤，则不会触发增加生命恢复的效果
引燃你附近的敌人
敌人离你越近，受到的伤害越高
你受到伤害时会剧烈爆炸并对附近的敌人造成伤害
使你免疫潮湿减益
召唤一个微型风暴跟着你
你的周围会出现冰锥
一个可以将弹幕速度减半的光环环绕着你
召唤一圈叶状水晶射向附近的敌人
使你获得孢子二段跳能力
站定不动时使你进入隐身状态
处于隐身状态时，攻击会产生蘑菇尾迹
“开发了荒野的每一个秘密”";
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
