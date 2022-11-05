using FargowiltasSouls.Projectiles.Souls;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class RedRidingEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Red Riding Enchantment");
            Tooltip.SetDefault(
@"Attacks ignore 10 enemy defense and deal 5 flat extra damage
Each successive attack ignores an additional 10 defense and deals 5 more damage
Upon reaching 10 stacks, spawn a rain of arrows
The arrow type defaults to Venom or whatever is first in your inventory
Homing and minion attacks do not increase these bonuses
Missing any attack will reset these bonuses
'Big Bad Red Riding Hood'");
        }

        protected override Color nameColor => new Color(192, 27, 60);
        public override string wizardEffect => Language.GetTextValue("Mods.FargowiltasSouls.WizardEffect.RedRiding");

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Yellow;
            Item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            RedRidingEffect(player, Item);
            HuntressEnchant.HuntressEffect(player);
        }

        public static void RedRidingEffect(Player player, Item item)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            if (modPlayer.RedRidingEnchantItem != null || !player.GetToggleValue("RedRidingRain"))
                return;

            modPlayer.RedRidingEnchantItem = item;

            if (modPlayer.RedRidingArrowCD > 0)
            {
                modPlayer.RedRidingArrowCD--;
            }
        }

        public static void SpawnArrowRain(Player player, NPC target)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            Item firstAmmo = PickAmmo(player);
            int arrowType = firstAmmo.shoot;
            //int damage = FargoSoulsUtil.HighestDamageTypeScaling(player, (int)(firstAmmo.damage * 5f));
            int heatray = Projectile.NewProjectile(player.GetSource_Accessory(modPlayer.RedRidingEnchantItem), player.Center, new Vector2(0, -6f), ProjectileID.HeatRay, 0, 0, Main.myPlayer);
            Main.projectile[heatray].tileCollide = false;
            //proj spawns arrows all around it until it dies
            Projectile.NewProjectile(player.GetSource_Accessory(modPlayer.RedRidingEnchantItem), target.Center.X, player.Center.Y - 500, 0f, 0f, ModContent.ProjectileType<ArrowRain>(), FargoSoulsUtil.HighestDamageTypeScaling(player, (int)(firstAmmo.damage * 5f)), 0f, player.whoAmI, arrowType, target.whoAmI);

            modPlayer.RedRidingArrowCD = 360;
        }

        private static Item PickAmmo(Player player)
        {
            Item item = new Item();
            bool flag = false;
            for (int i = 54; i < 58; i++)
            {
                if (player.inventory[i].ammo == AmmoID.Arrow && player.inventory[i].stack > 0)
                {
                    item = player.inventory[i];
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                for (int j = 0; j < 54; j++)
                {
                    if (player.inventory[j].ammo == AmmoID.Arrow && player.inventory[j].stack > 0)
                    {
                        item = player.inventory[j];
                        break;
                    }
                }
            }

            if (item.ammo != AmmoID.Arrow)
            {
                item.SetDefaults(ItemID.VenomArrow);
            }

            return item;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.HuntressAltHead)
            .AddIngredient(ItemID.HuntressAltShirt)
            .AddIngredient(ItemID.HuntressAltPants)
            .AddIngredient(null, "HuntressEnchant")
            .AddIngredient(ItemID.Marrow)
            .AddIngredient(ItemID.DD2BetsyBow)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
