using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;
using FargowiltasSouls.NPCs.Challengers;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Items.Summons
{

    public class FragilePixieLamp : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fragile Pixie Lamp");
            Tooltip.SetDefault("While in the Hallow during day, hold out to break, angering the pixies' master");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.useAnimation = 120;
            Item.useTime = 120;
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
                .AddIngredient(ItemID.Glass, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public override bool CanUseItem(Player Player)
        {
            if (Player.ZoneHallow && Main.dayTime)
                return !(NPC.AnyNPCs(ModContent.NPCType<LifeChallenger>()) || NPC.AnyNPCs(ModContent.NPCType<LifeChallenger>())); //not (x or y)
            return false;
        }
        public Vector2 OriginalLocation = Vector2.Zero;
        public override void UseItemFrame(Player player)
        {
            float shake = (120 - player.itemAnimation) / 20;
            player.itemLocation = player.itemLocation + new Vector2(Main.rand.NextFloat(-shake, shake), Main.rand.NextFloat(-shake, shake));
        }
        public override void HoldItem(Player player)
        {
            if (player.itemAnimation == player.itemAnimationMax)
            {
                OriginalLocation = player.itemLocation;
            }
            Vector2 ItemCenter = player.itemLocation + new Vector2(Item.width / 2 * player.direction, -Item.height / 2);
            if (player.itemAnimation > 1)
            {
                SoundEngine.PlaySound(SoundID.Pixie, player.Center);
                Dust.NewDust(ItemCenter, 0, 0, DustID.Pixie, Main.rand.Next(-2, 2), Main.rand.Next(-2, 2), 100, new Color(), 0.75f);
            }
            if (player.itemAnimation == 1)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<LifeChallenger>()) && player.ZoneHallow && Main.dayTime)
                {
                    SoundEngine.PlaySound(SoundID.Shatter, player.Center);
                    //shatter effect
                    for (int i = 0; i < 50; i++)
                        Dust.NewDust(ItemCenter - (Item.Size/2), Item.width, Item.height, DustID.Glass, player.velocity.X, player.velocity.Y, 100, new Color(), 1f);
                    for (int i = 1; i < 4; i++)
                        Gore.NewGore(Item.GetSource_FromThis(), ItemCenter, player.velocity, ModContent.Find<ModGore>(Mod.Name, $"PixieLampGore{i}").Type, Item.scale);
                    if (player.whoAmI == Main.myPlayer)
                    {
                        // If the player using the item is the client
                        // (explicitely excluded serverside here)
                       //SoundEngine.PlaySound(SoundID.Roar, player.position);

                        int type = ModContent.NPCType<LifeChallenger>();

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            // If the player is not in multiplayer, spawn directly
                            NPC.SpawnOnPlayer(player.whoAmI, type);
                        }
                        else
                        {
                            // If the player is in multiplayer, request a spawn
                            // This will only work if NPCID.Sets.MPAllowedEnemies[type] is true, set in NPC code
                            NetMessage.SendData(MessageID.SpawnBoss, number: player.whoAmI, number2: type);
                        }
                    }
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath3, player.Center);
                    //play fail sound
                }
            }
        }

        public override bool? UseItem(Player Player)
        {
            return true;
        }
    }
}
