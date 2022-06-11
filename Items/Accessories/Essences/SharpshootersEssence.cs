using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Essences
{
    public class SharpshootersEssence : BaseEssence
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Sharpshooter's Essence");
            Tooltip.SetDefault(
@"18% increased ranged damage
10% chance to not consume ammo
5% increased ranged critical chance
'This is only the beginning..'");
        }

        protected override Color nameColor => new Color(188, 253, 68);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Ranged) += 0.18f;
            player.GetCritChance(DamageClass.Ranged) += 5;
            player.GetModPlayer<FargoSoulsPlayer>().RangedEssence = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.PainterPaintballGun)
                .AddIngredient(ItemID.SnowballCannon)
                .AddIngredient(ItemID.RedRyder)
                .AddIngredient(ItemID.Harpoon)
                //.AddIngredient(ItemID.Musket)
                .AddIngredient(ItemID.Boomstick)
                .AddIngredient(ItemID.BeesKnees)
                .AddIngredient(ItemID.HellwingBow)
                .AddIngredient(ItemID.RangerEmblem)
                .AddIngredient(ItemID.HallowedBar, 5)

                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
