using FargowiltasSouls.Buffs.Souls;
using FargowiltasSouls.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class HuntressEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Huntress Enchantment");
            Tooltip.SetDefault(
@"Attacks ignore 10 enemy defense and deal 5 flat extra damage
Each successive attack ignores an additional 10 defense and deals 5 more damage
This stacks up to 10 times
Homing and minion attacks do not increase these bonuses
Missing any attack will reset these bonuses
'Accuracy brings power'");
        }

        protected override Color nameColor => new Color(122, 192, 76);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Yellow;
            Item.value = 200000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            HuntressEffect(player);
        }

        public static void HuntressEffect(Player player)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.HuntressEnchantActive = true;

            if (modPlayer.HuntressCD > 0)
            {
                modPlayer.HuntressCD--;
            }
        }

        public static void HuntressBonus(FargoSoulsPlayer modPlayer, Projectile proj, NPC target, ref int damage)
        {
            proj.GetGlobalProjectile<FargoSoulsGlobalProjectile>().HuntressProj = 2;

            if (modPlayer.HuntressCD == 0)
            {
                modPlayer.HuntressStage++;

                if (modPlayer.HuntressStage >= 10)
                {
                    modPlayer.HuntressStage = 10;

                    if (modPlayer.RedRidingEnchantItem != null && modPlayer.RedRidingArrowCD == 0)
                    {
                        RedRidingEnchant.SpawnArrowRain(modPlayer.Player, target);
                    }
                }

                modPlayer.HuntressCD = 30;
            }

            proj.ArmorPenetration = 10 * modPlayer.HuntressStage;
            damage += 5 * modPlayer.HuntressStage;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.HuntressWig)
            .AddIngredient(ItemID.HuntressJerkin)
            .AddIngredient(ItemID.HuntressPants)
            .AddIngredient(ItemID.IceBow)
            .AddIngredient(ItemID.ShadowFlameBow)
            .AddIngredient(ItemID.DD2PhoenixBow)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
