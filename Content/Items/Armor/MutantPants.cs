using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class MutantPants : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("True Mutant Pants");
            /* Tooltip.SetDefault(@"50% increased damage and 20% increased critical strike chance
40% increased movement and melee speed"); */
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "真·突变之胫");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"增加50%伤害和20%暴击率
            //增加40%移动和近战攻击速度
            //按住'上'和'跳跃'键悬停");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(0, 50);
            Item.defense = 50;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.50f;
            player.GetCritChance(DamageClass.Generic) += 20;

            player.moveSpeed += 0.4f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.4f;
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
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "MutantPants"))
            .AddIngredient(null, "AbomEnergy", 10)
            .AddIngredient(null, "EternalEnergy", 10)
            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}