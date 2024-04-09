using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class ChlorophyteEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Chlorophyte Enchantment");

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "叶绿魔石");

            /*string tooltip =
@"Summons a ring of leaf crystals to shoot at nearby enemies
Grants a double spore jump
While using wings, spores will continuously spawn
Allows the ability to dash slightly
Double tap a direction
'The jungle's essence crystallizes around you'"; */
            // Tooltip.SetDefault(tooltip);

            //             string tooltip_ch =
            // @"召唤一圈叶状水晶射击附近的敌人
            // 使你获得孢子二段跳能力
            // 使用翅膀进行飞行时会在你周围不断生成孢子
            // '丛林的精华凝结在你周围'";
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }

        public override Color nameColor => new(36, 137, 0);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.LightPurple;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item);
        }
        public static void AddEffects(Player player, Item item)
        {
            player.AddEffect<ChloroMinion>(item);
            player.FargoSouls().ChlorophyteEnchantActive = true;
            player.AddEffect<JungleJump>(item);
            player.AddEffect<JungleDashEffect>(item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("FargowiltasSouls:AnyChloroHead")
            .AddIngredient(ItemID.ChlorophytePlateMail)
            .AddIngredient(ItemID.ChlorophyteGreaves)
            .AddIngredient(null, "JungleEnchant")
            .AddIngredient(ItemID.ChlorophyteWarhammer)
            .AddIngredient(ItemID.ChlorophyteClaymore)
            //grape juice
            //.AddIngredient(ItemID.Seedling);
            //plantero pet

            .AddTile(TileID.CrystalBall)
           .Register();
        }
    }
    public class ChloroMinion : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<NatureHeader>();
        public override int ToggleItemType => ModContent.ItemType<ChlorophyteEnchant>();
        public override bool MinionEffect => true;
        public override void PostUpdateEquips(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<Chlorofuck>()] == 0)
            {
                int dmg = player.FargoSouls().ForceEffect<ChlorophyteEnchant>() ? 65 : 35;
                const int max = 5;
                float rotation = 2f * (float)Math.PI / max;
                for (int i = 0; i < max; i++)
                {
                    Vector2 spawnPos = player.Center + new Vector2(60, 0f).RotatedBy(rotation * i);
                    FargoSoulsUtil.NewSummonProjectile(GetSource_EffectItem(player), spawnPos, Vector2.Zero, ModContent.ProjectileType<Chlorofuck>(), dmg, 10f, player.whoAmI, Chlorofuck.Cooldown, rotation * i);
                }
            }
        }
    }
}
