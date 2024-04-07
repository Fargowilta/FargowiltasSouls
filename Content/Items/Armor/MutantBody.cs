using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class MutantBody : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("True Mutant Body");
            /* Tooltip.SetDefault(@"70% increased damage and 30% increased critical strike chance
Increases max life and mana by 200
Increases damage reduction by 30%
Drastically increases life regen"); */
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "真·突变之躯");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"增加70%伤害和30%暴击率
            //增加200最大生命和法力值
            //增加50%伤害抗性
            //极大提升生命回复");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(0, 70);
            Item.defense = 70;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.70f;
            player.GetCritChance(DamageClass.Generic) += 30;

            player.statLifeMax2 += 200;
            player.statManaMax2 += 200;

            player.endurance += 0.3f;

            player.lifeRegen += 7;
            player.lifeRegenCount += 7;
            player.lifeRegenTime += 7;
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.Mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.OverrideColor = new Color(Main.DiscoR, 51, 255 - (int)(Main.DiscoR * 0.4));
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "MutantBody"))
            .AddIngredient(null, "AbomEnergy", 15)
            .AddIngredient(null, "EternalEnergy", 15)
            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}