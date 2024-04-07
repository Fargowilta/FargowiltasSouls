using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class AncientShadowEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Ancient Shadow Enchantment");
            /* Tooltip.SetDefault(
@"Your attacks may inflict Darkness on enemies
Darkened enemies occasionally fire shadowflame tentacles at other enemies
Three Shadow Orbs will orbit around you
'Archaic, yet functional'"); */
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "远古暗影魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
            // @"攻击有几率造成黑暗减益
            // 身上带有黑暗减益的敌人有几率向其他敌人发射暗影焰触手
            // 三颗暗影珠围绕着你旋转
            // '十分古老，却非常实用'");
        }

        public override Color nameColor => new(94, 85, 220);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.FargoSouls().AncientShadowEnchantActive = true;
            player.AddEffect<AncientShadowDarkness>(Item);
            player.AddEffect<ShadowBalls>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.AncientShadowHelmet)
            .AddIngredient(ItemID.AncientShadowScalemail)
            .AddIngredient(ItemID.AncientShadowGreaves)
            //.AddIngredient(ItemID.AncientNecroHelmet);
            //.AddIngredient(ItemID.AncientGoldHelmet);
            .AddIngredient<ShadowEnchant>()
            .AddIngredient(ItemID.ShadowFlameKnife)
            .AddIngredient(ItemID.ShadowFlameHexDoll)
            //dart rifle
            //toxicarp

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
    public class AncientShadowDarkness : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ShadowHeader>();
        public override int ToggleItemType => ModContent.ItemType<AncientShadowEnchant>();
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            if (!player.FargoSouls().TerrariaSoul)
            {
                if ((projectile == null || projectile.type != ProjectileID.ShadowFlame) && Main.rand.NextBool(5))
                    target.AddBuff(BuffID.Darkness, 600, true);
            }
        }
    }
}
