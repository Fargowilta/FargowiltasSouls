using Fargowiltas.Items.Summons.SwarmSummons.Energizers;
using Fargowiltas.Items.Tiles;
using FargowiltasSouls.Content.Items.Weapons.BossDrops;
using FargowiltasSouls.Content.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.SwarmDrops
{
    public class TheBigSting : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            // DisplayName.SetDefault("The Big Sting");
            /* Tooltip.SetDefault("Uses darts for ammo" +
                "\n66% chance to not consume ammo" +
                "\n'The reward for slaughtering many..'"); */

            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "大螫刺");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'屠戮众多的奖励..'");
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.width = 22;
            Item.height = 22;
            Item.damage = (int)(22 * 22 * 2.2);
            Item.useTime = 22 * 2;
            Item.useAnimation = 22 * 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.2f;
            Item.value = 500000;
            Item.rare = ItemRarityID.Purple;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BigStinger>();
            Item.useAmmo = AmmoID.Dart;
            Item.UseSound = SoundID.Item97;
            Item.shootSpeed = 15f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = Item.shoot;
            float multiplier = 1; //markiplier
            if (player.strongBees)
            {
                multiplier += 0.1f;
            }
            damage = (int)(damage * multiplier);
            knockback = (int)(knockback * multiplier);

        }

        public override Vector2? HoldoutOffset() => new Vector2(-30, 0);

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextBool(3);

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<TheSmallSting>())
            .AddIngredient(ModContent.ItemType<EnergizerBee>())
            .AddIngredient(ItemID.LunarBar, 10)

            .AddTile(ModContent.TileType<CrucibleCosmosSheet>())

            .Register();
        }
    }
}