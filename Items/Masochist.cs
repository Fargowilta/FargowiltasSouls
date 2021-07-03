using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items
{
    public class Masochist : SoulsItem
    {
        public override string Texture => "FargowiltasSouls/Items/Placeholder";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutant's Gift");
            Tooltip.SetDefault(@"Toggles Eternity Mode, entailing the following
Deviantt provides tips and assistance based on progress
Changes world to Expert Mode
Changes all vanilla and Souls Mod boss AIs and many enemy AIs
Compatible bosses drop additional loot including exclusive accessories
Rebalances many weapons and certain mechanics
Introduces new debuffs and possible debuff sources
Increases cash from enemies and adds certain drops
Increases spawn rates
Cannot be used while a boss is alive
Minions do reduced damage when used with another weapon
[c/ff0000:NOT INTENDED FOR USE WITH OTHER CONTENT MODS OR MODDED DIFFICULTIES]");
            DisplayName.AddTranslation(GameCulture.Chinese, "突变体的礼物");
            Tooltip.AddTranslation(GameCulture.Chinese, @"用于开/关永恒模式，包括以下内容
德维特会基于目前进度提供帮助和提示
将世界更改为专家模式
更改所有原版和Fargo之魂boss的AI和许多敌怪AI
兼容的boss掉落额外战利品，包括专属的饰品
平衡许多武器和某些机制
引入新的减益和减益来源
增加来自敌怪的金钱并添加特定的掉落
增加刷怪率
在boss活着时不能使用
召唤物会在使用其他武器时降低伤害
[c/ff0000:不适于与其他Mod的内容或难度一同使用]");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 1;
            item.rare = ItemRarityID.Blue;
            item.useAnimation = 30;
            item.useTime = 30;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = false;
        }

        public override bool UseItem(Player player)
        {
            bool bossExists = false;
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].active && Main.npc[i].boss)
                {
                    bossExists = true;
                    break;
                }
            }

            if (!bossExists)
            {
                FargoSoulsWorld.MasochistMode = !FargoSoulsWorld.MasochistMode;
                Main.expertMode = true;

                if (Main.netMode != NetmodeID.MultiplayerClient && FargoSoulsWorld.MasochistMode && !FargoSoulsWorld.spawnedDevi
                    && !NPC.AnyNPCs(ModLoader.GetMod("Fargowiltas").NPCType("Deviantt")))
                {
                    FargoSoulsWorld.spawnedDevi = true;

                    //NPC.SpawnOnPlayer(player.whoAmI, ModLoader.GetMod("Fargowiltas").NPCType("Deviantt"));
                    int projType = ModLoader.GetMod("Fargowiltas").ProjectileType("SpawnProj");
                    int spawnType = ModLoader.GetMod("Fargowiltas").NPCType("Deviantt");
                    Projectile.NewProjectile(player.Center - 1000 * Vector2.UnitY, Vector2.Zero,
                        projType, 0, 0, Main.myPlayer, spawnType);

                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("Deviantt has awoken!"), new Color(175, 75, 255));
                    else if (Main.netMode != NetmodeID.MultiplayerClient)
                        Main.NewText("Deviantt has awoken!", new Color(175, 75, 255));
                }

                Main.PlaySound(SoundID.Roar, (int)player.position.X, (int)player.position.Y, 0);

                string text = FargoSoulsWorld.MasochistMode ? "Eternity Mode initiated!" : "Eternity Mode deactivated!";
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(text, 175, 75, 255);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(text), new Color(175, 75, 255));
                    NetMessage.SendData(MessageID.WorldData); //sync world
                }
            }
            return true;
        }
    }
}
