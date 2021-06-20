using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    public class UniverseSoul : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul of the Universe");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "寰宇之魂");
            
            string tooltip =
@"66% increased all damage
50% increased use speed for all weapons
50% increased shoot speed
25% increased all critical chance
Crits deal 5x damage
All weapons have double knockback
Increases your maximum mana by 300
Increases your max number of minions by 8
Increases your max number of sentries by 4
All attacks inflict Flames of the Universe
Effects of the Fire Gauntlet, Yoyo Bag, and Celestial Shell
Effects of Sniper Scope, Celestial Cuffs and Mana Flower
'The heavens themselves bow to you'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"增加66%伤害
增加50%武器使用速度
增加50%射击速度
增加25%暴击率
暴击伤害x5
武器击退翻倍
增加300点最大法力值
增加8个最大召唤栏位
增加4个最大哨兵栏位
攻击会造成宇宙之火减益
拥有烈火手套、悠悠球袋和天界壳的效果
拥有狙击镜、 天界手铐、和魔力花的效果
“诸天也向你俯首”";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);

            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 10));

        }
        public override int NumFrames => 10;

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.value = 5000000;
            item.rare = -12;
            item.expert = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            modPlayer.AllDamageUp(.66f);
            modPlayer.AllCritUp(25);
            //use speed, velocity, debuffs, crit dmg, mana up, double knockback
            modPlayer.UniverseEffect = true;

            if (player.GetToggleValue("Universe"))
            {
                modPlayer.AttackSpeed += .5f;
            }

            player.maxMinions += 8;
            player.maxTurrets += 4;

            //accessorys
            if (player.GetToggleValue("YoyoBag"))
            {
                player.counterWeight = 556 + Main.rand.Next(6);
                player.yoyoGlove = true;
                player.yoyoString = true;
            }
            //celestial shell
            if (player.GetToggleValue("MoonCharm"))
            {
                player.wolfAcc = true;
            }

            if (player.GetToggleValue("NeptuneShell"))
            {
                player.accMerman = true;
            }
            if (hideVisual)
            {
                player.hideMerman = true;
                player.hideWolf = true;
            }

            player.lifeRegen += 2;
            player.statDefense += 4;

            if (player.GetToggleValue("Sniper"))
            {
                player.scope = true;
            }
            player.manaFlower = true;
            player.manaMagnet = true;
            player.magicCuffs = true;

            if (ModLoader.GetMod("FargowiltasSoulsDLC") != null)
            {
                Mod fargoDLC = ModLoader.GetMod("FargowiltasSoulsDLC");

                if (ModLoader.GetMod("ThoriumMod") != null)
                {
                    fargoDLC.GetItem("GuardianAngelsSoul").UpdateAccessory(player, hideVisual);
                    fargoDLC.GetItem("BardSoul").UpdateAccessory(player, hideVisual);
                }
                if (ModLoader.GetMod("CalamityMod") != null)
                {
                    fargoDLC.GetItem("RogueSoul").UpdateAccessory(player, hideVisual);
                }
                if (ModLoader.GetMod("DBZMOD") != null)
                {
                    fargoDLC.GetItem("KiSoul").UpdateAccessory(player, hideVisual);
                }
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "GladiatorsSoul");
            recipe.AddIngredient(null, "SnipersSoul");
            recipe.AddIngredient(null, "ArchWizardsSoul");
            recipe.AddIngredient(null, "ConjuristsSoul");
            //recipe.AddIngredient(null, "OlympiansSoul");

            if (ModLoader.GetMod("FargowiltasSoulsDLC") != null)
            {
                Mod fargoDLC = ModLoader.GetMod("FargowiltasSoulsDLC");

                if (ModLoader.GetMod("ThoriumMod") != null)
                {
                    recipe.AddIngredient(fargoDLC.ItemType("GuardianAngelsSoul"));
                    recipe.AddIngredient(fargoDLC.ItemType("BardSoul"));
                }
                if (ModLoader.GetMod("CalamityMod") != null)
                {
                    recipe.AddIngredient(fargoDLC.ItemType("RogueSoul"));
                }
                if (ModLoader.GetMod("DBZMOD") != null)
                {
                    recipe.AddIngredient(fargoDLC.ItemType("KiSoul"));
                }
            }

            recipe.AddIngredient(null, "MutantScale", 10);

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));

            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
