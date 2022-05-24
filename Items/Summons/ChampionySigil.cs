using Terraria.ID;
using Terraria;

namespace FargowiltasSouls.Items.Summons
{
    public class ChampionySigil : SigilOfChampions
    {
        public override string Texture => "FargowiltasSouls/Items/Summons/SigilOfChampions";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Championy Sigil");
            Tooltip.SetDefault("Summons the Champions");
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.maxStack = 20;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.consumable = true;
        }

        public override bool CanUseItem(Player player) => true;

        public override bool ConsumeItem(Player player) => player.altFunctionUse != 2;

        public override void AddRecipes() { }
    }
}