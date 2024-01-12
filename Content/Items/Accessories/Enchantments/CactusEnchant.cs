using FargowiltasSouls.Content.Projectiles.Souls;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.ModPlayers;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class CactusEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Cactus Enchantment");
            /* Tooltip.SetDefault(
@"While attacking you release a spray of needles
Enemies will explode into needles on death if they are struck with your needles
'It's the quenchiest!'"); */

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "仙人掌魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
            // @"反弹25%接触伤害
            // 敌人死亡时有几率爆裂出针刺
            // '太解渴了！'");
        }

        protected override Color nameColor => new(121, 158, 29);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Green;
            Item.value = 20000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CactusEffect(player, Item);
        }

        public static void CactusEffect(Player player, Item item)
        {
            player.DisplayToggle("Cactus");
            //player.thorns = .25f;

            if (player.GetToggleValue("Cactus"))
            {
                FargoSoulsPlayer modPlayer = player.FargoSouls();
                modPlayer.CactusEnchantItem = item;

                if (modPlayer.CactusProcCD > 0)
                {
                    modPlayer.CactusProcCD--;
                }
            }
        }

        public static void CactusProc(NPC npc, Player player)
        {
            CactusSpray(player, npc.Center);
        }

        public static void CactusSelfProc(FargoSoulsPlayer modPlayer)
        {
            if (modPlayer.CactusProcCD == 0)
            {
                Player player = modPlayer.Player;
                CactusSpray(player, player.Center);
                modPlayer.CactusProcCD = 15;
            }
        }
        private static void CactusSpray(Player player, Vector2 position)
        {
            int dmg = 20;
            int numNeedles = 8;
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.ForceEffect(modPlayer.CactusEnchantItem.type))
            {
                dmg = 75;
                numNeedles = 16;
            }

            for (int i = 0; i < numNeedles; i++)
            {
                int p = Projectile.NewProjectile(player.GetSource_Accessory(modPlayer.CactusEnchantItem), player.Center, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 4, ModContent.ProjectileType<CactusNeedle>(), FargoSoulsUtil.HighestDamageTypeScaling(player, dmg), 5f);
                if (p != Main.maxProjectiles)
                {
                    Projectile proj = Main.projectile[p];
                    if (proj != null && proj.active)
                    {
                        proj.FargoSouls().CanSplit = false;

                        proj.ai[0] = 1; //these needles can inflict enemies with needled
                    }
                    
                }
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CactusHelmet)
                .AddIngredient(ItemID.CactusBreastplate)
                .AddIngredient(ItemID.CactusLeggings)
                .AddIngredient(ItemID.Waterleaf)
                .AddIngredient(ItemID.Flounder)
                .AddIngredient(ItemID.SecretoftheSands)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
