using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Summons
{
    public class MutantsCurse : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutant's Curse");
            Tooltip.SetDefault("'At least this way, you don't need that doll'");
            DisplayName.AddTranslation(GameCulture.Chinese, "突变体诅咒");
            Tooltip.AddTranslation(GameCulture.Chinese, "'至少不需要用娃娃了'");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(3, 11));
        }
        public override int NumFrames => 11;
        public override void SetDefaults()
        {
            item.width = 52;
            item.height = 52;
            item.rare = ItemRarityID.Purple;
            item.maxStack = 999;
            item.useAnimation = 30;
            item.useTime = 30;
            item.useStyle = ItemUseStyleID.HoldingUp;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.consumable = true;
            item.value = Item.buyPrice(1);
            item.noUseGraphic = true;
        }

        public override bool UseItem(Player player)
        {
            int mutant = NPC.FindFirstNPC(ModLoader.GetMod("Fargowiltas").NPCType("Mutant"));

            if (mutant > -1 && Main.npc[mutant].active)
            {
                Main.npc[mutant].Transform(mod.NPCType("MutantBoss"));
                if (Main.netMode == NetmodeID.SinglePlayer)
                if (Language.ActiveCulture == GameCulture.Chinese)
                {
                    Main.NewText("突变体已苏醒！", 175, 75, 255);
                }
                else
                {
                    Main.NewText("Mutant has awoken!", 175, 75, 255);
                }
                else if (Main.netMode == NetmodeID.Server)
                if (Language.ActiveCulture == GameCulture.Chinese)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("突变体已苏醒"), new Color(175, 75, 255));
                }
                else
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("Mutant has awoken!"), new Color(175, 75, 255));
                }
            }
            else
            {
                NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("MutantBoss"));
            }

            return true;
        }
    }
}
