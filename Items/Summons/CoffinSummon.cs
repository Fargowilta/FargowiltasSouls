//JAVYZ TODO: CURSED COFFIN BOSS SUMMON
/*
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;
using FargowiltasSouls.NPCs.Challengers;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.IO;
using FargowiltasSouls.NPCs.Champions;

namespace FargowiltasSouls.Items.Summons
{

    public class CoffinSummon : SoulsItem
    {
        public override string Texture => "FargowiltasSouls/Items/Placeholder";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Coffin Summon");
            Tooltip.SetDefault("While in the underground Desert, summon the Cursed Coffin");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }

        public override bool IsLoadingEnabled(Mod mod) => false; //prevent appearing

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
                .AddRecipeGroup("FargowiltasSouls:AnyGoldBar", 4)
                .AddTile(TileID.DemonAltar)
                .Register();
        }

        public override bool CanUseItem(Player Player)
        {
            if (Player.ZoneDesert && Player.ZoneDirtLayerHeight)
                return !NPC.AnyNPCs(ModContent.NPCType<CursedCoffin>()); //not (x or y)
            return false;
        }

        public override bool? UseItem(Player Player)
        {
            NPC.SpawnOnPlayer(Player.whoAmI, ModContent.NPCType<CursedCoffin>());
            SoundEngine.PlaySound(SoundID.Roar, Player.Center);
            return true;
        }
    }
}
*/