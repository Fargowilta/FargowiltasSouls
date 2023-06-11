using FargowiltasSouls.Items.Misc;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace FargowiltasSouls
{
    public class FargoSoulsWorld : ModSystem
    {
        internal enum Downed //to keep them organized and synced, DO NOT rearrange
        {
            TimberChampion,
            TerraChampion,
            EarthChampion,
            NatureChampion,
            LifeChampion,
            ShadowChampion,
            SpiritChampion,
            WillChampion,
            CosmosChampion,
            TrojanSquirrel,
            LifeChallenger,
            CursedCoffin,
            BanishedBaron
        }

        public static bool SwarmActive;

        public static bool downedBetsy;

        //masomode
        public const int MaxCountPreHM = 560;
        public const int MaxCountHM = 240;

        public static bool ShouldBeEternityMode;
        public static bool EternityMode { get; private set; }
        public static bool MasochistModeReal;
        public static bool CanPlayMaso;
        public static bool downedFishronEX;
        public static bool downedDevi;
        public static bool downedAbom;
        public static bool downedMutant;
        public static bool AngryMutant;

        public static bool haveForcedAbomFromGoblins;
        public static int skipMutantP1;

        public static bool ReceivedTerraStorage;
        public static bool spawnedDevi;

        public static bool[] downedBoss = new bool[Enum.GetValues(typeof(Downed)).Length];
        public static bool downedAnyBoss;

        public static bool WOFDroppedDeviGift2;

        public override void Unload()
        {
            base.Unload();

            downedBoss = null;
        }

        private void ResetFlags()
        {
            SwarmActive = false;
            downedBetsy = false;

            ShouldBeEternityMode = false;
            EternityMode = false;
            CanPlayMaso = false;
            MasochistModeReal = false;
            downedFishronEX = false;
            downedDevi = false;
            downedAbom = false;
            downedMutant = false;
            AngryMutant = false;

            haveForcedAbomFromGoblins = false;
            skipMutantP1 = 0;

            ReceivedTerraStorage = false;
            spawnedDevi = false;

            for (int i = 0; i < downedBoss.Length; i++)
                downedBoss[i] = false;

            downedAnyBoss = false;
            WOFDroppedDeviGift2 = false;
        }

        public override void OnWorldLoad()
        {
            ResetFlags();
        }

        public override void OnWorldUnload()
        {
            ResetFlags();
        }

        public override void SaveWorldData(TagCompound tag)
        {

            List<string> downed = new List<string>();
            if (downedBetsy) downed.Add("betsy");
            if (ShouldBeEternityMode) downed.Add("shouldBeEternityMode");
            if (EternityMode) downed.Add("eternity");
            if (CanPlayMaso) downed.Add("CanPlayMaso");
            if (MasochistModeReal) downed.Add("getReal");
            if (downedFishronEX) downed.Add("downedFishronEX");
            if (downedDevi) downed.Add("downedDevi");
            if (downedAbom) downed.Add("downedAbom");
            if (downedMutant) downed.Add("downedMutant");
            if (AngryMutant) downed.Add("AngryMutant");
            if (haveForcedAbomFromGoblins) downed.Add("haveForcedAbomFromGoblins");
            if (ReceivedTerraStorage) downed.Add("ReceivedTerraStorage");
            if (spawnedDevi) downed.Add("spawnedDevi");
            if (downedAnyBoss) downed.Add("downedAnyBoss");
            if (WOFDroppedDeviGift2) downed.Add("WOFDroppedDeviGift2");

            for (int i = 0; i < downedBoss.Length; i++)
            {
                if (downedBoss[i])
                    downed.Add("downedBoss" + i.ToString());
            }

            tag.Add("downed", downed);
            tag.Add("mutantP1", skipMutantP1);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            IList<string> downed = tag.GetList<string>("downed");
            downedBetsy = downed.Contains("betsy");
            ShouldBeEternityMode = downed.Contains("shouldBeEternityMode");
            EternityMode = downed.Contains("eternity") || downed.Contains("masochist");
            CanPlayMaso = downed.Contains("CanPlayMaso");
            MasochistModeReal = downed.Contains("getReal");
            downedFishronEX = downed.Contains("downedFishronEX");
            downedDevi = downed.Contains("downedDevi");
            downedAbom = downed.Contains("downedAbom");
            downedMutant = downed.Contains("downedMutant");
            AngryMutant = downed.Contains("AngryMutant");
            haveForcedAbomFromGoblins = downed.Contains("haveForcedAbomFromGoblins");
            ReceivedTerraStorage = downed.Contains("ReceivedTerraStorage");
            spawnedDevi = downed.Contains("spawnedDevi");
            downedAnyBoss = downed.Contains("downedAnyBoss");
            WOFDroppedDeviGift2 = downed.Contains("WOFDroppedDeviGift2");

            for (int i = 0; i < downedBoss.Length; i++)
                downedBoss[i] = downed.Contains($"downedBoss{i}") || downed.Contains($"downedChampion{i}");

            if (tag.ContainsKey("mutantP1"))
                skipMutantP1 = tag.GetAsInt("mutantP1");
        }

        public override void NetReceive(BinaryReader reader)
        {
            skipMutantP1 = reader.ReadInt32();

            BitsByte flags = reader.ReadByte();
            downedBetsy = flags[0];
            EternityMode = flags[1];
            downedFishronEX = flags[2];
            downedDevi = flags[3];
            downedAbom = flags[4];
            downedMutant = flags[5];
            AngryMutant = flags[6];
            haveForcedAbomFromGoblins = flags[7];

            flags = reader.ReadByte();
            ReceivedTerraStorage = flags[0];
            spawnedDevi = flags[1];
            MasochistModeReal = flags[2];
            CanPlayMaso = flags[3];
            ShouldBeEternityMode = flags[4];
            downedAnyBoss = flags[5];
            WOFDroppedDeviGift2 = flags[6];

            for (int i = 0; i < downedBoss.Length; i++)
            {
                int bits = i % 8;
                if (bits == 0)
                    flags = reader.ReadByte();

                downedBoss[i] = flags[bits];
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(skipMutantP1);

            writer.Write(new BitsByte
            {
                [0] = downedBetsy,
                [1] = EternityMode,
                [2] = downedFishronEX,
                [3] = downedDevi,
                [4] = downedAbom,
                [5] = downedMutant,
                [6] = AngryMutant,
                [7] = haveForcedAbomFromGoblins
            });

            writer.Write(new BitsByte
            {
                [0] = ReceivedTerraStorage,
                [1] = spawnedDevi,
                [2] = MasochistModeReal,
                [3] = CanPlayMaso,
                [4] = ShouldBeEternityMode,
                [5] = downedAnyBoss,
                [6] = WOFDroppedDeviGift2
            });

            BitsByte bitsByte = new BitsByte();
            for (int i = 0; i < downedBoss.Length; i++)
            {
                int bit = i % 8;

                if (bit == 0 && i != 0)
                {
                    writer.Write(bitsByte);
                    bitsByte = new BitsByte();
                }

                bitsByte[bit] = downedBoss[i];
            }
            writer.Write(bitsByte);
        }

        public override void PreUpdateNPCs()
        {
            SwarmActive = ModLoader.TryGetMod("Fargowiltas", out Mod fargo) && (bool)fargo.Call("SwarmActive");
        }

        public override void PostUpdateWorld()
        {
            NPC.LunarShieldPowerExpert = 150;

            if (ShouldBeEternityMode)
            {
                if (EternityMode && !FargoSoulsUtil.WorldIsExpertOrHarder())
                {
                    EternityMode = false;
                    FargoSoulsUtil.PrintLocalization($"Mods.{Mod.Name}.Message.{Name}.EternityWrongDifficulty", new Color(175, 75, 255));
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.WorldData);
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(SoundID.Roar, Main.LocalPlayer.Center);
                }
                else if (!EternityMode && FargoSoulsUtil.WorldIsExpertOrHarder() && !FargoSoulsUtil.AnyBossAlive())
                {
                    EternityMode = true;
                    FargoSoulsUtil.PrintLocalization($"Mods.{Mod.Name}.Message.{Name}.EternityOn", new Color(175, 75, 255));
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.WorldData);
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(SoundID.Roar, Main.LocalPlayer.Center);
                }
            }
            else if (EternityMode)
            {
                EternityMode = false;
                FargoSoulsUtil.PrintLocalization($"Mods.{Mod.Name}.Message.{Name}.EternityOff", new Color(175, 75, 255));
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
                if (!Main.dedServ)
                    SoundEngine.PlaySound(SoundID.Roar, Main.LocalPlayer.Center);
            }

            if (EternityMode)
            {
                NPC.LunarShieldPowerExpert = 50;

                if (Main.raining || Sandstorm.Happening || Main.bloodMoon)
                {
                    if (!haveForcedAbomFromGoblins && !downedAnyBoss //pre boss, disable some events
                        && ModContent.TryFind("Fargowiltas", "Abominationn", out ModNPC abom) && !NPC.AnyNPCs(abom.Type))
                    {
                        Main.raining = false;
                        Main.rainTime = 0;
                        Main.maxRaining = 0;
                        Sandstorm.Happening = false;
                        Sandstorm.TimeLeft = 0;
                        if (Main.bloodMoon)
                            FargoSoulsUtil.PrintLocalization($"Mods.{Mod.Name}.Message.{Name}.BloodMoonCancel", new Color(175, 75, 255));
                        Main.bloodMoon = false;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);
                    }
                }

                if (!MasochistModeReal && EternityMode && FargoSoulsUtil.WorldIsMaster() && CanPlayMaso && !FargoSoulsUtil.AnyBossAlive())
                {
                    MasochistModeReal = true;
                    FargoSoulsUtil.PrintLocalization($"Mods.{Mod.Name}.Message.{Name}.MasochistOn", new Color(51, 255, 191, 0));
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.WorldData);
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(SoundID.Roar, Main.LocalPlayer.Center);
                }
            }

            if (MasochistModeReal && !(EternityMode && FargoSoulsUtil.WorldIsMaster() && CanPlayMaso))
            {
                MasochistModeReal = false;
                FargoSoulsUtil.PrintLocalization($"Mods.{Mod.Name}.Message.{Name}.MasochistOff", new Color(51, 255, 191, 0));
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
                if (!Main.dedServ)
                    SoundEngine.PlaySound(SoundID.Roar, Main.LocalPlayer.Center);
            }

            //Main.NewText(BuilderMode);

            #region commented

            //right when day starts
            /*if(/*Main.time == 0 && Main.dayTime && !Main.eclipse && FargoSoulsWorld.masochistMode)
            {
                    SoundEngine.PlaySound(SoundID.Roar, player.Center);

                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.eclipse = true;
                        //Main.NewText(Lang.misc[20], 50, 255, 130, false);
                    }
                    else
                    {
                        //NetMessage.SendData(61, -1, -1, "", player.whoAmI, -6f, 0f, 0f, 0, 0, 0);
                    }


            }*/

            // if (this.itemTime == 0 && this.itemAnimation > 0 && item.type == 361 && Main.CanStartInvasion(1, true))
            // {
            // this.itemTime = item.useTime;
            // SoundEngine.PlaySound(SoundID.Roar, this.Center);
            // if (Main.netMode != NetmodeID.MultiplayerClient)
            // {
            // if (Main.invasionType == 0)
            // {
            // Main.invasionDelay = 0;
            // Main.StartInvasion(1);
            // }
            // }
            // else
            // {
            // NetMessage.SendData(61, -1, -1, "", this.whoAmI, -1f, 0f, 0f, 0, 0, 0);
            // }
            // }
            // if (this.itemTime == 0 && this.itemAnimation > 0 && item.type == 602 && Main.CanStartInvasion(2, true))
            // {
            // this.itemTime = item.useTime;
            // SoundEngine.PlaySound(SoundID.Roar, this.Center);
            // if (Main.netMode != NetmodeID.MultiplayerClient)
            // {
            // if (Main.invasionType == 0)
            // {
            // Main.invasionDelay = 0;
            // Main.StartInvasion(2);
            // }
            // }
            // else
            // {
            // NetMessage.SendData(61, -1, -1, "", this.whoAmI, -2f, 0f, 0f, 0, 0, 0);
            // }
            // }
            // if (this.itemTime == 0 && this.itemAnimation > 0 && item.type == 1315 && Main.CanStartInvasion(3, true))
            // {
            // this.itemTime = item.useTime;
            // SoundEngine.PlaySound(SoundID.Roar, this.Center);
            // if (Main.netMode != NetmodeID.MultiplayerClient)
            // {
            // if (Main.invasionType == 0)
            // {
            // Main.invasionDelay = 0;
            // Main.StartInvasion(3);
            // }
            // }
            // else
            // {
            // NetMessage.SendData(61, -1, -1, "", this.whoAmI, -3f, 0f, 0f, 0, 0, 0);
            // }
            // }
            // if (this.itemTime == 0 && this.itemAnimation > 0 && item.type == 1844 && !Main.dayTime && !Main.pumpkinMoon && !Main.snowMoon && !DD2Event.Ongoing)
            // {
            // this.itemTime = item.useTime;
            // SoundEngine.PlaySound(SoundID.Roar, this.Center);
            // if (Main.netMode != NetmodeID.MultiplayerClient)
            // {
            // Main.NewText(Lang.misc[31], 50, 255, 130, false);
            // Main.startPumpkinMoon();
            // }
            // else
            // {
            // NetMessage.SendData(61, -1, -1, "", this.whoAmI, -4f, 0f, 0f, 0, 0, 0);
            // }
            // }

            // if (this.itemTime == 0 && this.itemAnimation > 0 && item.type == 3601 && NPC.downedGolemBoss && Main.hardMode && !NPC.AnyDanger() && !NPC.AnyoneNearCultists())
            // {
            // SoundEngine.PlaySound(SoundID.Roar, this.Center);
            // this.itemTime = item.useTime;
            // if (Main.netMode == NetmodeID.SinglePlayer)
            // {
            // WorldGen.StartImpendingDoom();
            // }
            // else
            // {
            // NetMessage.SendData(61, -1, -1, "", this.whoAmI, -8f, 0f, 0f, 0, 0, 0);
            // }
            // }
            // if (this.itemTime == 0 && this.itemAnimation > 0 && item.type == 1958 && !Main.dayTime && !Main.pumpkinMoon && !Main.snowMoon && !DD2Event.Ongoing)
            // {
            // this.itemTime = item.useTime;
            // SoundEngine.PlaySound(SoundID.Roar, this.Center);
            // if (Main.netMode != NetmodeID.MultiplayerClient)
            // {
            // Main.NewText(Lang.misc[34], 50, 255, 130, false);
            // Main.startSnowMoon();
            // }
            // else
            // {
            // NetMessage.SendData(61, -1, -1, "", this.whoAmI, -5f, 0f, 0f, 0, 0, 0);
            // }
            // }

            #endregion
        }

        public override void PostWorldGen()
        {
            /*WorldGen.PlaceTile(Main.spawnTileX - 1, Main.spawnTileY, TileID.GrayBrick, false, true);
            WorldGen.PlaceTile(Main.spawnTileX, Main.spawnTileY, TileID.GrayBrick, false, true);
            WorldGen.PlaceTile(Main.spawnTileX + 1, Main.spawnTileY, TileID.GrayBrick, false, true);
            Main.tile[Main.spawnTileX - 1, Main.spawnTileY].slope(0);
            Main.tile[Main.spawnTileX, Main.spawnTileY].slope(0);
            Main.tile[Main.spawnTileX + 1, Main.spawnTileY].slope(0);
            WorldGen.PlaceTile(Main.spawnTileX, Main.spawnTileY - 1, ModContent.Find<ModTile>("Fargowiltas", "RegalStatueSheet"), false, true);*/

            bool TryPlacingStatue(int baseCheckX, int baseCheckY)
            {
                List<int> legalBlocks = new List<int> {
                    TileID.Stone,
                    TileID.Grass,
                    TileID.Dirt,
                    TileID.SnowBlock,
                    TileID.IceBlock,
                    TileID.ClayBlock,
                    TileID.Mud,
                    TileID.JungleGrass,
                    TileID.Sand
                };

                bool canPlaceStatueHere = true;
                for (int i = 0; i < 3; i++) //check no obstructing blocks
                    for (int j = 0; j < 4; j++)
                    {
                        Tile tile = Framing.GetTileSafely(baseCheckX + i, baseCheckY + j);
                        if (WorldGen.SolidOrSlopedTile(tile))
                        {
                            canPlaceStatueHere = false;
                            break;
                        }
                    }
                for (int i = 0; i < 3; i++) //check for solid foundation
                {
                    Tile tile = Framing.GetTileSafely(baseCheckX + i, baseCheckY + 4);
                    if (!WorldGen.SolidTile(tile) || !legalBlocks.Contains(tile.TileType))
                    {
                        canPlaceStatueHere = false;
                        break;
                    }
                }

                if (canPlaceStatueHere)
                {
                    for (int i = 0; i < 3; i++) //MAKE SURE nothing in the way
                        for (int j = 0; j < 4; j++)
                            WorldGen.KillTile(baseCheckX + i, baseCheckY + j);

                    WorldGen.PlaceTile(baseCheckX, baseCheckY + 4, TileID.GrayBrick, false, true);
                    WorldGen.PlaceTile(baseCheckX + 1, baseCheckY + 4, TileID.GrayBrick, false, true);
                    WorldGen.PlaceTile(baseCheckX + 2, baseCheckY + 4, TileID.GrayBrick, false, true);
                    Tile tile = Main.tile[baseCheckX, baseCheckY + 4]; tile.Slope = 0;
                    tile = Main.tile[baseCheckX + 1, baseCheckY + 4]; tile.Slope = 0;
                    tile = Main.tile[baseCheckX + 2, baseCheckY + 4]; tile.Slope = 0;
                    WorldGen.PlaceTile(baseCheckX + 1, baseCheckY + 3, ModContent.TileType<Tiles.MutantStatueGift>(), false, true);

                    return true;
                }

                return false;
            }

            int positionX = Main.spawnTileX - 1; //offset by dimensions of statue
            int positionY = Main.spawnTileY - 4;
            bool placed = false;
            for (int offsetX = -50; offsetX <= 50; offsetX++)
            {
                for (int offsetY = -30; offsetY <= 10; offsetY++)
                {
                    if (TryPlacingStatue(positionX + offsetX, positionY + offsetY))
                    {
                        placed = true;
                        break;
                    }
                }

                if (placed)
                    break;
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            base.UpdateUI(gameTime);

            FargowiltasSouls.UserInterfaceManager.UpdateUI(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            base.ModifyInterfaceLayers(layers);

            FargowiltasSouls.UserInterfaceManager.ModifyInterfaceLayers(layers);
        }

        string AnyItem(int id) => $"{Lang.misc[37]} {Lang.GetItemName(id)}";
        string AnyItem(string fargoSoulsLocalizationKey) => $"{Lang.misc[37]} {Language.GetTextValue($"Mods.FargowiltasSouls.RecipeGroups.{fargoSoulsLocalizationKey}")}";
        string ItemXOrY(int id1, int id2) => $"{Lang.GetItemName(id1)} {Language.GetTextValue($"Mods.FargowiltasSouls.RecipeGroups.Or")} {Lang.GetItemName(id2)}";

        public override void AddRecipeGroups()
        {
            RecipeGroup group;

            //drax
            group = new RecipeGroup(() => AnyItem(ItemID.Drax), ItemID.Drax, ItemID.PickaxeAxe);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyDrax", group);

            //dungeon enemies
            group = new RecipeGroup(() => AnyItem("BonesBanner"), ItemID.AngryBonesBanner, ItemID.BlueArmoredBonesBanner, ItemID.HellArmoredBonesBanner, ItemID.RustyArmoredBonesBanner);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyBonesBanner", group);

            //cobalt
            group = new RecipeGroup(() => AnyItem(ItemID.CobaltRepeater), ItemID.CobaltRepeater, ItemID.PalladiumRepeater);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyCobaltRepeater", group);

            //mythril
            group = new RecipeGroup(() => AnyItem(ItemID.MythrilRepeater), ItemID.MythrilRepeater, ItemID.OrichalcumRepeater);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyMythrilRepeater", group);

            //adamantite
            group = new RecipeGroup(() => AnyItem(ItemID.AdamantiteRepeater), ItemID.AdamantiteRepeater, ItemID.TitaniumRepeater);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyAdamantiteRepeater", group);

            //evil wood
            group = new RecipeGroup(() => ItemXOrY(ItemID.Ebonwood, ItemID.Shadewood), ItemID.Ebonwood, ItemID.Shadewood);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyEvilWood", group);

            //any adamantite
            group = new RecipeGroup(() => AnyItem(ItemID.AdamantiteBar), ItemID.AdamantiteBar, ItemID.TitaniumBar);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyAdamantite", group);

            //shroomite head
            group = new RecipeGroup(() => AnyItem(ItemID.ShroomiteHelmet), ItemID.ShroomiteHelmet, ItemID.ShroomiteMask, ItemID.ShroomiteHeadgear);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyShroomHead", group);

            //orichalcum head
            group = new RecipeGroup(() => AnyItem(ItemID.OrichalcumHelmet), ItemID.OrichalcumHelmet, ItemID.OrichalcumMask, ItemID.OrichalcumHeadgear);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyOriHead", group);

            //palladium head
            group = new RecipeGroup(() => AnyItem(ItemID.PalladiumHelmet), ItemID.PalladiumHelmet, ItemID.PalladiumMask, ItemID.PalladiumHeadgear);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyPallaHead", group);

            //cobalt head
            group = new RecipeGroup(() => AnyItem(ItemID.CobaltHelmet), ItemID.CobaltHelmet, ItemID.CobaltHat, ItemID.CobaltMask);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyCobaltHead", group);

            //mythril head
            group = new RecipeGroup(() => AnyItem(ItemID.MythrilHelmet), ItemID.MythrilHelmet, ItemID.MythrilHat, ItemID.MythrilHood);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyMythrilHead", group);

            //titanium head
            group = new RecipeGroup(() => AnyItem(ItemID.TitaniumHelmet), ItemID.TitaniumHelmet, ItemID.TitaniumMask, ItemID.TitaniumHeadgear);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyTitaHead", group);

            //hallowed head
            group = new RecipeGroup(() => AnyItem(ItemID.HallowedHelmet), ItemID.HallowedHelmet, ItemID.HallowedMask, ItemID.HallowedHeadgear, ItemID.HallowedHood);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyHallowHead", group);

            //ancient hallow
            group = new RecipeGroup(() => AnyItem(ItemID.AncientHallowedHelmet), ItemID.AncientHallowedHelmet, ItemID.AncientHallowedHeadgear, ItemID.AncientHallowedHood, ItemID.AncientHallowedMask);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyAncientHallowHead", group);

            //adamantite head
            group = new RecipeGroup(() => AnyItem(ItemID.AdamantiteHelmet), ItemID.AdamantiteHelmet, ItemID.AdamantiteMask, ItemID.AdamantiteHeadgear);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyAdamHead", group);

            //chloro head
            group = new RecipeGroup(() => AnyItem(ItemID.ChlorophyteHelmet), ItemID.ChlorophyteHelmet, ItemID.ChlorophyteMask, ItemID.ChlorophyteHeadgear);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyChloroHead", group);

            //spectre head
            group = new RecipeGroup(() => ItemXOrY(ItemID.SpectreHood, ItemID.SpectreMask), ItemID.SpectreHood, ItemID.SpectreMask);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnySpectreHead", group);

            //beetle body
            group = new RecipeGroup(() => ItemXOrY(ItemID.BeetleShell, ItemID.BeetleScaleMail), ItemID.BeetleShell, ItemID.BeetleScaleMail);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyBeetle", group);

            //            //phasesabers
            //            group = new RecipeGroup(() => AnyItem("Phasesaber"), ItemID.RedPhasesaber, ItemID.BluePhasesaber, ItemID.GreenPhasesaber, ItemID.PurplePhasesaber, ItemID.WhitePhasesaber,
            //                ItemID.YellowPhasesaber);
            //            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyPhasesaber", group);

            //            //vanilla butterflies
            //            group = new RecipeGroup(() => AnyItem("Butterfly"), ItemID.JuliaButterfly, ItemID.MonarchButterfly, ItemID.PurpleEmperorButterfly,
            //                ItemID.RedAdmiralButterfly, ItemID.SulphurButterfly, ItemID.TreeNymphButterfly, ItemID.UlyssesButterfly, ItemID.ZebraSwallowtailButterfly);
            //            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyButterfly", group);

            //vanilla squirrels
            group = new RecipeGroup(() => AnyItem(ItemID.Squirrel),
                ItemID.Squirrel,
                ItemID.SquirrelRed,
                ItemID.SquirrelGold,
                ItemID.GemSquirrelAmber,
                ItemID.GemSquirrelAmethyst,
                ItemID.GemSquirrelDiamond,
                ItemID.GemSquirrelEmerald,
                ItemID.GemSquirrelRuby,
                ItemID.GemSquirrelSapphire,
                ItemID.GemSquirrelTopaz,
                ModContent.ItemType<TopHatSquirrelCaught>(),
                ModContent.Find<ModItem>("Fargowiltas", "Squirrel").Type
            );
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnySquirrel", group);

            //            //vanilla fish
            //            group = new RecipeGroup(() => AnyItem("CommonFish"), ItemID.AtlanticCod, ItemID.Bass, ItemID.Trout, ItemID.RedSnapper, ItemID.Salmon, ItemID.Tuna);
            //            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyCommonFish", group);

            //vanilla birds
            group = new RecipeGroup(() => AnyItem(ItemID.Bird), ItemID.Bird, ItemID.BlueJay, ItemID.Cardinal, ItemID.GoldBird, ItemID.Duck, ItemID.MallardDuck, ItemID.Grebe, ItemID.Penguin, ItemID.Seagull);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyBird", group);

            //            //vanilla scorpions
            //            group = new RecipeGroup(() => AnyItem(ItemID.Scorpion), ItemID.Scorpion, ItemID.BlackScorpion);
            //            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyScorpion", group);

            //            //gold pick
            //            group = new RecipeGroup(() => AnyItem(ItemID.GoldPickaxe), ItemID.GoldPickaxe, ItemID.PlatinumPickaxe);
            //            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyGoldPickaxe", group);

            //            //fish trash
            //            group = new RecipeGroup(() => AnyItem("FishingTrash"), ItemID.OldShoe, ItemID.TinCan, ItemID.FishingSeaweed);
            //            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyFishingTrash", group);

            //vanilla rotten chunk/vertebrae
            group = new RecipeGroup(() => ItemXOrY(ItemID.RottenChunk, ItemID.Vertebrae), ItemID.RottenChunk, ItemID.Vertebrae);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyRottenChunk", group);

            //vanilla gold and plat ore
            group = new RecipeGroup(() => ItemXOrY(ItemID.GoldOre, ItemID.PlatinumOre), ItemID.GoldOre, ItemID.PlatinumOre);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyGoldOre", group);

            //vanilla gold and plat bar
            group = new RecipeGroup(() => ItemXOrY(ItemID.GoldBar, ItemID.PlatinumBar), ItemID.GoldBar, ItemID.PlatinumBar);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyGoldBar", group);

        }

        public override void PostAddRecipes()
        {
            foreach (Recipe recipe in Main.recipe.Where(recipe => recipe.requiredTile.Contains(TileID.Anvils) || recipe.requiredTile.Contains(TileID.MythrilAnvil)))
            {
                recipe.AddConsumeItemCallback(IronBonusBars);
            }
        }

        public static Recipe.ConsumeItemCallback IronBonusBars = (Recipe recipe, int type, ref int amount) => {

            Player player = Main.LocalPlayer;
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (modPlayer.IronEnchantItem == null) return;

            if (recipe.requiredTile.Contains(TileID.MythrilAnvil) && !modPlayer.TerraForce)
            {
                return;
            }

            //calc the new amount consumed (each has 33% of not)
            int amountUsed = 0;

            for (int i = 0; i < amount; i++)
            {
                if (!Main.rand.NextBool(3))
                    amountUsed++;
            }

            amount = amountUsed;
        };
    }
}
