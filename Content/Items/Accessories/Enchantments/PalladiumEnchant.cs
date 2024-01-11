using FargowiltasSouls.Content.Projectiles.Souls;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using System;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class PalladiumEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Palladium Enchantment");
            /* Tooltip.SetDefault(
@"Briefly increases life regeneration after striking an enemy
You spawn an orb of damaging life energy every 80 life regenerated
'You feel your wounds slowly healing'"); */
        }

        protected override Color nameColor => new(245, 172, 40);
        

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<PalladiumEffect>(Item);
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

    public class PalladiumEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<EarthHeader>();
        public override bool HasToggle => true;

        public override void PostUpdateEquips(Player player)
        {
            PalladiumFields fields = player.GetEffectFields<PalladiumFields>();
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            int increment = player.statLife - modPlayer.StatLifePrevious;
            if (increment > 0)
            {
                fields.PalladCounter += increment;
                if (fields.PalladCounter > 80)
                {
                    fields.PalladCounter = 0;
                    if (player.whoAmI == Main.myPlayer && player.statLife < player.statLifeMax2 && player.GetToggleValue(this))
                    {
                        int damage = player.ForceEffect<PalladiumEffect>() ? 100 : 50;
                        Projectile.NewProjectile(player.GetSource_Accessory(player.EffectItem<PalladiumEffect>()), player.Center, -Vector2.UnitY, ModContent.ProjectileType<PalladOrb>(),
                            FargoSoulsUtil.HighestDamageTypeScaling(player, damage), 10f, player.whoAmI, -1);
                    }
                }
            }

            if (player.ForceEffect<PalladiumEffect>() || modPlayer.TerrariaSoul)
                player.onHitRegen = true;
        }

        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            if (!player.onHitRegen)
            {
                player.AddBuff(BuffID.RapidHealing, Math.Min(300, hitInfo.Damage / 3)); //heal time based on damage dealt, capped at 5sec
            }
        }
    }

    public class PalladiumFields : EffectFields
    {
        public int PalladCounter;
    }
}
