using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    public class UniverseSoul : BaseSoul
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Soul of the Universe");

            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "寰宇之魂");

            string tooltip =
@"66% increased all damage for your current weapon class
25% increased critical chance for your current weapon class
50% increased use speed for all weapons
50% increased shoot speed
Crits deal 4x instead of 2x
All weapons have double knockback
Increases your maximum mana by 300
Increases your max number of minions by 4
Increases your max number of sentries by 4
All attacks inflict Flames of the Universe
Effects of the Fire Gauntlet, Yoyo Bag, and Celestial Shell
Effects of Sniper Scope, Celestial Cuffs and Mana Flower
'The heavens themselves bow to you'";
            Tooltip.SetDefault(tooltip);

            //string tooltip_ch =
//@"增加66%伤害
//增加50%武器使用速度
//增加50%射击速度
//增加25%暴击率
//暴击伤害x5
//武器击退翻倍
//增加300点最大法力值
//+8最大召唤栏
//+4最大哨兵栏
//攻击会造成宇宙之火减益
//拥有烈火手套、悠悠球袋和天界壳效果
//拥有狙击镜、 天界手铐、和魔力花效果
//'诸天也向你俯首'";
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);

            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 10));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }
        public override int NumFrames => 10;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = 5000000;
            Item.rare = -12;
            Item.expert = true;
            Item.defense = 4;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            DamageClass damageClass = player.ProcessDamageTypeFromHeldItem();
            player.GetDamage(damageClass) += .66f;
            player.GetCritChance(damageClass) += 25;

            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            //use speed, velocity, debuffs, crit dmg, mana up, double knockback
            modPlayer.UniverseSoul = true;
            modPlayer.UniverseCore = true;

            if (player.GetToggleValue("Universe"))
                modPlayer.AttackSpeed += .5f;

            player.maxMinions += 4;
            player.maxTurrets += 4;

            if (player.GetToggleValue("MagmaStone"))
            {
                player.magmaStone = true;
            }
            player.kbGlove = true;
            player.autoReuseGlove = true;
            player.meleeScaleGlove = true;

            if (player.GetToggleValue("YoyoBag", false))
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

            if (player.GetToggleValue("Sniper"))
            {
                player.scope = true;
            }

            if (player.GetToggleValue("ManaFlower", false))
                player.manaFlower = true;
            player.manaMagnet = true;
            player.magicCuffs = true;

            if (ModLoader.TryGetMod("FargowiltasSoulsDLC", out Mod fargoDLC))
            {
                if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
                {
                    ModContent.Find<ModItem>("FargowiltasSoulsDLC", "GuardianAngelsSoul").UpdateAccessory(player, hideVisual);
                    ModContent.Find<ModItem>("FargowiltasSoulsDLC", "BardSoul").UpdateAccessory(player, hideVisual);
                }
                if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
                {
                    ModContent.Find<ModItem>("FargowiltasSoulsDLC", "RogueSoul").UpdateAccessory(player, hideVisual);
                }
                if (ModLoader.TryGetMod("DBZMOD", out Mod dbz))
                {
                    ModContent.Find<ModItem>("FargowiltasSoulsDLC", "KiSoul").UpdateAccessory(player, hideVisual);
                }
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe()
            .AddIngredient(null, "UniverseCore")
            .AddIngredient(null, "BerserkerSoul")
            .AddIngredient(null, "SnipersSoul")
            .AddIngredient(null, "ArchWizardsSoul")
            .AddIngredient(null, "ConjuristsSoul")
            .AddIngredient(null, "AbomEnergy", 10)
            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"));

            if (ModLoader.TryGetMod("FargowiltasSoulsDLC", out Mod fargoDLC))
            {
                if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
                {
                    recipe.AddIngredient(ModContent.Find<ModItem>("FargowiltasSoulsDLC", "GuardianAngelsSoul"));
                    recipe.AddIngredient(ModContent.Find<ModItem>("FargowiltasSoulsDLC", "BardSoul"));
                }
                if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
                {
                    recipe.AddIngredient(ModContent.Find<ModItem>("FargowiltasSoulsDLC", "RogueSoul"));
                }
                if (ModLoader.TryGetMod("DBZMOD", out Mod dbz))
                {
                    recipe.AddIngredient(ModContent.Find<ModItem>("FargowiltasSoulsDLC", "KiSoul"));
                }
            }

            recipe.Register();
        }
    }
}
