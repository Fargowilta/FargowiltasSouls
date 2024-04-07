namespace FargowiltasSouls
{
	// Note: Obsolete: Entirely moved to hjson files


	/*
    public partial class FargowiltasSouls
    {
    /// <summary>
    /// DEPRECATED <para/>
    /// Input item as string (item class name) for FargoSouls items, and as int for vanilla items.
    /// </summary>
    public void AddToggle(string toggle, object item, string color = "ffffff") 
    {

        if (item.GetType() == typeof(int))
        {
            Language.GetOrRegister(toggle, () => $"[i:{item}] [c/{color}:{{$Mods.{Name}.Toggler.{toggle}}}]");
        }
        else if (item.GetType() == typeof(string))
        {
            Language.GetOrRegister(toggle, () => $"[i:FargowiltasSouls/{item}] [c/{color}:{{$Mods.{Name}.Toggler.{toggle}}}]");
        }
    }
    private void AddLocalizations()
    {
        #region helpers

        void Add(string key, string message)
        {
            Language.GetOrRegister(key, () => message);
        }

        void AddBossSpawnInfo(string bossName, string spawnInfo)
        {
            Add($"BossChecklist.{bossName}SpawnInfo", spawnInfo);
        }


        #endregion helpers

        #region pet toggles
        AddToggle("PetHeader", ItemID.ZephyrFish);
        AddToggle("PetDinoConfig", ItemID.AmberMosquito);
        AddToggle("PetEaterConfig", ItemID.EatersBone);
        AddToggle("PetFaceMonsterConfig", ItemID.BoneRattle);
        AddToggle("PetGrinchConfig", ItemID.BabyGrinchMischiefWhistle);
        AddToggle("PetHornetConfig", ItemID.Nectar);
        AddToggle("PetImpConfig", ItemID.HellCake);
        AddToggle("PetPenguinConfig", ItemID.Fish);
        AddToggle("PetPandaConfig", ItemID.BambooLeaf);
        AddToggle("PetDGConfig", ItemID.BoneKey);
        AddToggle("PetSnowmanConfig", ItemID.ToySled);
        AddToggle("PetShroomConfig", ItemID.StrangeGlowingMushroom);
        AddToggle("PetWerewolfConfig", ItemID.FullMoonSqueakyToy);
        AddToggle("PetBernieConfig", ItemID.BerniePetItem);
        AddToggle("PetBlackCatConfig", ItemID.UnluckyYarn);
        //AddToggle("PetBlueChickenConfig", ItemID.egg);
        //AddToggle("PetCavelingConfig", ItemID.glowt);
        AddToggle("PetChesterConfig", ItemID.ChesterPetItem);
        AddToggle("PetCompanionCubeConfig", ItemID.CompanionCube);
        AddToggle("PetCursedSaplingConfig", ItemID.CursedSapling);
        //public string PetDirt;
        AddToggle("PetKittenConfig", ItemID.BallOfFuseWire);
        AddToggle("PetEsteeConfig", ItemID.CelestialWand);
        AddToggle("PetEyeSpringConfig", ItemID.EyeSpring);
        AddToggle("PetFoxConfig", ItemID.ExoticEasternChewToy);
        AddToggle("PetButterflyConfig", ItemID.BedazzledNectar);
        AddToggle("PetGlommerConfig", ItemID.GlommerPetItem);
        AddToggle("PetDragonConfig", ItemID.DD2PetDragon);
        //AddToggle("PetJunimoConfig", ItemID.stardr);
        AddToggle("PetHarpyConfig", ItemID.BirdieRattle);
        AddToggle("PetLizardConfig", ItemID.LizardEgg);
        AddToggle("PetMinitaurConfig", ItemID.TartarSauce);
        AddToggle("PetParrotConfig", ItemID.ParrotCracker);
        AddToggle("PetPigmanConfig", ItemID.PigPetItem);
        AddToggle("PetPlanteroConfig", ItemID.MudBud);
        AddToggle("PetGatoConfig", ItemID.DD2PetGato);
        AddToggle("PetPupConfig", ItemID.DogWhistle);
        AddToggle("PetSeedConfig", ItemID.Seedling);
        AddToggle("PetSpiderConfig", ItemID.SpiderEgg);
        AddToggle("PetMimicConfig", ItemID.OrnateShadowKey);
        AddToggle("PetSharkConfig", ItemID.SharkBait);
        //AddToggle("PetSpiffoConfig", ItemID.spiff);
        AddToggle("PetSquashConfig", ItemID.MagicalPumpkinSeed);
        AddToggle("PetGliderConfig", ItemID.EucaluptusSap);
        AddToggle("PetTikiConfig", ItemID.TikiTotem);
        AddToggle("PetTurtleConfig", ItemID.Seaweed);
        AddToggle("PetVoltConfig", ItemID.LightningCarrot);
        AddToggle("PetZephyrConfig", ItemID.ZephyrFish);

        AddToggle("PetOrbConfig", ItemID.ShadowOrb);
        AddToggle("PetHeartConfig", ItemID.CrimsonHeart);
        AddToggle("PetLanternConfig", ItemID.MagicLantern);
        AddToggle("PetNaviConfig", ItemID.FairyBell);
        AddToggle("PetFlickerConfig", ItemID.DD2OgrePetItem);
        AddToggle("PetWispConfig", ItemID.WispinaBottle);
        AddToggle("PetSuspEyeConfig", ItemID.SuspiciousLookingTentacle);

        AddToggle("PetKSConfig", ItemID.KingSlimePetItem);
        AddToggle("PetEoCConfig", ItemID.EyeOfCthulhuPetItem);
        AddToggle("PetEoWConfig", ItemID.EaterOfWorldsPetItem);
        AddToggle("PetBoCConfig", ItemID.BrainOfCthulhuPetItem);
        AddToggle("PetDeerConfig", ItemID.DeerclopsPetItem);
        AddToggle("PetQBConfig", ItemID.QueenBeePetItem);
        AddToggle("PetSkeleConfig", ItemID.SkeletronPetItem);
        AddToggle("PetQSConfig", ItemID.QueenSlimePetItem);
        AddToggle("PetDestroyerConfig", ItemID.DestroyerPetItem);
        AddToggle("PetTwinsConfig", ItemID.TwinsPetItem);
        AddToggle("PetSkelePrimeConfig", ItemID.SkeletronPrimePetItem);

        AddToggle("PetOgreConfig", ItemID.DD2OgrePetItem);
        AddToggle("PetPlanteraConfig", ItemID.PlanteraPetItem);
        AddToggle("PetPumpkingConfig", ItemID.PumpkingPetItem);
        AddToggle("PetEverscreamConfig", ItemID.EverscreamPetItem);
        AddToggle("PetIceQueenConfig", ItemID.IceQueenPetItem);
        AddToggle("PetDukeConfig", ItemID.DukeFishronPetItem);
        AddToggle("PetGolemConfig", ItemID.GolemPetItem);
        AddToggle("PetEoLConfig", ItemID.FairyQueenPetItem);
        AddToggle("PetBetsyConfig", ItemID.DD2BetsyPetItem);
        AddToggle("PetMartianConfig", ItemID.MartianPetItem);
        AddToggle("PetLCConfig", ItemID.LunaticCultistPetItem);
        AddToggle("PetMLConfig", ItemID.MoonLordPetItem);


        #endregion pet toggles

        }
    }*/
}
