using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items
{
    public class ForgorGift : SoulsItem
    {
        public override string Texture => "FargowiltasSouls/Items/Placeholder";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Forgor Gift");
            Tooltip.SetDefault(@"[c/ff0000:Debug item]
Resets vanilla and Souls mod invasion/boss flags, INCLUDING hardmode (Wall of Flesh downed flag)
Right click allows getting a new gift from Deviantt, resets credit/discount card, resets your emode slot, rabies vaccine, sinew dash, etc.
Right click on the item in inventory to reset Mutant phase 1 skip
i forgor");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Blue;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            base.RightClick(player);

            FargoSoulsWorld.skipMutantP1 = 0;

            FargoSoulsUtil.PrintText("forgor");
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
                modPlayer.MutantsPactSlot = false;
                modPlayer.MutantsDiscountCard = false;
                modPlayer.MutantsCreditCard = false;
                modPlayer.ReceivedMasoGift = false;
                modPlayer.RabiesVaccine = false;
                modPlayer.DeerSinew = false;
                modPlayer.HasClickedWrench = false;
            }
            else
            {
                Main.hardMode = false;

                NPC.downedAncientCultist = false;
                NPC.downedBoss1 = false;
                NPC.downedBoss2 = false;
                NPC.downedBoss3 = false;
                NPC.downedChristmasIceQueen = false;
                NPC.downedChristmasSantank = false;
                NPC.downedChristmasTree = false;
                NPC.downedClown = false;
                NPC.downedDeerclops = false;
                NPC.downedEmpressOfLight = false;
                NPC.downedFishron = false;
                NPC.downedFrost = false;
                NPC.downedGoblins = false;
                NPC.downedGolemBoss = false;
                NPC.downedHalloweenKing = false;
                NPC.downedHalloweenTree = false;
                NPC.downedMartians = false;
                NPC.downedMechBoss1 = false;
                NPC.downedMechBoss2 = false;
                NPC.downedMechBoss3 = false;
                NPC.downedMechBossAny = false;
                NPC.downedMoonlord = false;
                NPC.downedPirates = false;
                NPC.downedPlantBoss = false;
                NPC.downedQueenBee = false;
                NPC.downedQueenSlime = false;
                NPC.downedSlimeKing = false;
                NPC.downedTowerNebula = false;
                NPC.downedTowerSolar = false;
                NPC.downedTowerStardust = false;
                NPC.downedTowerVortex = false;

                FargoSoulsWorld.downedAbom = false;
                FargoSoulsWorld.downedBetsy = false;
                FargoSoulsWorld.downedDevi = false;
                FargoSoulsWorld.downedFishronEX = false;
                FargoSoulsWorld.downedMutant = false;
                for (int i = 0; i < FargoSoulsWorld.downedBoss.Length; i++)
                    FargoSoulsWorld.downedBoss[i] = false;
            }

            FargoSoulsUtil.PrintText("forgor");
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData); //sync world

            return true;
        }
    }
}