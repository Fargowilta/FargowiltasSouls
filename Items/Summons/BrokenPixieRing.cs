using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;
using FargowiltasSouls.NPCs.Challengers;

namespace FargowiltasSouls.Items.Summons
{

    public class BrokenPixieRing : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Broken Pixie Ring");
            Tooltip.SetDefault("Summons Lifelight");
        }

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 30;
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
                .AddIngredient(ItemID.PixieDust, 4)
                .AddIngredient(ItemID.SoulofLight, 3)
                .AddIngredient(ItemID.Amethyst)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public override bool CanUseItem(Player Player)
        {
            if (Player.ZoneHallow && Main.dayTime)
                return !(NPC.AnyNPCs(ModContent.NPCType<LifeChallenger>()) || NPC.AnyNPCs(ModContent.NPCType<LifeChallenger2>())); //not (x or y)
            return false;
        }

        public override bool? UseItem(Player Player)
        {
            if (Player.whoAmI == Main.myPlayer)
            {
                // If the player using the item is the client
                // (explicitely excluded serverside here)
                SoundEngine.PlaySound(SoundID.Roar, Player.position);

                int type = ModContent.NPCType<LifeChallenger>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // If the player is not in multiplayer, spawn directly
                    NPC.SpawnOnPlayer(Player.whoAmI, type);
                }
                else
                {
                    // If the player is in multiplayer, request a spawn
                    // This will only work if NPCID.Sets.MPAllowedEnemies[type] is true, set in NPC code
                    NetMessage.SendData(MessageID.SpawnBoss, number: Player.whoAmI, number2: type);
                }
            }

            return true;
        }
    }
}
