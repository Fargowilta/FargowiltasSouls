using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Projectiles.Minions;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Forces
{
    public class WillForce : BaseForce
    {
        public override void SetStaticDefaults()
        {
            Enchants[Type] =
            [
                ModContent.ItemType<GoldEnchant>(),
                ModContent.ItemType<PlatinumEnchant>(),
                ModContent.ItemType<GladiatorEnchant>(),
                ModContent.ItemType<RedRidingEnchant>(),
                ModContent.ItemType<ValhallaKnightEnchant>()
            ];
        }
        public override void UpdateInventory(Player player)
        {
            player.AddEffect<GoldToPiggy>(Item);
        }
        public override void UpdateVanity(Player player)
        {
            player.AddEffect<GoldToPiggy>(Item);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SetActive(player);
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            player.AddEffect<GoldToPiggy>(Item);
            modPlayer.PlatinumEffect = Item;
            player.AddEffect<HuntressEffect>(Item);
            player.FargoSouls().ValhallaEnchantActive = true;
            player.AddEffect<ValhallaDash>(Item);
            SquireEnchant.SquireEffect(player, Item);
            player.AddEffect<WillGladiatorEffect>(Item);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            foreach (int ench in Enchants[Type])
                recipe.AddIngredient(ench);
            recipe.AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"));
            recipe.Register();
        }
    }
    public class WillGladiatorEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<WillHeader>();
        public override int ToggleItemType => ModContent.ItemType<WillForce>();
        public override bool MinionEffect => true;
        
        public override void PostUpdateEquips(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<GladiatorSpirit>()] == 0)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    Projectile proj = Projectile.NewProjectileDirect(GetSource_EffectItem(player), player.Center, Vector2.Zero, ModContent.ProjectileType<GladiatorSpirit>(), 0, 0f, player.whoAmI);
                    proj.netUpdate = true;
                }
            }
        }
    }
}
