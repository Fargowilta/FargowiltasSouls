using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;

namespace FargowiltasSouls.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class MutantPants : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("True Mutant Pants");
            Tooltip.SetDefault(@"50% increased damage and 20% increased critical strike chance
40% increased movement and melee speed");
            DisplayName.AddTranslation(GameCulture.Chinese, "真·突变之胫");
            Tooltip.AddTranslation(GameCulture.Chinese, @"增加50%伤害和20%暴击率
增加40%移动和近战攻击速度
按住'上'和'跳跃'键悬停");*/
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.rare = ItemRarityID.Purple;
            item.value = Item.sellPrice(0, 50);
            item.defense = 50;
        }

        public override void UpdateEquip(Player player)
        {
            const float damageUp = 0.5f;
            const int critUp = 20;
            player.meleeDamage += damageUp;
            player.rangedDamage += damageUp;
            player.magicDamage += damageUp;
            player.minionDamage += damageUp;
            player.meleeCrit += critUp;
            player.rangedCrit += critUp;
            player.magicCrit += critUp;

            player.moveSpeed += 0.4f;
            player.meleeSpeed += 0.4f;

            /*if (player.controlDown && player.controlJump && !player.mount.Active)
            {
                player.position.Y -= player.velocity.Y;
                if (player.velocity.Y > 1)
                    player.velocity.Y = 1;
                else if (player.velocity.Y < -1)
                    player.velocity.Y = -1;
            }*/
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(Main.DiscoR, 51, 255 - (int)(Main.DiscoR * 0.4));
                }
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModLoader.GetMod("Fargowiltas").ItemType("MutantPants"));
            recipe.AddIngredient(null, "AbomEnergy", 10);
            recipe.AddIngredient(null, "Sadism", 10);
            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}