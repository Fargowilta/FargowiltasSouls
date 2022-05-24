using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Souls;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class CactusEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Cactus Enchantment");
            Tooltip.SetDefault(
@"While attacking you release a spray of needles
Enemies will explode into needles on death if they are struck with your needles
'It's the quenchiest!'");

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "仙人掌魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
            // @"反弹25%接触伤害
            // 敌人死亡时有几率爆裂出针刺
            // '太解渴了！'");
        }

        protected override Color nameColor => new Color(121, 158, 29);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Green;
            Item.value = 20000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CactusEffect(player);
        }

        public static void CactusEffect(Player player)
        {
            //player.thorns = .25f;

            if (player.GetToggleValue("Cactus"))
            {
                FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
                modPlayer.CactusEnchantActive = true;

                if (modPlayer.CactusProcCD > 0)
                {
                    modPlayer.CactusProcCD--;
                }
            }
        }

        public static void CactusProc(NPC npc, Player player)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            int dmg = 20;
            int numNeedles = 8;

            if (modPlayer.LifeForce || modPlayer.WizardEnchantActive)
            {
                dmg = 75;
                numNeedles = 16;
            }

            Projectile[] projs = FargoSoulsUtil.XWay(numNeedles, player.GetSource_ItemUse(player.HeldItem), npc.Center, ModContent.ProjectileType<CactusNeedle>(), 4, FargoSoulsUtil.HighestDamageTypeScaling(player, dmg), 5f);

            double randomRotation = Main.rand.NextDouble() * MathHelper.Pi * 2;

            for (int i = 0; i < projs.Length; i++)
            {
                if (projs[i] == null) continue;
                Projectile p = projs[i];
                p.GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;

                p.velocity = p.velocity.RotatedBy(randomRotation);
            }
        }

        public static void CactusSelfProc(FargoSoulsPlayer modPlayer)
        {
            if (modPlayer.CactusProcCD == 0)
            {
                Player player = modPlayer.Player;
                int dmg = 20;
                int numNeedles = 8;

                if (modPlayer.LifeForce || modPlayer.WizardEnchantActive)
                {
                    dmg = 75;
                    numNeedles = 16;
                }

                Projectile[] projs = FargoSoulsUtil.XWay(numNeedles, player.GetSource_ItemUse(player.HeldItem), player.Center, ModContent.ProjectileType<CactusNeedle>(), 4, FargoSoulsUtil.HighestDamageTypeScaling(player, dmg), 5f);

                double randomRotation = Main.rand.NextDouble() * MathHelper.Pi * 2;

                for (int i = 0; i < projs.Length; i++)
                {
                    if (projs[i] == null) continue;
                    Projectile p = projs[i];
                    p.GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;

                    p.ai[0] = 1; //these needles can inflict enemies with needled
                    p.velocity = p.velocity.RotatedBy(randomRotation);
                }

                modPlayer.CactusProcCD = 15;
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
