﻿using FargowiltasSouls.Items.Accessories.Forces;
using FargowiltasSouls.Items.Materials;
using FargowiltasSouls.Items.Pets;
using FargowiltasSouls.Items.Placeables.Trophies;
using FargowiltasSouls.Items.Summons;
using FargowiltasSouls.NPCs.AbomBoss;
using FargowiltasSouls.NPCs.Challengers;
using FargowiltasSouls.NPCs.Champions;
using FargowiltasSouls.NPCs.DeviBoss;
using FargowiltasSouls.NPCs.MutantBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls
{
    public partial class FargowiltasSouls
    {
        private void BossChecklistCompatibility()
        {
            if (ModLoader.TryGetMod("BossChecklist", out Mod bossChecklist))
            {
                Func<bool> AllPlayersAreDead = () => Main.player.All(plr => !plr.active || plr.dead);

                void Add(string type, string bossName, List<int> npcIDs, float progression, Func<bool> downed, Func<bool> available, List<int> collectibles, List<int> spawnItems, bool hasKilledAllMessage, string portrait = null)
                {
                    bossChecklist.Call(
                        $"Add{type}",
                        this,
                        $"$Mods.{Name}.NPCName.{bossName}",
                        npcIDs,
                        progression,
                        downed,
                        available,
                        collectibles,
                        spawnItems,
                        $"$Mods.{Name}.BossChecklist.{bossName}SpawnInfo",
                        hasKilledAllMessage ? new Func<NPC, string>(npc => AllPlayersAreDead() ? $"Mods.{Name}.BossChecklist.{bossName}KilledAll" : $"Mods.{Name}.BossChecklist.{bossName}Despawn") : $"Mods.{Name}.BossChecklist.{bossName}Despawn",
                        portrait == null ? null : new Action<SpriteBatch, Rectangle, Color>((spriteBatch, rect, color) =>
                        {
                            Texture2D tex = Assets.Request<Texture2D>(portrait, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                            Rectangle sourceRect = tex.Bounds;
                            float scale = Math.Min(1f, (float)rect.Width / sourceRect.Width);
                            spriteBatch.Draw(tex, rect.Center.ToVector2(), sourceRect, color, 0f, sourceRect.Size() / 2, scale, SpriteEffects.None, 0);
                        })
                    );
                }

                List<int> TryAddMusicBoxToCollectibles(string musicBoxName, params int[] items)
                {
                    List<int> collectibles = new List<int>(items);
                    if (ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod))
                        collectibles.Add(ModContent.Find<ModItem>("FargowiltasMusic", musicBoxName).Type);
                    return collectibles;
                }

                Add("Boss",
                    "DeviBoss",
                    new List<int> { ModContent.NPCType<DeviBoss>() },
                    6.9f,
                    () => FargoSoulsWorld.downedDevi,
                    () => true,
                    TryAddMusicBoxToCollectibles(
                        "DeviMusicBox",
                        ModContent.ItemType<DeviatingEnergy>(),
                        ModContent.ItemType<DeviTrophy>(),
                        ModContent.ItemType<ChibiHat>(),
                        ModContent.ItemType<BrokenBlade>()
                    ),
                    new List<int> { ModContent.ItemType<DevisCurse>() },
                    true
                );

                Add("Boss",
                    "AbomBoss",
                    new List<int> { ModContent.NPCType<AbomBoss>() },
                    20f,
                    () => FargoSoulsWorld.downedAbom,
                    () => true,
                    TryAddMusicBoxToCollectibles(
                        "AbomMusicBox",
                        ModContent.ItemType<AbomEnergy>(),
                        ModContent.ItemType<AbomTrophy>(),
                        ModContent.ItemType<BabyScythe>(),
                        ModContent.ItemType<BrokenHilt>()
                    ),
                    new List<int> { ModContent.ItemType<AbomsCurse>() },
                    true
                );

                Add("Boss",
                    "MutantBoss",
                    new List<int> { ModContent.NPCType<MutantBoss>() },
                    23f,
                    () => FargoSoulsWorld.downedMutant,
                    () => true,
                    TryAddMusicBoxToCollectibles(
                        "MutantMusicBox",
                        ModContent.ItemType<EternalEnergy>(),
                        ModContent.ItemType<MutantTrophy>(),
                        ModContent.ItemType<SpawnSack>(),
                        ModContent.ItemType<PhantasmalEnergy>()
                    ),
                    new List<int> { ModContent.ItemType<AbominationnVoodooDoll>() },
                    true
                );


                #region champions

                Add("MiniBoss",
                    "TimberChampion",
                    new List<int> { ModContent.NPCType<TimberChampion>() },
                    18.1f,
                    () => FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.TimberChampion],
                    () => true,
                    TryAddMusicBoxToCollectibles(
                        "ChampionMusicBox",
                        TimberForce.Enchants
                    ),
                    new List<int> { ModContent.ItemType<SigilOfChampions>() },
                    false
                );
                Add("MiniBoss",
                    "TerraChampion",
                    new List<int> { ModContent.NPCType<TerraChampion>(), ModContent.NPCType<TerraChampionBody>(), ModContent.NPCType<TerraChampionTail>() },
                    18.15f,
                    () => FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.TerraChampion],
                    () => true,
                    TryAddMusicBoxToCollectibles(
                        "ChampionMusicBox",
                        TerraForce.Enchants
                    ),
                    new List<int> { ModContent.ItemType<SigilOfChampions>() },
                    false,
                    "NPCs/Champions/TerraChampion_Still"
                );
                Add("MiniBoss",
                    "EarthChampion",
                    new List<int> { ModContent.NPCType<EarthChampion>(), ModContent.NPCType<EarthChampionHand>() },
                    18.2f,
                    () => FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.EarthChampion],
                    () => true,
                    TryAddMusicBoxToCollectibles(
                        "ChampionMusicBox",
                        EarthForce.Enchants
                    ),
                    new List<int> { ModContent.ItemType<SigilOfChampions>() },
                    false,
                    "NPCs/Champions/EarthChampion_Still"
                );
                Add("MiniBoss",
                    "NatureChampion",
                    new List<int> { ModContent.NPCType<NatureChampion>(), ModContent.NPCType<NatureChampionHead>() },
                    18.25f,
                    () => FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.NatureChampion],
                    () => true,
                    TryAddMusicBoxToCollectibles(
                        "ChampionMusicBox",
                        NatureForce.Enchants
                    ),
                    new List<int> { ModContent.ItemType<SigilOfChampions>() },
                    false,
                    "NPCs/Champions/NatureChampion_Still"
                );
                Add("MiniBoss",
                    "LifeChampion",
                    new List<int> { ModContent.NPCType<LifeChampion>() },
                    18.3f,
                    () => FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.LifeChampion],
                    () => true,
                    TryAddMusicBoxToCollectibles(
                        "ChampionMusicBox",
                        LifeForce.Enchants
                    ),
                    new List<int> { ModContent.ItemType<SigilOfChampions>() },
                    false,
                    "NPCs/Champions/LifeChampion_Still"
                );
                Add("MiniBoss",
                    "ShadowChampion",
                    new List<int> { ModContent.NPCType<ShadowChampion>() },
                    18.35f,
                    () => FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.ShadowChampion],
                    () => true,
                    TryAddMusicBoxToCollectibles(
                        "ChampionMusicBox",
                        ShadowForce.Enchants
                    ),
                    new List<int> { ModContent.ItemType<SigilOfChampions>() },
                    false
                );
                Add("MiniBoss",
                    "SpiritChampion",
                    new List<int> { ModContent.NPCType<SpiritChampion>(), ModContent.NPCType<SpiritChampionHand>() },
                    18.4f,
                    () => FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.SpiritChampion],
                    () => true,
                    TryAddMusicBoxToCollectibles(
                        "ChampionMusicBox",
                        SpiritForce.Enchants
                    ),
                    new List<int> { ModContent.ItemType<SigilOfChampions>() },
                    false,
                    "NPCs/Champions/SpiritChampion_Still"
                );
                Add("MiniBoss",
                    "WillChampion",
                    new List<int> { ModContent.NPCType<WillChampion>() },
                    18.45f,
                    () => FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.WillChampion],
                    () => true,
                    TryAddMusicBoxToCollectibles(
                        "ChampionMusicBox",
                        WillForce.Enchants
                    ),
                    new List<int> { ModContent.ItemType<SigilOfChampions>() },
                    false
                );

                Add("Boss",
                    "CosmosChampion",
                    new List<int> { ModContent.NPCType<CosmosChampion>() },
                    18.5f,
                    () => FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.CosmosChampion],
                    () => true,
                    TryAddMusicBoxToCollectibles(
                        "ChampionMusicBox",
                        CosmoForce.Enchants
                    ),
                    new List<int> { ModContent.ItemType<SigilOfChampions>() },
                    true
                );

                #endregion champions


                #region challengers

                Add("Boss",
                    "TrojanSquirrel",
                    new List<int> { ModContent.NPCType<TrojanSquirrel>() },
                    0.5f,
                    () => FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.TrojanSquirrel],
                    () => true,
                    //TryAddMusicBoxToCollectibles(
                    //    "DeviMusicBox",
                    //    ModContent.ItemType<DeviatingEnergy>(),
                    //    ModContent.ItemType<DeviTrophy>(),
                    //    ModContent.ItemType<ChibiHat>(),
                    //    ModContent.ItemType<BrokenBlade>()
                    //)
                    new List<int>(new int[]
                    {
                        ModContent.ItemType<TrojanSquirrelTrophy>()
                    }),
                    new List<int> { ModContent.ItemType<SquirrelCoatofArms>() },
                    false,
                    "NPCs/Challengers/TrojanSquirrel_Still"
                );
                Add("Boss",
                    "LifeChallenger",
                    new List<int> { ModContent.NPCType<LifeChallenger>() },
                    11.49f,
                    () => FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.LifeChallenger],
                    () => true,
                    //TryAddMusicBoxToCollectibles(
                    //    "DeviMusicBox",
                    //    ModContent.ItemType<DeviatingEnergy>(),
                    //    ModContent.ItemType<DeviTrophy>(),
                    //    ModContent.ItemType<ChibiHat>(),
                    //    ModContent.ItemType<BrokenBlade>()
                    //)
                    new List<int>(new int[]
                    {
                        //ModContent.ItemType<LifeChallengerTrophy>() ADD TROPHY
                    }),
                    new List<int> { ModContent.ItemType<FragilePixieLamp>() },
                    false,
                    "NPCs/Challengers/LifeChallenger_Still"
                );

                #endregion challengers
            }
        }
    }
}
