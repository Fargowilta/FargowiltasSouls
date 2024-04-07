using FargowiltasSouls.Content.Bosses.AbomBoss;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Summons
{
    public class AbomsCurse : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Abominationn's Curse");
            // Tooltip.SetDefault("Must be used on the surface");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 10));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }
        public override int NumFrames => 10;
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 48;
            Item.rare = ItemRarityID.Purple;
            Item.maxStack = 20;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
            Item.value = Item.buyPrice(gold: 8);
        }

        public override bool CanUseItem(Player player) => player.Center.Y / 16 < Main.worldSurface;

        public override bool? UseItem(Player player)
        {
            FargoSoulsUtil.SpawnBossNetcoded(player, ModContent.NPCType<AbomBoss>());
            return true;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.GoblinBattleStandard)
            .AddIngredient(ItemID.PirateMap)
            .AddIngredient(ItemID.PumpkinMoonMedallion)
            .AddIngredient(ItemID.NaughtyPresent)
            .AddIngredient(ItemID.SnowGlobe)
            .AddIngredient(ItemID.DD2ElderCrystal)
            .AddIngredient(ItemID.LunarBar, 5)
            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}