using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class CobaltEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Cobalt Enchantment");
            /* Tooltip.SetDefault(
@"Grants an explosion jump that inflicts Oiled and grants brief invulnerability
When you are hurt, you violently explode to damage nearby enemies
'I can't believe it's not Palladium'"); */
        }

        protected override Color nameColor => new(61, 164, 196);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<CobaltEffect>(Item);
            player.AddEffect<AncientCobaltEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("FargowiltasSouls:AnyCobaltHead")
            .AddIngredient(ItemID.CobaltBreastplate)
            .AddIngredient(ItemID.CobaltLeggings)
            .AddIngredient(null, "AncientCobaltEnchant")
            .AddIngredient(ItemID.ScarabBomb, 10)
            .AddIngredient(ItemID.DD2ExplosiveTrapT1Popper)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }

    public class CobaltEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<EarthHeader>();
        public override bool HasToggle => true;

        public override void OnHurt(Player player, Player.HurtInfo info)
        {
            if (player.GetToggleValue(this) && player.whoAmI == Main.myPlayer)
            {
                FargoSoulsPlayer modPlayer = player.FargoSouls();

                int baseDamage = 75;
                int multiplier = 2;
                int cap = 150;

                if (player.ForceEffect<CobaltEffect>())
                {
                    baseDamage = 150;
                    multiplier = 4;
                    cap = 400;
                }

                if (modPlayer.TerrariaSoul)
                {
                    baseDamage = 300;
                    multiplier = 5;
                    cap = 600;
                }

                int explosionDamage = baseDamage + info.Damage * multiplier;
                if (explosionDamage > cap)
                    explosionDamage = cap;

                Projectile p = FargoSoulsUtil.NewProjectileDirectSafe(player.GetSource_Accessory(player.EffectItem<CobaltEffect>()), player.Center, Vector2.Zero, ModContent.ProjectileType<Explosion>(), (int)(explosionDamage * player.ActualClassDamage(DamageClass.Melee)), 0f, Main.myPlayer);
                if (p != null)
                    p.FargoSouls().CanSplit = false;
            }
        }
    }
}
