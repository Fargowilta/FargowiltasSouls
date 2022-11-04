using FargowiltasSouls.Buffs.Souls;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class CrimsonEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Crimson Enchantment");
            Tooltip.SetDefault(
@"After taking a hit, regen is greatly increased until the half the hit is healed off
If you take another hit before it's healed, the heal ends early
This does not affect hits dealing less than 10 damage
'The blood of your enemy is your rebirth'");
        }

        protected override Color nameColor => new Color(200, 54, 75);
        public override string wizardEffect => "";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Orange;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CrimsonEffect(player);
        }

        public static void CrimsonEffect(Player player)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.CrimsonEnchantActive = true;
        }

        public static void CrimsonHurt(Player player, FargoSoulsPlayer modPlayer, ref int damage)
        {
            //if was already healing, stop the heal and do nothing
            if (player.HasBuff(ModContent.BuffType<CrimsonRegen>()))
            {
                player.ClearBuff(ModContent.BuffType<CrimsonRegen>());
            }
            //start new heal
            else if(damage >= 10)
            {
                player.AddBuff(ModContent.BuffType<CrimsonRegen>(), 300);

                int totalToRegen = damage / 2;
                modPlayer.CrimsonRegenAmount = (int)((float)totalToRegen / 5f * 2f);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.CrimsonHelmet)
            .AddIngredient(ItemID.CrimsonScalemail)
            .AddIngredient(ItemID.CrimsonGreaves)
            //blood axe tging
            .AddIngredient(ItemID.TheUndertaker)
            .AddIngredient(ItemID.TheMeatball)
            .AddIngredient(ItemID.CrimsonHeart)

            //blood rain bow

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
