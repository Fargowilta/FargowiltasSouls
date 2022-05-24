using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.FinalUpgrades
{
    public class SparklingLove : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Sparkling Love");
            Tooltip.SetDefault(@"Right click to summon the soul of Deviantt
Right click pattern becomes denser with up to 12 empty minion slots
'The soul-consuming demon axe of love and justice from a defeated foe...'");
        }

        public override void SetDefaults()
        {
            Item.damage = 1700;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 27;
            Item.useTime = 27;
            Item.shootSpeed = 16f;
            Item.knockBack = 14f;
            Item.width = 32;
            Item.height = 32;
            Item.scale = 2f;
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<Projectiles.BossWeapons.SparklingLove>();
            Item.value = Item.sellPrice(0, 70);
            Item.noMelee = true; //no melee hitbox
            Item.noUseGraphic = true; //dont draw Item
            Item.DamageType = DamageClass.Melee;
            Item.autoReuse = true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.shoot = ModContent.ProjectileType<Projectiles.BossWeapons.SparklingDevi>();
                Item.useStyle = ItemUseStyleID.Swing;
                Item.DamageType = DamageClass.Summon;
                Item.noUseGraphic = false;
                Item.noMelee = false;
                Item.useAnimation = 66;
                Item.useTime = 66;
                Item.mana = 100;
            }
            else
            {
                Item.shoot = ModContent.ProjectileType<Projectiles.BossWeapons.SparklingLove>();
                Item.useStyle = ItemUseStyleID.Swing;
                Item.DamageType = DamageClass.Melee;
                Item.noUseGraphic = true;
                Item.noMelee = true;
                Item.useAnimation = 27;
                Item.useTime = 27;
                Item.mana = 0;
            }
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                FargoSoulsUtil.NewSummonProjectile(source, position, velocity, type, Item.damage, knockback, player.whoAmI);
                return false;
            }

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                Main.spriteBatch.End(); //end and begin main.spritebatch to apply a shader
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
                var lineshader = GameShaders.Misc["PulseCircle"].UseColor(new Color(255, 48, 154)).UseSecondaryColor(new Color(255, 169, 240));
                lineshader.Apply(null);
                Utils.DrawBorderString(Main.spriteBatch, line.Text, new Vector2(line.X, line.Y), new Color(255, 169, 240), 1); //draw the tooltip manually
                Main.spriteBatch.End(); //then end and begin again to make remaining tooltip lines draw in the default way
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            //.AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "EnergizerMoon"));
            .AddIngredient(ModContent.ItemType<EternalEnergy>(), 30)
            .AddIngredient(ModContent.ItemType<AbomEnergy>(), 30)
            .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 30)
            .AddIngredient(ModContent.ItemType<BrokenBlade>())
            .AddIngredient(ModContent.ItemType<SparklingAdoration>())

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}