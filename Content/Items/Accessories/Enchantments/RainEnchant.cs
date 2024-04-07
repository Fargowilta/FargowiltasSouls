using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class RainEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Rain Enchantment");
            /* Tooltip.SetDefault(
@"Grants immunity to Wet
Spawns a miniature storm that follows your cursor
It only attacks if there is a clear line of sight between you
Effects of Inner Tube
'Come again some other day'"); */
        }

        public override Color nameColor => new(255, 236, 0);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item);
        }
        public static void AddEffects(Player player, Item item)
        {
            player.buffImmune[BuffID.Wet] = true;
            player.AddEffect<RainUmbrellaEffect>(item);
            player.AddEffect<RainInnerTubeEffect>(item);
            player.AddEffect<RainWetEffect>(item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.RainHat)
            .AddIngredient(ItemID.RainCoat)
            .AddIngredient(ItemID.UmbrellaHat)
            .AddIngredient(ItemID.FloatingTube) //inner tube
            .AddIngredient(ItemID.Umbrella)
            .AddIngredient(ItemID.WaterGun)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
    public class RainUmbrellaEffect : AccessoryEffect
    {
        public override int ToggleItemType => ModContent.ItemType<RainEnchant>();
        public override Header ToggleHeader => Header.GetHeader<NatureHeader>();
        public override bool MinionEffect => true;
        public override void PostUpdateEquips(Player player)
        {
            if (!player.HasBuff(ModContent.BuffType<RainCDBuff>()))
            {
                player.FargoSouls().AddMinion(EffectItem(player), true, ModContent.ProjectileType<RainUmbrella>(), 0, 0);

                if (!player.controlDown)
                {
                    player.slowFall = true;
                }
            }
        }
    }
    public class RainWetEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            target.AddBuff(BuffID.Wet, 180);
        }
    }
    public class RainInnerTubeEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<NatureHeader>();
        public override int ToggleItemType => ModContent.ItemType<RainEnchant>();
        public override void PostUpdateEquips(Player player)
        {
            player.hasFloatingTube = true;
            player.canFloatInWater = true;
        }
    }
}
