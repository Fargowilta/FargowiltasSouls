using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Forces
{
    public class TimberForce : BaseForce
    {

        public override void SetStaticDefaults()
        {
            Enchants[Type] =
            [
                ModContent.ItemType<WoodEnchant>(),
                ModContent.ItemType<BorealWoodEnchant>(),
                ModContent.ItemType<RichMahoganyEnchant>(),
                ModContent.ItemType<EbonwoodEnchant>(),
                ModContent.ItemType<ShadewoodEnchant>(),
                ModContent.ItemType<PalmWoodEnchant>(),
                ModContent.ItemType<PearlwoodEnchant>()
            ];
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            SetActive(player);
            WoodEnchant.WoodEffect(player, Item);
            player.AddEffect<MahoganyEffect>(Item);
            player.AddEffect<PearlwoodEffect>(Item);
            player.AddEffect<TimberEffect>(Item);
        }

        public override void UpdateVanity(Player player)
        {
            player.FargoSouls().WoodEnchantDiscount = true;
        }

        public override void UpdateInventory(Player player)
        {
            player.FargoSouls().WoodEnchantDiscount = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            foreach (int ench in Enchants[Type])
                recipe.AddIngredient(ench);
            recipe.AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"));
            recipe.Register();
        }
        public class TimberEffect : AccessoryEffect
        {
            public override Header ToggleHeader => Header.GetHeader<TimberHeader>();
            public override int ToggleItemType => ModContent.ItemType<TimberForce>();
            public override bool ExtraAttackEffect => true;
            public override bool IgnoresMutantPresence => true;

            public override void PostUpdateEquips(Player player)
            {
                FargoSoulsPlayer modPlayer = player.FargoSouls();
                if (modPlayer.TimberBranchCD > 0)
                    modPlayer.TimberBranchCD--;
            }
            public override void TryAdditionalAttacks(Player player, int damage, DamageClass damageType)
            {
                FargoSoulsPlayer modPlayer = player.FargoSouls();
                if (player.whoAmI == Main.myPlayer)
                {
                    if (modPlayer.TimberBranchCD <= 0)
                    {
                        Item item = EffectItem(player);
                        modPlayer.TimberBranchCD = 60 * 4;

                        Vector2 vel = Vector2.Normalize(Main.MouseWorld - player.Center) * 1f;
                        int branchDamage = FargoSoulsUtil.HighestDamageTypeScaling(player, 5750);

                        int depth = 5;
                        int p = Projectile.NewProjectile(player.GetSource_Accessory(item), player.Center, vel, ModContent.ProjectileType<TimberBranch>(), branchDamage, 1, Main.myPlayer, ai0: depth);

                        //int numSnowballs = 1;
                        //if (p != Main.maxProjectiles)
                        //FargoSoulsGlobalProjectile.SplitProj(Main.projectile[p], numSnowballs, MathHelper.Pi / 10, 1);
                    }
                    else // Slightly reduce cooldown on attack
                    {
                        modPlayer.TimberBranchCD -= 4;
                    }
                }
            }

        }
    }
}
