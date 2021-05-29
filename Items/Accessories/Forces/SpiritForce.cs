using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Forces
{
    public class SpiritForce : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Force of Spirit");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "心灵之力");
            
            string tooltip =
@"If you reach zero HP you will revive with 50 HP and spawn several bones
Collect the bones once they stop moving to heal for 15 HP each
Double tap down to call an ancient storm to the cursor location
Any projectiles shot through your storm gain 60% damage
Summons an Enchanted Sword familiar
You gain a shield that can reflect projectiles
You may continue to summon temporary minions and sentries after maxing out on your slots
Damage has a chance to spawn damaging orbs
If you crit, you might also get a healing orb
'Ascend from this mortal realm'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"受到致死伤害时会使你回复50点生命值并爆出几根骨头而非死亡
每根骨头会回复15点生命值
双击'下'键召唤远古风暴至光标位置
穿过远古风暴的弹幕会获得60%额外伤害
召唤一柄附魔剑
使你获得一面可以反弹弹幕的盾牌
在召唤栏用光后你仍可以召唤临时的哨兵和仆从
伤害敌人时有几率生成幽魂珠
攻击造成暴击时有几率生成治疗珠
'从尘世飞升'";
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
            //spectre works for all, spirit trapper works for all
            modPlayer.SpiritForce = true;
            //revive, bone zone, pet
            modPlayer.FossilEffect(hideVisual);
            //storm
            modPlayer.ForbiddenEffect();
            //sword, shield, pet
            modPlayer.HallowEffect(hideVisual);
            //infested debuff, pet
            modPlayer.TikiEffect(hideVisual);
            //pet
            modPlayer.SpectreEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(null, "FossilEnchant");
            recipe.AddIngredient(null, "ForbiddenEnchant");
            recipe.AddIngredient(null, "HallowEnchant");
            recipe.AddIngredient(null, "TikiEnchant");
            recipe.AddIngredient(null, "SpectreEnchant");

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));

            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
