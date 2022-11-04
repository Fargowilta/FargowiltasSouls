using FargowiltasSouls.Buffs.Souls;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class TitaniumEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Titanium Enchantment");
            Tooltip.SetDefault(
@"Attacking generates a defensive barrier of up to 20 titanium shards
When you reach the maximum amount, you gain 75% damage resistance for 3 seconds
This has a cooldown of 10 seconds
'The power of titanium in the palm of your hand'");
        }

        protected override Color nameColor => new Color(130, 140, 136);
        public override string wizardEffect => "Shard damage increased, Titanium Shield duration increased to 5 seconds and damage resistance increased to 95%";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TitaniumEffect(player, Item);
        }

        public static void TitaniumEffect(Player player, Item item)
        {
            if (player.GetToggleValue("Titanium"))
            {
                player.GetModPlayer<FargoSoulsPlayer>().TitaniumEnchantItem = item;
            }
        }

        public static void TitaniumShards(FargoSoulsPlayer modPlayer, Player player)
        {
            player.AddBuff(306, 600, true, false);
            if (player.ownedProjectileCounts[ProjectileID.TitaniumStormShard] < 20)
            {
                int baseDamage = 50;

                if (modPlayer.EarthForce)
                {
                    baseDamage = 120;
                }

                Projectile.NewProjectile(player.GetSource_Accessory(modPlayer.TitaniumEnchantItem), player.Center, Vector2.Zero, ProjectileID.TitaniumStormShard /*ModContent.ProjectileType<TitaniumShard>()*/, FargoSoulsUtil.HighestDamageTypeScaling(player, baseDamage), 15f, player.whoAmI, 0f, 0f);
            }
            else
            {
                if (!modPlayer.TitaniumCD && !player.HasBuff(ModContent.BuffType<TitaniumDRBuff>()))
                {
                    //dust ring
                    for (int j = 0; j < 20; j++)
                    {
                        Vector2 vector6 = Vector2.UnitY * 5f;
                        vector6 = vector6.RotatedBy((j - (20 / 2 - 1)) * 6.28318548f / 20) + player.Center;
                        Vector2 vector7 = vector6 - player.Center;
                        int d = Dust.NewDust(vector6 + vector7, 0, 0, DustID.Titanium);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].scale = 1.5f;
                        Main.dust[d].velocity = vector7;
                    }

                    int buffDuration = 180;

                    if (modPlayer.EarthForce)
                    {
                        buffDuration = 300;
                    }

                    player.AddBuff(ModContent.BuffType<TitaniumDRBuff>(), buffDuration);
                    player.AddBuff(ModContent.BuffType<TitaniumCD>(), buffDuration + 600);
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("FargowiltasSouls:AnyTitaHead")
                .AddIngredient(ItemID.TitaniumBreastplate)
                .AddIngredient(ItemID.TitaniumLeggings)
                .AddIngredient(ItemID.Chik)
                .AddIngredient(ItemID.CrystalStorm)
                .AddIngredient(ItemID.CrystalVileShard)

                .AddTile(TileID.CrystalBall)
                .Register();
        }
    }
}
