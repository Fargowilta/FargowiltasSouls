using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class HuntressEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Huntress Enchantment");
            Tooltip.SetDefault(
@"Arrows will stick in enemies and apply a stacking bleed debuff per arrow
Hit the enemy with a melee attack to deal bonus damage per arrow




Arrows will periodically fall towards your cursor
The arrow type is based on the first arrow in your inventory
Double tap down to create a localized rain of arrows at the cursor's position for a few seconds
This has a cooldown of 15 seconds
Boosts Explosive Traps
'The Hunt is On'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "女猎人魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
            // @"箭矢会定期落至你光标周围
            // 箭矢的种类取决于你背包中第一个箭矢
            // 双击'下'键后令箭雨倾斜在光标位置
            // 此效果有15秒冷却时间
            // 爆炸机关攻击速度更快且会造成涂油减益
            // 点燃涂油的敌人以造成额外伤害
            // '狩猎开始了'");
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
            .AddIngredient(ItemID.DD2ExplosiveTrapT2Popper)
            //tendon bow
            .AddIngredient(ItemID.DaedalusStormbow)
            //shadiwflame bow
            .AddIngredient(ItemID.DD2PhoenixBow)
            //dog pet

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
