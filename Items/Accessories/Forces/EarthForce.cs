using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Forces
{
    public class EarthForce : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Force of Earth");

            Tooltip.SetDefault(
@"25% chance for your projectiles to explode into shards
20% increased weapon use speed
Greatly increases life regeneration after striking an enemy
You spawn an orb of damaging life energy every 80 life regenerated
Flower petals will cause extra damage to your target
Damaging debuffs deal 5x damage
One of your projectiles will split into 3 every 3/4 of a second
Briefly become invulnerable after striking an enemy
'Gaia's blessing shines upon you'");

            DisplayName.AddTranslation(GameCulture.Chinese, "大地之力");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"你的弹幕有25%几率爆裂成碎片
武器使用速度增加20%
击中敌人后大幅增加你的生命恢复速度
你每恢复80点生命值便会生成一个伤害性的生命能量球
花瓣将落到你攻击的敌人的身上，造成额外伤害
伤害性减益造成的伤害x5
每3/4秒会随机使你的一个弹幕分裂成三个
攻击敌人后会使你无敌一小段时间
“盖亚的祝福照耀着你”");
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
