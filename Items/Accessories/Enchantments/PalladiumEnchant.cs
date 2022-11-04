using FargowiltasSouls.Projectiles.Souls;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class PalladiumEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Palladium Enchantment");
            Tooltip.SetDefault(
@"Briefly increases life regeneration after striking an enemy
You spawn an orb of damaging life energy every 80 life regenerated
'You feel your wounds slowly healing'");
        }

        protected override Color nameColor => new Color(245, 172, 40);
        public override string wizardEffect => "Rapid Healing always lasts 5 seconds, increases orb damage";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            PalladiumEffect(player, Item);
        }

        public static void PalladiumEffect(Player player, Item item)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            //no lifesteal needed here for SoE
            if (modPlayer.Eternity) return;

            if (player.GetToggleValue("Palladium"))
            {
                if (modPlayer.EarthForce || modPlayer.TerrariaSoul)
                    player.onHitRegen = true;
                modPlayer.PalladEnchantItem = item;

                /*if (palladiumCD > 0)
                    palladiumCD--;*/
            }
        }

        public static void PalladiumUpdate(FargoSoulsPlayer modPlayer)
        {
            Player player = modPlayer.Player;
            int increment = player.statLife - modPlayer.StatLifePrevious;
            if (increment > 0)
            {
                modPlayer.PalladCounter += increment;
                if (modPlayer.PalladCounter > 80)
                {
                    modPlayer.PalladCounter = 0;
                    if (player.whoAmI == Main.myPlayer && player.statLife < player.statLifeMax2 && player.GetToggleValue("PalladiumOrb"))
                    {
                        int damage = modPlayer.EarthForce ? 100 : 50;
                        Projectile.NewProjectile(player.GetSource_Accessory(modPlayer.PalladEnchantItem), player.Center, -Vector2.UnitY, ModContent.ProjectileType<PalladOrb>(),
                            FargoSoulsUtil.HighestDamageTypeScaling(player, damage), 10f, player.whoAmI, -1);
                    }
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("FargowiltasSouls:AnyPallaHead")
            .AddIngredient(ItemID.PalladiumBreastplate)
            .AddIngredient(ItemID.PalladiumLeggings)
            .AddIngredient(ItemID.BatBat)
            .AddIngredient(ItemID.SoulDrain)
            .AddIngredient(ItemID.UndergroundReward)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
