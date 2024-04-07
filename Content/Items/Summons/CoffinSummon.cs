
using FargowiltasSouls.Content.Bosses.CursedCoffin;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace FargowiltasSouls.Content.Items.Summons
{

    public class CoffinSummon : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Coffin Summon");
            //Tooltip.SetDefault("While in the underground Desert, summon the Cursed Coffin");
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }

        public override bool IsLoadingEnabled(Mod mod) => CursedCoffin.Enabled;

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Orange;
            Item.consumable = true;
            Item.maxStack = 20;
            Item.noUseGraphic = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                //.AddRecipeGroup("FargowiltasSouls:AnyDemoniteBar", 4)
                .AddIngredient(ItemID.ClayBlock, 15)
                .AddIngredient(ItemID.FossilOre, 8)
                .AddIngredient(ItemID.Sapphire, 2)
                .AddTile(TileID.DemonAltar)
                .Register();
        }

        public override bool CanUseItem(Player Player)
        {
            if (Player.ZoneDesert && (Player.ZoneDirtLayerHeight || Player.ZoneRockLayerHeight))
                return !NPC.AnyNPCs(NPCType<CursedCoffin>()); //not (x or y)
            return false;
        }

        public override bool? UseItem(Player Player)
        {
            NPC.SpawnOnPlayer(Player.whoAmI, NPCType<CursedCoffin>());
            SoundEngine.PlaySound(SoundID.Shatter, Player.Center);
            return true;
        }
    }
}
