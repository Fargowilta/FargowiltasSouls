using FargowiltasSouls.Content.Bosses.MutantBoss;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Summons
{
    public class MutantsCurse : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mutant's Curse");
            /* Tooltip.SetDefault(@"Must be used on the surface
'At least this way, you don't need that doll'"); */
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "突变体的诅咒");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'至少不需要用娃娃了'");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(3, 11));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }
        public override int NumFrames => 11;
        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 52;
            Item.rare = ItemRarityID.Purple;
            Item.maxStack = 20;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
            Item.value = Item.buyPrice(1);
        }

        public override bool CanUseItem(Player player) => player.Center.Y / 16 < Main.worldSurface;

        public override bool? UseItem(Player player)
        {
            FargoSoulsUtil.SpawnBossNetcoded(player, ModContent.NPCType<MutantBoss>());
            return true;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
    }
}