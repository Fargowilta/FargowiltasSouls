using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Forces
{
    [AutoloadEquip(EquipType.Shield)]
    public class TerraForce : BaseForce
    {

        public override void SetStaticDefaults()
        {
            Enchants[Type] =
            [
                ModContent.ItemType<CopperEnchant>(),
                ModContent.ItemType<TinEnchant>(),
                ModContent.ItemType<IronEnchant>(),
                ModContent.ItemType<LeadEnchant>(),
                ModContent.ItemType<SilverEnchant>(),
                ModContent.ItemType<TungstenEnchant>(),
                ModContent.ItemType<ObsidianEnchant>()
            ];
        }

        public override void UpdateInventory(Player player)
        {
            AshWoodEnchant.PassiveEffect(player);
            IronEnchant.AddEffects(player, Item);
        }
        public override void UpdateVanity(Player player)
        {
            AshWoodEnchant.PassiveEffect(player);
            IronEnchant.AddEffects(player, Item);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SetActive(player);

            player.AddEffect<TerraLightningEffect>(Item);

            IronEnchant.AddEffects(player, Item);
            player.AddEffect<LeadEffect>(Item);
            player.AddEffect<SilverEffect>(Item);

            player.lavaImmune = true;
            player.fireWalk = true;

            //in lava effects
            if (player.lavaWet)
            {
                player.gravity = Player.defaultGravity;
                player.ignoreWater = true;
                player.accFlipper = true;
            }
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
    public class TerraLightningEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<TerraHeader>();
        public override int ToggleItemType => ModContent.ItemType<TerraForce>();
        public override bool ExtraAttackEffect => true;
        public override bool IgnoresMutantPresence => true;

        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.TerraProcCD > 0)
                modPlayer.TerraProcCD--;
        }
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            bool wetCheck = target.HasBuff(BuffID.Wet);
            if ((hitInfo.Crit || wetCheck))
            {
                LightningProc(player, target);
            }
        }

        public static void LightningProc(Player player, NPC target)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.TerraProcCD == 0)
            {
                int dmg = 3500;
                int cdLength = 300;

                Vector2 ai = target.Center - player.Center;
                Vector2 velocity = Vector2.Normalize(ai) * 20;

                int damage = FargoSoulsUtil.HighestDamageTypeScaling(modPlayer.Player, dmg);
                FargoSoulsUtil.NewProjectileDirectSafe(modPlayer.Player.GetSource_ItemUse(modPlayer.Player.HeldItem), player.Center, velocity, ModContent.ProjectileType<TerraLightning>(), damage, 0f, modPlayer.Player.whoAmI, ai.ToRotation());

                modPlayer.TerraProcCD = cdLength;
            }
        }
    }
}
