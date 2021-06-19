using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Forces
{
    public class ShadowForce : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadow Force");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "暗影之力");
            
            string tooltip = @"Four Shadow Orbs will orbit around you
Your attacks may inflict Darkness on enemies
Slain enemies may drop a pile of bones
All of your minions gain an extra scythe attack
Throw a smoke bomb to teleport to it and gain the First Strike Buff
Don't attack to gain a single use monk dash
Dash into any walls, to teleport through them to the next opening
Summons a Flameburst minion that will travel to your mouse after charging up
After attacking for 2 seconds you will be enveloped in flames
Switching weapons will increase the next attack's damage by 150%
Greatly enhances Flameburst and Lightning Aura effectiveness
'Dark, Darker, Yet Darker'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch = @"四颗暗影珠围绕着你旋转
攻击有几率造成黑暗减益
击杀敌人时有几率爆出一个骨堆
你的召唤物能进行额外的镰刀攻击
扔出一颗烟雾弹，将你传送至烟雾弹的位置并给予你先发制人增益
不攻击一段时间后使你获得静坐冥想增益
朝墙壁冲刺时会直接穿过去
召唤一个爆炸烈焰仆从，在充能完毕后会移动至光标位置
持续攻击两秒后你将被火焰包裹
切换武器后，下次攻击的伤害增加150%
大幅强化爆炸烈焰哨兵和闪电光环的效果
“黑暗，黑暗，更加黑暗”";
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
            //warlock, shade, plague accessory effect for all
            modPlayer.ShadowForce = true;
            //shoot from where you were meme, pet
            modPlayer.DarkArtistEffect(hideVisual);
            modPlayer.ApprenticeEffect();

            //DG meme, pet
            modPlayer.NecroEffect(hideVisual);
            //shadow orbs
            modPlayer.AncientShadowEffect();
            //darkness debuff, pets
            modPlayer.ShadowEffect(hideVisual);
            //tele thru walls, pet
            modPlayer.ShinobiEffect(hideVisual);
            //monk dash mayhem
            modPlayer.MonkEffect();
            //smoke bomb nonsense, pet
            modPlayer.NinjaEffect(hideVisual);
            //scythe doom, pets
            modPlayer.SpookyEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(null, "AncientShadowEnchant");
            recipe.AddIngredient(null, "NecroEnchant");
            recipe.AddIngredient(null, "SpookyEnchant");
            recipe.AddIngredient(null, "ShinobiEnchant");
            recipe.AddIngredient(null, "DarkArtistEnchant");

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));

            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
