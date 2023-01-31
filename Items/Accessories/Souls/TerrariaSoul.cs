using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    [AutoloadEquip(EquipType.Shield)]
    public class TerrariaSoul : BaseSoul
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Soul of Terraria");

            string tooltip =
@"Summons fireballs, shadow orbs, icicles, leaf crystals, flameburst minion, hallowed sword and shield, and beetles
Right Click to Guard
Double tap down to spawn a sentry and portal, call a storm and arrow rain, toggle stealth, and direct your empowered guardian
Gold Key encases you in gold, Freeze Key freezes time for 5 seconds, minions spew scythes
Solar shield allows you to dash, Dash into any walls, to teleport through them
Throw a smoke bomb to teleport to it and gain the First Strike Buff
Attacks may spawn lightning, a storm cloud, flower petals, spectre orbs, a Dungeon Guardian, snowballs, spears, or buff boosters
Attacks cause increased life regen, shadow dodge, Flameburst shots, meteor showers, and reduced enemy immune frames
Critical chance is set to 25%, Crit to increase it by 5%, At 100% every 10th attack gains 4% life steal
Getting hit drops your crit back down, trigger a blood geyser, and reflects damage
Projectiles may split or shatter and spawn stars, item and projectile size increased, attract items from further away
Nearby enemies are ignited, You leave behind a trail of fire, jump to create a spore explosion
Grants Crimson regen, immunity to fire, fall damage, and lava, and doubled herb collection
Grants 50% chance for Mega Bees, 15% chance for minion crits, 20% chance for bonus loot
Critters have increased defense and their souls will aid you, You may summon temporary minions
All grappling hooks are more effective and fire homing shots, Greatly enhances all DD2 sentries
Your attacks inflict Midas, Enemies explode into needles
You violently explode to damage nearby enemies when hurt and revive with 200 HP when killed
Effects of Flower Boots and Greedy Ring
'A true master of Terraria'";
            Tooltip.SetDefault(tooltip);

            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 24));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                Main.spriteBatch.End(); //end and begin main.spritebatch to apply a shader
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
                GameShaders.Armor.Apply(GameShaders.Armor.GetShaderIdFromItemId(ItemID.LivingRainbowDye), Item, null); //use living rainbow dye shader
                Utils.DrawBorderString(Main.spriteBatch, line.Text, new Vector2(line.X, line.Y), Color.White, 1); //draw the tooltip manually
                Main.spriteBatch.End(); //then end and begin again to make remaining tooltip lines draw in the default way
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
                return false;
            }
            return true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = 5000000;
            Item.rare = -12;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            //includes revive, both spectres, adamantite, and star heal
            modPlayer.TerrariaSoul = true;

            //WOOD
            ModContent.Find<ModItem>(Mod.Name, "TimberForce").UpdateAccessory(player, hideVisual);
            //TERRA
            ModContent.Find<ModItem>(Mod.Name, "TerraForce").UpdateAccessory(player, hideVisual);
            //EARTH
            ModContent.Find<ModItem>(Mod.Name, "EarthForce").UpdateAccessory(player, hideVisual);
            //NATURE
            ModContent.Find<ModItem>(Mod.Name, "NatureForce").UpdateAccessory(player, hideVisual);
            //LIFE
            ModContent.Find<ModItem>(Mod.Name, "LifeForce").UpdateAccessory(player, hideVisual);
            //SPIRIT
            ModContent.Find<ModItem>(Mod.Name, "SpiritForce").UpdateAccessory(player, hideVisual);
            //SHADOW
            ModContent.Find<ModItem>(Mod.Name, "ShadowForce").UpdateAccessory(player, hideVisual);
            //WILL
            ModContent.Find<ModItem>(Mod.Name, "WillForce").UpdateAccessory(player, hideVisual);
            //COSMOS
            ModContent.Find<ModItem>(Mod.Name, "CosmoForce").UpdateAccessory(player, hideVisual);
        }
		
		public override void UpdateVanity(Player player)
		{
			player.GetModPlayer<FargoSoulsPlayer>().WoodEnchantDiscount = true;
		}

        public override void UpdateInventory(Player player)
        {
            player.GetModPlayer<FargoSoulsPlayer>().WoodEnchantDiscount = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(null, "TimberForce")
            .AddIngredient(null, "TerraForce")
            .AddIngredient(null, "EarthForce")
            .AddIngredient(null, "NatureForce")
            .AddIngredient(null, "LifeForce")
            .AddIngredient(null, "SpiritForce")
            .AddIngredient(null, "ShadowForce")
            .AddIngredient(null, "WillForce")
            .AddIngredient(null, "CosmoForce")
            .AddIngredient(null, "AbomEnergy", 10)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))


            .Register();
        }
    }
}
