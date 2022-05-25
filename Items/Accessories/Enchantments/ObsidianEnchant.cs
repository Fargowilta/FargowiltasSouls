using FargowiltasSouls.Buffs.Souls;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class ObsidianEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Obsidian Enchantment");
            Tooltip.SetDefault(
@"Grants immunity to fire and lava
You have normal movement and can swim in lava
While standing in lava or lava wet, your attacks spawn explosions
'The earth calls'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "黑曜石魔石");
            //@"使你免疫火与岩浆
            //使你可以在岩浆中正常移动和游泳
            //在岩浆中时，你的攻击会引发爆炸
            //'大地的呼唤'"); e
        }

        protected override Color nameColor => new Color(69, 62, 115);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Orange;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ObsidianEffect(player);
        }

        public static void ObsidianEffect(Player player)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            player.lavaImmune = true;
            player.fireWalk = true;
            player.buffImmune[BuffID.OnFire] = true;

            //in lava effects
            if (player.lavaWet)
            {
                player.gravity = Player.defaultGravity;
                player.ignoreWater = true;
                player.accFlipper = true;

                if (player.GetToggleValue("Obsidian"))
                {
                    player.AddBuff(ModContent.BuffType<ObsidianLavaWetBuff>(), 600);
                }
            }

            modPlayer.ObsidianEnchantActive = (modPlayer.TerraForce) || player.lavaWet || modPlayer.LavaWet;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.ObsidianHelm)
            .AddIngredient(ItemID.ObsidianShirt)
            .AddIngredient(ItemID.ObsidianPants)
            .AddIngredient(ItemID.MoltenSkullRose) //molten skull rose
            .AddIngredient(ItemID.Cascade)
            .AddIngredient(ItemID.Fireblossom)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
