using FargowiltasSouls.Buffs.Souls;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class NinjaEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Ninja Enchantment");
            Tooltip.SetDefault(
@"Drastically increases projectile speed
Reduces damage to compensate for increased speed
Increases armor pen by 10
'Attack faster than the eye can see'");
        }

        protected override Color nameColor => new Color(48, 49, 52);
        public override string wizardEffect => Language.GetTextValue("Mods.FargowiltasSouls.WizardEffect.Ninja");

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Green;
            Item.value = 30000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.NinjaEnchantItem = Item;
        }

        public static void NinjaSpeedSetup(FargoSoulsPlayer modPlayer, Projectile projectile, FargoSoulsGlobalProjectile globalProj)
        {
            Player player = modPlayer.Player;

            if (projectile.friendly /*&& FargoSoulsUtil.IsSummonDamage(projectile, true, false)*/ && player.GetToggleValue("NinjaSpeed"))
            {
                int speedIncrease = 1;

                if (modPlayer.TerraForce)
                {
                    speedIncrease = 2;
                }

                globalProj.NinjaSpeedup = projectile.extraUpdates + speedIncrease;

                if (NeedsNinjaNerf(projectile))
                {
                    int armorPen = 15;
                    if (modPlayer.TerraForce)
                        armorPen *= 3;
                    if (modPlayer.TerrariaSoul)
                        armorPen *= 5;

                    projectile.ArmorPenetration += armorPen;
                }
            }
        }

        public static bool NeedsNinjaNerf(Projectile projectile)
            => projectile.maxPenetrate == 1
            || projectile.usesLocalNPCImmunity
            || projectile.type == ProjectileID.StardustCellMinionShot;



        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.NinjaHood)
                .AddIngredient(ItemID.NinjaShirt)
                .AddIngredient(ItemID.NinjaPants)
                .AddIngredient(ItemID.Rally)
                .AddIngredient(ItemID.Shuriken, 100)
                .AddIngredient(ItemID.ThrowingKnife, 100)

                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
