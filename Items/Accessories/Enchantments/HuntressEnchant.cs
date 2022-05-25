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
Each successive attack ignores an additonal 10 defense and deals 5 more damage
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


            //if (player.GetToggleValue("Huntress") && player.whoAmI == Main.myPlayer)
            //{
            //    FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            //    modPlayer.HuntressCD++;

            //    Item firstAmmo = PickAmmo(player);
            //    int arrowType = firstAmmo.shoot;
            //    int damage = FargoSoulsUtil.HighestDamageTypeScaling(player, (int)(firstAmmo.damage * 3f));

            //    if (modPlayer.RedEnchantActive)
            //    {
            //        damage *= 2;
            //    }

            //    //fire arrow at nearby enemy
            //    if (modPlayer.HuntressCD >= 30)
            //    {
            //        Vector2 mouse = Main.MouseWorld;
            //        Vector2 pos = new Vector2(mouse.X - player.direction * 100, mouse.Y - 800);
            //        Vector2 velocity = Vector2.Normalize(mouse - pos) * 25;

            //        int p = Projectile.NewProjectile(player.GetProjectileSource_Misc(0), pos, velocity, arrowType, damage, 2, player.whoAmI);
            //        Main.projectile[p].noDropItem = true;
            //        //Main.projectile[p].extraUpdates = 2;

            //        modPlayer.HuntressCD = 0;
            //    }

            //    //arrow rain ability
            //    if (!player.HasBuff(ModContent.BuffType<HuntressCD>()) && modPlayer.DoubleTap)
            //    {
            //        Vector2 mouse = Main.MouseWorld;

            //        int heatray = Projectile.NewProjectile(player.GetProjectileSource_Misc(0), player.Center, new Vector2(0, -6f), ProjectileID.HeatRay, 0, 0, Main.myPlayer);
            //        Main.projectile[heatray].tileCollide = false;
            //        //proj spawns arrows all around it until it dies
            //        Projectile.NewProjectile(player.GetProjectileSource_Misc(0), mouse.X, player.Center.Y - 500, 0f, 0f, ModContent.ProjectileType<ArrowRain>(), FargoSoulsUtil.HighestDamageTypeScaling(player, firstAmmo.damage), 0f, player.whoAmI, arrowType, player.direction);

            //        player.AddBuff(ModContent.BuffType<HuntressCD>(), modPlayer.RedEnchantActive ? 600 : 900);
            //    }
            //}
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

                    //apply bleed
                    //target.AddBuff(ModContent.BuffType<HuntressBleed>(), 300);
                }

                modPlayer.HuntressCD = 30;
            }

            proj.ArmorPenetration = 10 * modPlayer.HuntressStage;
            damage += 5 * modPlayer.HuntressStage;
        }

        private static Item PickAmmo(Player player)
        {
            Item item = new Item();
            bool flag = false;
            for (int i = 54; i < 58; i++)
            {
                if (player.inventory[i].ammo == AmmoID.Arrow && player.inventory[i].stack > 0)
                {
                    item = player.inventory[i];
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                for (int j = 0; j < 54; j++)
                {
                    if (player.inventory[j].ammo == AmmoID.Arrow && player.inventory[j].stack > 0)
                    {
                        item = player.inventory[j];
                        break;
                    }
                }
            }

            if (item.ammo != AmmoID.Arrow)
            {
                item.SetDefaults(ItemID.ChlorophyteArrow);
            }

            return item;
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
