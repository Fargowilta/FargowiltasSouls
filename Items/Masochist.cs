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
            /*DisplayName.SetDefault("Mutant's Gift");
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
[i:1612][c/00ff00:Recommended to use Fargo's Mutant Mod Debuff Display (in config)]
[c/ff0000:NOT INTENDED FOR USE WITH OTHER CONTENT MODS OR MODDED DIFFICULTIES]");
            DisplayName.AddTranslation(GameCulture.Chinese, "突变体的礼物");
            Tooltip.AddTranslation(GameCulture.Chinese, "'用开/关受虐模式'");*/
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
            if (!FargoSoulsUtil.AnyBossAlive())
            {
                FargoSoulsWorld.EternityMode = !FargoSoulsWorld.EternityMode;
                Main.expertMode = true;

                //if (FargoSoulsWorld.MasochistMode) ModLoader.GetMod("Fargowiltas").Call("DebuffDisplay", true);

                if (Main.netMode != NetmodeID.MultiplayerClient && FargoSoulsWorld.EternityMode && !FargoSoulsWorld.spawnedDevi
                    && !NPC.AnyNPCs(ModLoader.GetMod("Fargowiltas").NPCType("Deviantt")))
                {
                    FargoSoulsWorld.spawnedDevi = true;

                    //NPC.SpawnOnPlayer(player.whoAmI, ModLoader.GetMod("Fargowiltas").NPCType("Deviantt"));
                    int projType = ModLoader.GetMod("Fargowiltas").ProjectileType("SpawnProj");
                    int spawnType = ModLoader.GetMod("Fargowiltas").NPCType("Deviantt");
                    Projectile.NewProjectile(player.Center - 1000 * Vector2.UnitY, Vector2.Zero, projType, 0, 0, Main.myPlayer, spawnType);

                    FargoSoulsUtil.PrintText(Language.GetTextValue("Mods.FargowiltasSouls.Masochist.DevianttAwoken"), new Color(175, 75, 255));
                }

                Main.PlaySound(SoundID.Roar, (int)player.position.X, (int)player.position.Y, 0);

                FargoSoulsUtil.PrintText(FargoSoulsWorld.EternityMode ? Language.GetTextValue("Mods.FargowiltasSouls.Masochist.Activate") : Language.GetTextValue("Mods.FargowiltasSouls.Masochist.Deactivate"), new Color(175, 75, 255));

                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData); //sync world
            }
            return true;
        }
    }
}