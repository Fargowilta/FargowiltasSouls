using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class OrichalcumEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Orichalcum Enchantment");
            Tooltip.SetDefault(
@"Flower petals will cause extra damage to your target and inflict Orichalcum Poison
Damaging debuffs deal 2.5x damage
'Nature blesses you'");
        }

        protected override Color nameColor => new Color(235, 50, 145);
        public override string wizardEffect => Language.GetTextValue("Mods.FargowiltasSouls.WizardEffect.Orichalcum");

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            OrichalcumEffect(player, Item);
        }

        public static void OrichalcumEffect(Player player, Item item)
        {
            player.GetModPlayer<FargoSoulsPlayer>().OriEnchantItem = item;

            if (!player.GetToggleValue("Orichalcum"))
                return;

            player.onHitPetal = true;
        }

        public static void OriDotModifier(NPC npc, FargoSoulsPlayer modPlayer, ref int damage)
        {
            float multiplier = 2.5f;

            if (modPlayer.EarthForce)
            {
                multiplier = 4f;
            }

            npc.lifeRegen = (int)(npc.lifeRegen * multiplier);
            damage = (int)(damage * multiplier);

            //half as effective if daybreak applied
            if (npc.daybreak)
            {
                npc.lifeRegen /= 2;
                damage /= 2;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("FargowiltasSouls:AnyOriHead")
            .AddIngredient(ItemID.OrichalcumBreastplate)
            .AddIngredient(ItemID.OrichalcumLeggings)
            .AddIngredient(ItemID.FlowerofFire)
            .AddIngredient(ItemID.FlowerofFrost)
            .AddIngredient(ItemID.CursedFlames)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
