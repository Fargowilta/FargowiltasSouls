//JAVYZ TODO: Banished Baron
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

    public class BaronSummon : SoulsItem
    {
        public override string Texture => "FargowiltasSouls/Items/Placeholder";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Baron Summon");
            Tooltip.SetDefault("While underwater at the Ocean, summon the Banished Baron");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }

        //public override bool IsLoadingEnabled(Mod mod) => false; //prevent appearing

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
            CreateRecipe() //change
                .AddRecipeGroup("FargowiltasSouls:AnyGoldBar", 4)
                .AddTile(TileID.DemonAltar)
                .Register();
        }

        public override bool CanUseItem(Player Player)
        {
            if (Player.ZoneBeach && Player.wet)
                return !NPC.AnyNPCs(ModContent.NPCType<BanishedBaron>()); //not (x or y)
            return false;
        }

        public override bool? UseItem(Player Player)
        {
            FargoSoulsUtil.SpawnBossNetcoded(Player, ModContent.NPCType<BanishedBaron>());
            SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Sounds/BaronSummon"), Player.Center);
            return true;
        }
    }
}
*/