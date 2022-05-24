using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class CobaltEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Cobalt Enchantment");
            Tooltip.SetDefault(
@"Grants an explosion jump
When you are hurt, you violently explode to damage nearby enemies
'I can't believe it's not Palladium'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "钴蓝魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
            // @"你的弹幕有25%几率爆裂成碎片
            // 此效果在每秒内只会发生一次
            // '真不敢相信这竟然不是钯金'");
        }

        protected override Color nameColor => new Color(61, 164, 196);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().CobaltEnchantItem = Item;
            AncientCobaltEnchant.AncientCobaltEffect(player, Item, 250);
        }

        public static void CobaltHurt(Player player, double damage)
        {
            if (player.GetToggleValue("Cobalt") && player.whoAmI == Main.myPlayer)
            {
                FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

                int baseDamage = 50;
                int multiplier = 2;
                int cap = 150;

                if (modPlayer.EarthForce)
                {
                    baseDamage = 50;
                    multiplier = 4;
                    cap = 250;
                }

                if (modPlayer.TerrariaSoul)
                {
                    baseDamage = 250;
                    multiplier = 5;
                    cap = 500;
                }

                int explosionDamage = baseDamage + (int)damage * multiplier;
                if (explosionDamage > cap)
                    explosionDamage = cap;

                Projectile p = FargoSoulsUtil.NewProjectileDirectSafe(player.GetSource_Accessory(modPlayer.CobaltEnchantItem), player.Center, Vector2.Zero, ModContent.ProjectileType<Explosion>(), (int)(explosionDamage * player.ActualClassDamage(DamageClass.Melee)), 0f, Main.myPlayer);
                if (p != null)
                    p.GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
            }
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
}
