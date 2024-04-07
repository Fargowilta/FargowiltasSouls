using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Content.Items.Dyes;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Buffs;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Items.Weapons.SwarmDrops;
using static FargowiltasSouls.Core.Systems.DashManager;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Content.Items.Accessories.Masomode;

namespace FargowiltasSouls.Core.ModPlayers
{
    public partial class FargoSoulsPlayer : ModPlayer
    {
        public ToggleBackend Toggler = new();

        public Dictionary<AccessoryEffect, bool> TogglesToSync = new();

        public List<AccessoryEffect> disabledToggles = new();

        public List<BaseEnchant> EquippedEnchants = new();


        public bool IsStandingStill;
        public float AttackSpeed;
        public float WingTimeModifier = 1f;

        public bool FreeEaterSummon = true;
        public int Screenshake;

        public bool RustRifleReloading = false;
        public float RustRifleReloadZonePos = 0;
        public float RustRifleTimer = 0;

        public int RockeaterDistance = EaterLauncher.BaseDistance;

        public bool fireNoDamage = false;

        public int The22Incident;

        public Dictionary<int, bool> KnownBuffsToPurify = new();


        public bool IsStillHoldingInSameDirectionAsMovement
            => (Player.velocity.X > 0 && Player.controlRight)
            || (Player.velocity.X < 0 && Player.controlLeft)
            || Player.dashDelay < 0
            || IsDashingTimer > 0;

        //grapple check needed because grapple state extends dash state forever
        public bool IsInADashState
            => (Player.dashDelay == -1 || IsDashingTimer > 0) && Player.grapCount <= 0;

        public override void SaveData(TagCompound tag)
        {
            var playerData = new List<string>();
            if (MutantsPactSlot) playerData.Add("MutantsPactSlot");
            if (MutantsDiscountCard) playerData.Add("MutantsDiscountCard");
            if (MutantsCreditCard) playerData.Add("MutantsCreditCard");
            if (ReceivedMasoGift) playerData.Add("ReceivedMasoGift");
            if (RabiesVaccine) playerData.Add("RabiesVaccine");
            if (DeerSinew) playerData.Add("DeerSinew");
            if (HasClickedWrench) playerData.Add("HasClickedWrench");


            tag.Add($"{Mod.Name}.{Player.name}.Data", playerData);

            var togglesOff = new List<string>();
            if (Toggler != null && Toggler.Toggles != null)
            {
                foreach (KeyValuePair<AccessoryEffect, Toggle> entry in Toggler.Toggles)
                {
                    if (!Toggler.Toggles[entry.Key].ToggleBool)
                        togglesOff.Add(entry.Key.Name);
                }
            }
            tag.Add($"{Mod.Name}.{Player.name}.TogglesOff", togglesOff);
            Toggler.Save();
        }

        public override void LoadData(TagCompound tag)
        {
            var playerData = tag.GetList<string>($"{Mod.Name}.{Player.name}.Data");
            MutantsPactSlot = playerData.Contains("MutantsPactSlot");
            MutantsDiscountCard = playerData.Contains("MutantsDiscountCard");
            MutantsCreditCard = playerData.Contains("MutantsCreditCard");
            ReceivedMasoGift = playerData.Contains("ReceivedMasoGift");
            RabiesVaccine = playerData.Contains("RabiesVaccine");
            DeerSinew = playerData.Contains("DeerSinew");
            HasClickedWrench = playerData.Contains("HasClickedWrench");

            List<string> disabledToggleNames = tag.GetList<string>($"{Mod.Name}.{Player.name}.TogglesOff").ToList();
            disabledToggles = ToggleLoader.LoadedToggles.Keys.Where(x => disabledToggleNames.Contains(x.Name)).ToList();
        }
        public override void OnEnterWorld()
        {
            Toggler.TryLoad();
            Toggler.LoadPlayerToggles(this);
            disabledToggles.Clear();

            if (!ModLoader.TryGetMod("FargowiltasMusic", out Mod _))
            {
                Main.NewText(Language.GetTextValue($"Mods.{Mod.Name}.Message.NoMusic1"), Color.LimeGreen);
                Main.NewText(Language.GetTextValue($"Mods.{Mod.Name}.Message.NoMusic2"), Color.LimeGreen);
            }
            if (!ModLoader.TryGetMod("FargowiltasCrossmod", out Mod soulsDLC))
            {
                List<string> supportedMods = new();
                if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
                {
                    supportedMods.Add(calamity.DisplayName);
                }
                if (ModLoader.TryGetMod("NoxusBoss", out Mod WotG))
                {
                    supportedMods.Add(WotG.DisplayName);
                }
                if (supportedMods.Count > 0)
                {
                    string modsString = "";
                    for (int i = 0; i < supportedMods.Count; i++)
                    {
                        modsString += supportedMods[i];
                        if (i + 2 < supportedMods.Count)
                        {
                            modsString += ", ";
                        }
                        else if (i + 1 < supportedMods.Count)
                        {
                            modsString += " and ";
                        }
                    }
                    Main.NewText(Language.GetTextValue($"Mods.{Mod.Name}.Message.NoDLC1", modsString), Color.Green);
                    Main.NewText(Language.GetTextValue($"Mods.{Mod.Name}.Message.NoDLC2"), Color.Green);
                }
            }

            Main.NewText(Language.GetTextValue($"Mods.{Mod.Name}.Message.Wiki"), Color.Lime);

            if (Toggler.CanPlayMaso)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    WorldSavingSystem.CanPlayMaso = true;
                }
                else if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    //Main.NewText("send it");
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)FargowiltasSouls.PacketID.SyncCanPlayMaso);
                    packet.Write(Toggler.CanPlayMaso);
                    packet.Send();
                }
            }

        }

        public override void ResetEffects()
        {
            HasDash = false;
            FargoDash = DashType.None;

            AttackSpeed = 1f;
            if (Screenshake > 0)
                Screenshake--;

            if (NoUsingItems > 0)
                NoUsingItems--;

            //            Wood = false;

            WingTimeModifier = 1f;


            QueenStingerItem = null;
            EridanusSet = false;
            GaiaSet = false;
            StyxSet = false;
            if (StyxAttackReadyTimer > 0)
                StyxAttackReadyTimer--;
            NekomiSet = false;
            if (NekomiHitCD > 0)
                NekomiHitCD--;
            if (NekomiAttackReadyTimer > 0)
                NekomiAttackReadyTimer--;

            BrainMinion = false;
            EaterMinion = false;
            BigBrainMinion = false;
            DukeFishron = false;

            SquirrelMount = false;

            SeekerOfAncientTreasures = false;
            AccursedSarcophagus = false;
            BabyLifelight = false;
            BabySilhouette = false;
            BiteSizeBaron = false;
            Nibble = false;
            ChibiDevi = false;
            MutantSpawn = false;
            BabyAbom = false;

            //            #region enchantments 
            PetsActive = true;
            //CrimsonRegen = false;
            MinionCrits = false;
            FirstStrike = false;
            ShellHide = false;
            GoldShell = false;
            LavaWet = false;

            WoodEnchantItem = null;
            WoodEnchantDiscount = false;
            fireNoDamage = false;

            SnowVisual = false;
            ApprenticeEnchantActive = false;
            DarkArtistEnchantActive = false;
            CrystalEnchantActive = false;
            IronRecipes = false;
            ChlorophyteEnchantActive = false;
            PearlwoodStar = false;

            if (!MonkEnchantActive)
                Player.ClearBuff(ModContent.BuffType<MonkBuff>());
            MonkEnchantActive = false;
            ShinobiEnchantActive = false;
            PlatinumEffect = null;
            AncientShadowEnchantActive = false;
            SquireEnchantActive = false;
            ValhallaEnchantActive = false;
            TitaniumDRBuff = false;
            TitaniumCD = false;

            CactusImmune = false;


            //            #endregion

            //souls
            MeleeSoul = false;
            MagicSoul = false;
            RangedSoul = false;
            SummonSoul = false;
            ColossusSoul = false;
            SupersonicSoul = false;
            WorldShaperSoul = false;
            FlightMasterySoul = false;
            RangedEssence = false;
            BuilderMode = false;
            UniverseSoul = false;
            UniverseCore = false;
            FishSoul1 = false;
            FishSoul2 = false;
            TerrariaSoul = false;
            VoidSoul = false;
            Eternity = false;

            PrimeSoulItemCount = 0;
            if (!PrimeSoulActiveBuffer)
            {
                PrimeSoulActive = false;
            }
            PrimeSoulActiveBuffer = false;

            if (ForceEffects != null)
                ForceEffects.Clear();

            //maso
            SlimyShieldItem = null;
            DarkenedHeartItem = null;
            NecromanticBrewItem = null;
            DeerSinewNerf = false;
            PureHeart = false;
            PungentEyeballMinion = false;
            CrystalSkullMinion = false;
            FusedLens = false;
            FusedLensCanDebuff = false;
            Supercharged = false;
            Probes = false;
            MagicalBulb = false;
            PlanterasChild = false;
            SkullCharm = false;
            PungentEyeball = false;
            LumpOfFlesh = false;
            LihzahrdTreasureBoxItem = null;
            BetsysHeartItem = null;
            BetsyDashing = false;
            MutantAntibodies = false;
            GravityGlobeEXItem = null;
            MoonChalice = false;
            LunarCultist = false;
            TrueEyes = false;
            AbomWandItem = null;
            MasochistSoul = false;
            MasochistSoulItem = null;
            MasochistHeart = false;
            SandsofTime = false;
            SecurityWallet = false;
            FrigidGemstoneItem = null;
            NymphsPerfume = false;
            NymphsPerfumeRespawn = false;
            RainbowSlime = false;
            SkeletronArms = false;
            IceQueensCrown = false;
            CirnoGraze = false;
            MiniSaucer = false;
            CanAmmoCycle = false;
            TribalCharm = false;
            TribalCharmEquipped = false;
            SupremeDeathbringerFairy = false;
            GodEaterImbue = false;
            MutantSetBonusItem = null;
            AbomMinion = false;
            PhantasmalRing = false;
            TwinsEX = false;
            TimsConcoction = false;
            DeviGraze = false;
            Graze = false;
            GrazeRadius = 100f;
            DevianttHeartItem = null;
            MutantEyeItem = null;
            MutantEyeVisual = false;
            AbomRebirth = false;
            WasHurtBySomething = false;
            PrecisionSeal = false;
            GelicWingsItem = null;
            ConcentratedRainbowMatter = false;

            //debuffs
            Hexed = false;
            Unstable = false;
            Fused = false;
            Shadowflame = false;
            Oiled = false;
            Slimed = false;
            noDodge = false;
            noSupersonic = false;
            NoMomentum = false;
            Bloodthirsty = false;
            DisruptedFocus = false;

            Smite = false;
            Anticoagulation = false;
            GodEater = false;
            FlamesoftheUniverse = false;
            MutantNibble = false;
            Asocial = false;
            Kneecapped = false;
            Defenseless = false;
            Purified = false;
            Infested = false;
            Rotting = false;
            SqueakyToy = false;
            Atrophied = false;
            Jammed = false;
            ReverseManaFlow = false;
            CurseoftheMoon = false;
            OceanicMaul = false;
            DeathMarked = false;
            Hypothermia = false;
            Midas = false;
            if (MutantPresence == false)
                PresenceTogglerTimer = 0;
            MutantPresence = MutantPresence && Player.HasBuff(ModContent.BuffType<MutantPresenceBuff>());
            HadMutantPresence = MutantPresence;
            MutantFang = false;
            DevianttPresence = false;
            Swarming = false;
            LowGround = false;
            Flipped = false;
            LihzahrdCurse = false;
            //LihzahrdBlessing = false;
            Berserked = false;
            CerebralMindbreak = false;
            NanoInjection = false;
            Stunned = false;
            HasJungleRose = false;
            HaveCheckedAttackSpeed = false;
            BoxofGizmos = false;
            OxygenTank = false;
            //IronEnchantShield = false;
            WizardedItem = null;

            EquippedEnchants.Clear();

            WizardTooltips = false;

            if (WizardEnchantActive)
            {
                WizardEnchantActive = false;
                for (int i = 3; i <= 9; i++)
                {
                    if (!Player.armor[i].IsAir && (Player.armor[i].type == ModContent.ItemType<WizardEnchant>() || Player.armor[i].type == ModContent.ItemType<CosmoForce>()))
                    {
                        WizardEnchantActive = true;
                        Item ench = Player.armor[i + 1];
                        if (ench != null && !ench.IsAir && ench.ModItem != null && ench.ModItem is BaseEnchant)
                        {
                            WizardedItem = ench;
                        }
                        break;
                    }
                }
            }

            if (!Mash && MashCounter > 0)
                MashCounter--;
            Mash = false;

        }
        public override void OnRespawn()
        {
            if (NymphsPerfumeRespawn)
                NymphsPerfumeRestoreLife = 6;
        }
        public override void UpdateDead()
        {
            bool wasSandsOfTime = SandsofTime;
            bool wasNymphsPerfumeRespawn = NymphsPerfumeRespawn;

            ResetEffects();

            SandsofTime = wasSandsOfTime;
            NymphsPerfumeRespawn = wasNymphsPerfumeRespawn;

            if (SandsofTime && !LumUtils.AnyBosses() && Player.respawnTimer > 10)
                Player.respawnTimer -= Eternity ? 6 : 1;

            //maso disables respawning during mp boss
            /*if (WorldSavingSystem.MasochistModeReal && LumUtils.AnyBosses())
            {
                if (Player.respawnTimer < 10)
                    Player.respawnTimer = 10;

                if (Main.netMode == NetmodeID.MultiplayerClient && Main.npc[FargoSoulsGlobalNPC.boss].HasValidTarget && Main.npc[FargoSoulsGlobalNPC.boss].HasPlayerTarget)
                    Player.Center = Main.player[Main.npc[FargoSoulsGlobalNPC.boss].target].Center;
            }*/

            ReallyAwfulDebuffCooldown = 0;
            ParryDebuffImmuneTime = 0;
            WingTimeModifier = 1f;
            FreeEaterSummon = true;

            AbominableWandRevived = false;

            EridanusTimer = 0;
            StyxMeter = 0;
            StyxTimer = 0;
            StyxAttackReadyTimer = 0;
            NekomiMeter = 0;
            NekomiTimer = 0;
            NekomiAttackReadyTimer = 0;

            CirnoGrazeCounter = 0;

            //debuffs
            unstableCD = 0;
            lightningRodTimer = 0;

            BuilderMode = false;
            NoUsingItems = 0;

            FreezeTime = false;
            freezeLength = 0;

            ChillSnowstorm = false;
            chillLength = 0;

            SlimyShieldFalling = false;
            DarkenedHeartCD = 60;
            GuttedHeartCD = 60;
            IsDashingTimer = 0;
            GroundPound = 0;
            NymphsPerfumeCD = 30;
            WretchedPouchCD = 0;


            DeviGrazeBonus = 0;
            MutantEyeCD = 60;

            MythrilTimer = MythrilMaxTime;
            BeetleEnchantDefenseTimer = 0;

            Mash = false;
            WizardEnchantActive = false;
            MashCounter = 0;

            MaxLifeReduction = 0;
            CurrentLifeReduction = 0;

            The22Incident = 0;
        }



        public override void ModifyLuck(ref float luck)
        {
            if (Unlucky)
                luck -= 1.0f;

            Unlucky = false;
        }

        internal List<int> prevDyes = null;

        public void ManageLifeReduction()
        {
            if (OceanicMaul && LifeReductionUpdateTimer <= 0)
                LifeReductionUpdateTimer = 1; //trigger life reduction behaviour

            if (LifeReductionUpdateTimer > 0)
            {
                const int threshold = 30;
                if (LifeReductionUpdateTimer++ > threshold)
                {
                    LifeReductionUpdateTimer = 1;

                    if (OceanicMaul) //with maul, real max life gradually decreases to the desired point
                    {
                        if (MutantFang) //update faster
                            LifeReductionUpdateTimer = threshold - 10;

                        int newLifeReduction = CurrentLifeReduction + 5;
                        if (newLifeReduction > MaxLifeReduction)
                            newLifeReduction = MaxLifeReduction;
                        if (newLifeReduction > Player.statLifeMax2 - 100) //i.e. max life wont go below 100
                            newLifeReduction = Player.statLifeMax2 - 100;

                        if (CurrentLifeReduction < newLifeReduction)
                        {
                            CurrentLifeReduction = newLifeReduction;
                            CombatText.NewText(Player.Hitbox, Color.DarkRed, Language.GetTextValue($"Mods.{Mod.Name}.Buffs.OceanicMaulBuff.LifeDown"));
                        }
                    }
                    else //after maul wears off, real max life gradually recovers to normal value
                    {
                        CurrentLifeReduction -= 5;
                        if (MaxLifeReduction > CurrentLifeReduction)
                            MaxLifeReduction = CurrentLifeReduction;
                        CombatText.NewText(Player.Hitbox, Color.DarkGreen, Language.GetTextValue($"Mods.{Mod.Name}.Buffs.OceanicMaulBuff.LifeUp"));
                    }
                }
            }

            if (CurrentLifeReduction > 0)
            {
                if (CurrentLifeReduction > Player.statLifeMax2 - 100) //i.e. max life wont go below 100
                    CurrentLifeReduction = Player.statLifeMax2 - 100;
                Player.statLifeMax2 -= CurrentLifeReduction;
                //if (Player.statLife > Player.statLifeMax2) Player.statLife = Player.statLifeMax2;
            }
            else if (!OceanicMaul) //deactivate behaviour
            {
                CurrentLifeReduction = 0;
                MaxLifeReduction = 0;
                LifeReductionUpdateTimer = 0;
            }
        }

        public override float UseSpeedMultiplier(Item item)
        {
            int useTime = item.useTime;
            int useAnimate = item.useAnimation;

            if (useTime <= 0 || useAnimate <= 0 || item.damage <= 0)
                return base.UseSpeedMultiplier(item);

            if (!HaveCheckedAttackSpeed)
            {
                HaveCheckedAttackSpeed = true;

                AttackSpeed += Player.AccessoryEffects().ModifyUseSpeed(item);

                if (Berserked)
                {
                    AttackSpeed += .1f;
                }

                if (MagicSoul && item.CountsAsClass(DamageClass.Magic))
                {
                    AttackSpeed += .2f;
                }

                if (Player.HasEffect<MythrilEffect>())
                {
                    MythrilEffect.CalcMythrilAttackSpeed(this, item);
                }

                if (Player.HasEffect<WretchedPouchEffect>() && !MasochistSoul && AttackSpeed > 1f)
                {
                    float diff = AttackSpeed - 1f;
                    diff /= 2;
                    AttackSpeed -= diff;
                }

                //modify attack speed so it rounds up
                //int useTimeRoundUp = (int)Math.Round(useTime / AttackSpeed, MidpointRounding.ToPositiveInfinity);
                //if (useTimeRoundUp < useTime) //sanity check
                //{
                //    while (useTime / AttackSpeed < useTimeRoundUp)
                //    {
                //        AttackSpeed -= .01f; //small increments to avoid skipping past any integers
                //    }
                //}

                //checks so weapons dont break
                while (useTime / AttackSpeed < 1)
                {
                    AttackSpeed -= .01f;
                }

                while (useAnimate / AttackSpeed < 3)
                {
                    AttackSpeed -= .01f;
                }

                if (AttackSpeed < .1f)
                    AttackSpeed = .1f;
            }

            return AttackSpeed;
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            //            if (squireReduceIframes && (SquireEnchant || ValhallaEnchant))
            //            {
            //                if (Main.rand.NextBool(3))
            //                {
            //                    float scale = ValhallaEnchant ? 2f : 1.5f;
            //                    int type = ValhallaEnchant ? 87 : 91;
            //                    int dust = Dust.NewDust(Player.position, Player.width, Player.height, type, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 87, default(Color), scale);
            //                    Main.dust[dust].noGravity = true;
            //                    Main.dust[dust].velocity *= 1.8f;
            //                    Main.dust[dust].velocity.Y -= 0.5f;
            //                    if (Main.rand.NextBool(4))
            //                    {
            //                        Main.dust[dust].noGravity = false;
            //                        Main.dust[dust].scale *= 0.5f;
            //                    }
            //                    Main.PlayerDrawDust.Add(dust);
            //                }
            //                fullBright = true;
            //            }

            if (Shadowflame)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(Player.position - new Vector2(2f, 2f), Player.width, Player.height, DustID.Shadowflame, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 2f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    drawInfo.DustCache.Add(dust);
                }
                fullBright = true;
            }

            if (Rotting)
            {
                if (drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(Player.position - new Vector2(2f, 2f), Player.width, Player.height, DustID.Blood, Player.velocity.X * 0.1f, Player.velocity.Y * 0.1f, 0, default, 2f);
                    Main.dust[dust].noGravity = Main.rand.NextBool();
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    drawInfo.DustCache.Add(dust);
                }
            }

            if (Purified)
            {
                if (drawInfo.shadow == 0f)
                {
                    int index2 = Dust.NewDust(Player.position, Player.width, Player.height, DustID.GemDiamond, 0.0f, 0.0f, 100, default, 2.5f);
                    Main.dust[index2].velocity *= 2f;
                    Main.dust[index2].noGravity = true;
                    drawInfo.DustCache.Add(index2);
                }
            }

            if (Smite)
            {
                if (drawInfo.shadow == 0f)
                {
                    Color color = Main.DiscoColor;
                    int index2 = Dust.NewDust(Player.position, Player.width, Player.height, DustID.GemDiamond, 0.0f, 0.0f, 100, color, 2.5f);
                    Main.dust[index2].velocity *= 2f;
                    Main.dust[index2].noGravity = true;
                    drawInfo.DustCache.Add(index2);
                }
            }

            if (Anticoagulation)
            {
                if (drawInfo.shadow == 0f)
                {
                    int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Blood);
                    Main.dust[d].velocity *= 2f;
                    Main.dust[d].scale += 1f;
                }
            }

            if (Hexed)
            {
                if (Main.rand.NextBool(3) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(Player.position - new Vector2(2f, 2f), Player.width, Player.height, DustID.BubbleBlock, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 2.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 2f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.dust[dust].color = Color.GreenYellow;
                    drawInfo.DustCache.Add(dust);
                }
                if (Main.rand.NextBool() && drawInfo.shadow == 0f)
                {
                    int index2 = Dust.NewDust(Player.position, Player.width, Player.height, DustID.RuneWizard, 0.0f, 0.0f, 100, default, 2.5f);
                    Main.dust[index2].noGravity = true;
                    drawInfo.DustCache.Add(index2);
                }
                fullBright = true;
            }

            if (Infested)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(Player.position - new Vector2(2f, 2f), Player.width, Player.height, DustID.JungleSpore, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, InfestedDust);
                    Main.dust[dust].noGravity = true;
                    //Main.dust[dust].velocity *= 1.8f;
                    // Main.dust[dust].velocity.Y -= 0.5f;
                    drawInfo.DustCache.Add(dust);
                }
                fullBright = true;
            }

            if (CurrentLifeReduction > 0)
            {
                if (Main.rand.NextBool() && drawInfo.shadow == 0f)
                {
                    int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Blood);
                    Main.dust[d].velocity *= 2f;
                    Main.dust[d].scale += 1f;
                    drawInfo.DustCache.Add(d);
                }
            }

            if (GodEater)
            {
                if (Main.rand.NextBool(3) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(Player.position - new Vector2(2f, 2f), Player.width + 4, Player.height + 4, DustID.GemAmethyst, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.2f;
                    Main.dust[dust].velocity.Y -= 0.15f;
                    drawInfo.DustCache.Add(dust);
                }
                r *= 0.15f;
                g *= 0.03f;
                b *= 0.09f;
                fullBright = true;
            }

            if (FlamesoftheUniverse)
            {
                /*drawInfo.drawPlayer.onFire = true;
                drawInfo.drawPlayer.onFire2 = true;
                drawInfo.drawPlayer.onFrostBurn = true;
                drawInfo.drawPlayer.ichor = true;
                drawInfo.drawPlayer.burned = true;*/
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.VilePowder, Player.velocity.X * 0.2f, Player.velocity.Y * 0.2f, 100, new Color(50 * Main.rand.Next(6) + 5, 50 * Main.rand.Next(6) + 5, 50 * Main.rand.Next(6) + 5), 2.5f);
                    Main.dust[d].velocity.Y -= 1;
                    Main.dust[d].velocity *= 2f;
                    Main.dust[d].noGravity = true;
                    drawInfo.DustCache.Add(d);
                }
                fullBright = true;
            }

            if (CurseoftheMoon)
            {
                if (Main.rand.NextBool(5))
                {
                    int d = Dust.NewDust(Player.Center, 0, 0, DustID.Vortex, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 3f;
                    drawInfo.DustCache.Add(d);
                }
                if (Main.rand.NextBool(5))
                {
                    int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Vortex, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity.Y -= 1f;
                    Main.dust[d].velocity *= 2f;
                    drawInfo.DustCache.Add(d);
                }
            }

            if (DeathMarked)
            {
                if (Main.rand.NextBool() && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(Player.position - new Vector2(2f, 2f), Player.width, Player.height, DustID.Asphalt, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 0, default, 1.5f);
                    Main.dust[dust].velocity.Y--;
                    if (!Main.rand.NextBool(3))
                    {
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].scale += 0.5f;
                        Main.dust[dust].velocity *= 3f;
                        Main.dust[dust].velocity.Y -= 0.5f;
                    }
                    drawInfo.DustCache.Add(dust);
                }
                r *= 0.2f;
                g *= 0.2f;
                b *= 0.2f;
                fullBright = true;
            }

            if (Fused)
            {
                if (Main.rand.NextBool() && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(Player.position + new Vector2(Player.width / 2, Player.height / 5), 0, 0, DustID.Torch, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 0, default, 2f);
                    Main.dust[dust].velocity.Y -= 2f;
                    Main.dust[dust].velocity *= 2f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].scale += 0.5f;
                        Main.dust[dust].noGravity = true;
                    }
                    drawInfo.DustCache.Add(dust);
                }
            }

            if (Supercharged)
            {
                if (Main.rand.NextBool() && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Vortex, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f);
                    Main.dust[dust].scale += 0.5f;
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    if (Main.rand.NextBool(3))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }
            }


        }
        public void ConcentratedRainbowMatterTryAutoHeal()
        {
            if (ConcentratedRainbowMatter
                && Player.statLife < Player.statLifeMax2
                && Player.potionDelay <= 0
                && Player.HasEffect<RainbowHealEffect>()
                && !MutantNibble)
            {
                Item potion = Player.QuickHeal_GetItemToUse();
                if (potion != null)
                {
                    int heal = GetHealMultiplier(potion.healLife);
                    if (Player.statLife < Player.statLifeMax2 - heal && //only heal when full benefit (no wasted overheal)
                        (Player.statLife < Player.statLifeMax2 * 0.4 && //heal when very low AND when danger nearby (not after respawn in safety)
                        Main.npc.Any(n => n.active && n.damage > 0 && !n.friendly
                                     && Player.Distance(n.Center) < 1200 && (n.noTileCollide || Collision.CanHitLine(Player.Center, 0, 0, n.Center, 0, 0)))))
                    {
                        Player.QuickHeal();
                    }
                }
            }
        }


        private PlayerDeathReason DeathByLocalization(string key)
        {
            return PlayerDeathReason.ByCustomReason(Language.GetTextValue($"Mods.FargowiltasSouls.DeathMessage.{key}", Player.name));
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            bool retVal = true;

            if (Player.statLife <= 0) //revives
            {
                if (Player.whoAmI == Main.myPlayer && retVal && AbomRebirth)
                {
                    if (!WasHurtBySomething)
                    {
                        Player.statLife = 1;
                        return false; //short circuits the rest, this is deliberate
                    }
                }

                if (Player.whoAmI == Main.myPlayer && retVal && MutantSetBonusItem != null && Player.FindBuffIndex(ModContent.BuffType<MutantRebirthBuff>()) == -1)
                {
                    TryCleanseDebuffs();
                    Player.statLife = Player.statLifeMax2;
                    Player.HealEffect(Player.statLifeMax2);
                    Player.immune = true;
                    Player.immuneTime = 180;
                    Player.hurtCooldowns[0] = 180;
                    Player.hurtCooldowns[1] = 180;
                    string text = Language.GetTextValue($"Mods.{Mod.Name}.Message.Revived");
                    Main.NewText(text, Color.LimeGreen);
                    Player.AddBuff(ModContent.BuffType<MutantRebirthBuff>(), LumUtils.SecondsToFrames(120f));
                    retVal = false;

                    Projectile.NewProjectile(Player.GetSource_Accessory(MutantSetBonusItem), Player.Center, -Vector2.UnitY, ModContent.ProjectileType<GiantDeathray>(), (int)(7000 * Player.ActualClassDamage(DamageClass.Magic)), 10f, Player.whoAmI);
                }

                if (Player.whoAmI == Main.myPlayer && retVal && AbomWandItem != null && !AbominableWandRevived)
                {
                    AbominableWandRevived = true;
                    int heal = 1;
                    Player.statLife = heal;
                    Player.HealEffect(heal);
                    Player.immune = true;
                    Player.immuneTime = 120;
                    Player.hurtCooldowns[0] = 120;
                    Player.hurtCooldowns[1] = 120;
                    string text = Language.GetTextValue($"Mods.{Mod.Name}.Message.Revived");
                    CombatText.NewText(Player.Hitbox, Color.Yellow, text, true);
                    Main.NewText(text, Color.Yellow);
                    Player.AddBuff(ModContent.BuffType<AbomRebirthBuff>(), 900);
                    retVal = false;
                    for (int i = 0; i < 24; i++)
                    {
                        Projectile.NewProjectile(Player.GetSource_Accessory(AbomWandItem), Player.Center, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(4f, 16f),
                            ModContent.ProjectileType<StyxArmorScythe2>(), 0, 10f, Main.myPlayer, -60 - Main.rand.Next(60), -1);
                    }
                }
            }

            //killed by damage over time
            if (damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                if (Infested)
                    damageSource = DeathByLocalization("Infested");

                if (Anticoagulation)
                    damageSource = DeathByLocalization("Anticoagulation");

                if (Rotting)
                    damageSource = DeathByLocalization("Rotting");

                if (Shadowflame)
                    damageSource = DeathByLocalization("Shadowflame");

                if (NanoInjection)
                    damageSource = DeathByLocalization("NanoInjection");

                if (GodEater || FlamesoftheUniverse || CurseoftheMoon || MutantFang)
                    damageSource = DeathByLocalization("DivineWrath");
            }

            /*if (MutantPresence)
            {
                damageSource = PlayerDeathReason.ByCustomReason(Player.name + " was penetrated.");
            }*/

            if (StatLifePrevious > 0 && Player.statLife > StatLifePrevious)
                StatLifePrevious = Player.statLife;

            return retVal;
        }

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            if (GaiaOffense) //set armor and accessory shaders to gaia shader if set bonus is triggered
            {
                int gaiaShader = GameShaders.Armor.GetShaderIdFromItemId(ModContent.ItemType<GaiaDye>());
                drawInfo.cBody = gaiaShader;
                drawInfo.cHead = gaiaShader;
                drawInfo.cLegs = gaiaShader;
                drawInfo.cWings = gaiaShader;
                drawInfo.cHandOn = gaiaShader;
                drawInfo.cHandOff = gaiaShader;
                drawInfo.cShoe = gaiaShader;
            }

            if (GuardRaised)
            {
                Player.bodyFrame.Y = Player.bodyFrame.Height * 10;
                if (shieldTimer > 0)
                {
                    List<int> shaders = new()
                    {
                        GameShaders.Armor.GetShaderIdFromItemId(ItemID.ReflectiveSilverDye)
                    };
                    if (Player.HasEffect<DreadShellEffect>())
                        shaders.Add(GameShaders.Armor.GetShaderIdFromItemId(ItemID.BloodbathDye));
                    if (Player.HasEffect<PumpkingsCapeEffect>())
                        shaders.Add(GameShaders.Armor.GetShaderIdFromItemId(ItemID.PixieDye));

                    int shader = shaders[(int)(Main.GameUpdateCount / 4 % shaders.Count)];
                    drawInfo.cBody = shader;
                    drawInfo.cHead = shader;
                    drawInfo.cLegs = shader;
                    drawInfo.cWings = shader;
                    drawInfo.cHandOn = shader;
                    drawInfo.cHandOff = shader;
                    drawInfo.cShoe = shader;
                    drawInfo.cBack = shader;
                    drawInfo.cBackpack = shader;
                    drawInfo.cShield = shader;
                    drawInfo.cNeck = shader;
                    drawInfo.cHandOn = shader;
                    drawInfo.cHandOff = shader;
                    drawInfo.cBalloon = shader;
                    drawInfo.cBalloonFront = shader;
                    drawInfo.cFace = shader;
                    drawInfo.cFaceHead = shader;
                    drawInfo.cFront = shader;
                }
            }
        }

        public void AddPet(bool toggle, bool vanityToggle, int buff, int proj)
        {
            if (vanityToggle)
            {
                PetsActive = false;
                return;
            }

            if (Player.whoAmI == Main.myPlayer && toggle && Player.FindBuffIndex(buff) == -1 && Player.ownedProjectileCounts[proj] < 1)
            {
                Projectile p = Main.projectile[Projectile.NewProjectile(Player.GetSource_Misc("Pet"), Player.Center.X, Player.Center.Y, 0f, -1f, proj, 0, 0f, Player.whoAmI)];
                p.netUpdate = true;
            }
        }
        public void AddMinion(Item item, bool toggle, int proj, int damage, float knockback)
        {
            if (Player.whoAmI != Main.myPlayer) return;
            if (Player.ownedProjectileCounts[proj] < 1 && Player.whoAmI == Main.myPlayer && toggle)
            {
                FargoSoulsUtil.NewSummonProjectile(Player.GetSource_Accessory(item), Player.Center, -Vector2.UnitY, proj, damage, knockback, Main.myPlayer);
            }
        }

        private void KillPets()
        {
            int petId = Player.miscEquips[0].buffType;
            int lightPetId = Player.miscEquips[1].buffType;

            Player.buffImmune[petId] = true;
            Player.buffImmune[lightPetId] = true;

            Player.ClearBuff(petId);
            Player.ClearBuff(lightPetId);

            //memorizes Player selections
            if (!WasAsocial)
            {
                HidePetToggle0 = Player.hideMisc[0];
                HidePetToggle1 = Player.hideMisc[1];

                WasAsocial = true;
            }

            //disables pet and light pet too!
            if (!Player.hideMisc[0])
            {
                Player.TogglePet();
            }

            if (!Player.hideMisc[1])
            {
                Player.ToggleLight();
            }

            Player.hideMisc[0] = true;
            Player.hideMisc[1] = true;
        }

        public static void Squeak(Vector2 center)
        {
            if (!Main.dedServ)
                SoundEngine.PlaySound(new SoundStyle($"FargowiltasSouls/Assets/Sounds/SqueakyToy/squeak{Main.rand.Next(1, 7)}"), center);
        }

        private int InfestedExtraDot()
        {
            int buffIndex = Player.FindBuffIndex(ModContent.BuffType<InfestedBuff>());
            if (buffIndex == -1)
            {
                buffIndex = Player.FindBuffIndex(ModContent.BuffType<NeurotoxinBuff>());
                if (buffIndex == -1)
                    return 0;
            }

            int timeLeft = Player.buffTime[buffIndex];
            float baseVal = (float)(MaxInfestTime - timeLeft) / 90; //change the denominator to adjust max power of DOT
            int modifier = (int)(baseVal * baseVal + 4);

            InfestedDust = baseVal / 10 + 1f;
            if (InfestedDust > 5f)
                InfestedDust = 5f;

            return modifier * 2;
        }

        //        public override void CatchFish(Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer, int questFish, ref int caughtType, ref bool junk)
        //        {
        //            if (bait.type == ModContent.ItemType<TruffleWormEX>())
        //            {
        //                caughtType = 0;
        //                bool spawned = false;
        //                for (int i = 0; i < 1000; i++)
        //                {
        //                    if (Main.projectile[i].active && Main.projectile[i].bobber
        //                        && Main.projectile[i].owner == Player.whoAmI && Player.whoAmI == Main.myPlayer)
        //                    {
        //                        Main.projectile[i].ai[0] = 2f; //cut fishing lines
        //                        Main.projectile[i].netUpdate = true;

        //                        if (!spawned && Main.projectile[i].wet && WorldSavingSystem.EternityMode && !NPC.AnyNPCs(NPCID.DukeFishron)) //should spawn boss
        //                        {
        //                            spawned = true;
        //                            if (Main.netMode == NetmodeID.SinglePlayer) //singlePlayer
        //                            {
        //                                EModeGlobalNPC.spawnFishronEX = true;
        //                                NPC.NewNPC((int)Main.projectile[i].Center.X, (int)Main.projectile[i].Center.Y + 100,
        //                                    NPCID.DukeFishron, 0, 0f, 0f, 0f, 0f, Player.whoAmI);
        //                                EModeGlobalNPC.spawnFishronEX = false;
        //                                Main.NewText("Duke Fishron EX has awoken!", 50, 100, 255);
        //                            }
        //                            else if (Main.netMode == NetmodeID.MultiPlayerClient) //MP, broadcast(?) packet from spawning Player's client
        //                            {
        //                                var netMessage = mod.GetPacket();
        //                                netMessage.Write((byte)FargowiltasSouls.PacketID.SpawnFishronEX);
        //                                netMessage.Write((byte)Player.whoAmI);
        //                                netMessage.Write((int)Main.projectile[i].Center.X);
        //                                netMessage.Write((int)Main.projectile[i].Center.Y + 100);
        //                                netMessage.Send();
        //                            }
        //                            else if (Main.netMode == NetmodeID.Server)
        //                            {
        //                                ChatHelper.BroadcastChatMessage(Terraria.Localization.NetworkText.FromLiteral("???????"), Color.White);
        //                            }
        //                        }
        //                    }
        //                }
        //                if (spawned)
        //                {
        //                    bait.stack--;
        //                    if (bait.stack <= 0)
        //                        bait.SetDefaults(0);
        //                }
        //            }
        //        }

        public override void PostNurseHeal(NPC nurse, int health, bool removeDebuffs, int price)
        {
            if (Player.HasEffect<GuttedHeartMinions>())
                GuttedHeartMinions.NurseHeal(Player);
        }

        public override bool CanConsumeAmmo(Item weapon, Item ammo)
        {
            if (weapon.CountsAsClass(DamageClass.Ranged))
            {
                if (RangedEssence && Main.rand.NextBool(10))
                    return false;
                if (RangedSoul && Main.rand.NextBool(5))
                    return false;
            }
            if (GaiaSet && Main.rand.NextBool(10))
                return false;
            return true;
        }

        public int frameCounter = 0;
        public int frameSnow = 1;
        public int frameMutantAura = 0;
        //public int frameMutantLightning = 0;

        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            if (BetsyDashing || ShellHide || GoldShell)
            {
                foreach (var layer in PlayerDrawLayerLoader.Layers)
                {
                    //if (layer.Mod == null) //Only hide vanilla layers, optional. Does not hide modded layers added as Between or Multiple, but will hide layers marked as Before/After regardless
                    //{
                    layer.Hide();
                    //}
                }
            }

            //if (SquirrelMount)
            //{
            //    foreach (PlayerDrawLayer layer in PlayerDrawLayerLoader.Layers)
            //    {
            //        layer.


            //        if (layer != PlayerLayer.MountBack && PlayerLayer != PlayerLayer.MountFront && PlayerLayer != PlayerLayer.MiscEffectsFront && PlayerLayer != PlayerLayer.MiscEffectsBack)
            //        {
            //            PlayerLayer.visible = false;
            //        }
            //    }
            //}

        }

        public int GetHealMultiplier(int heal)
        {
            float multiplier = 1f;
            bool squire = SquireEnchantActive;
            bool valhalla = ValhallaEnchantActive;
            if ((squire || valhalla))
            {
                bool forceEffect = ForceEffect<SquireEnchant>() || ForceEffect<ValhallaKnightEnchant>();
                if (Eternity)
                    multiplier = 5f;
                else if (forceEffect && valhalla)
                    multiplier = 1.2f;
                else if (valhalla || (forceEffect && squire))
                    multiplier = 1.15f;
                else if (squire)
                    multiplier = 1.10f;
            }

            heal = (int)(heal * multiplier);

            return heal;
        }

        public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
        {
            healValue = GetHealMultiplier(healValue);
            if (Player.HasEffect<HallowEffect>())
            {
                healValue = 0;
            }
        }

        public void HealPlayer(int amount)
        {
            amount = GetHealMultiplier(amount);
            Player.statLife += amount;
            if (Player.statLife > Player.statLifeMax2)
                Player.statLife = Player.statLifeMax2;
            Player.HealEffect(amount);
        }

        public override void ModifyScreenPosition()
        {
            if (Screenshake > 0)
                Main.screenPosition += Main.rand.NextVector2Circular(7, 7);
        }

        public override void CopyClientState(ModPlayer clientClone)
        {
            FargoSoulsPlayer modPlayer = clientClone as FargoSoulsPlayer;
            modPlayer.Toggler = Toggler;
        }

        public void SyncToggle(AccessoryEffect effect)
        {
            if (!TogglesToSync.ContainsKey(effect))
                TogglesToSync.Add(effect, Player.GetToggle(effect).ToggleBool);
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            foreach (KeyValuePair<AccessoryEffect, bool> toggle in TogglesToSync)
            {
                ModPacket packet = Mod.GetPacket();

                packet.Write((byte)FargowiltasSouls.PacketID.SyncOneToggle);
                packet.Write((byte)Player.whoAmI);
                packet.Write(toggle.Key.FullName);
                packet.Write(toggle.Value);

                packet.Send(toWho, fromWho);
            }

            TogglesToSync.Clear();
        }
        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            FargoSoulsPlayer modPlayer = clientPlayer as FargoSoulsPlayer;
            if (modPlayer.Toggler.Toggles != Toggler.Toggles)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)FargowiltasSouls.PacketID.SyncTogglesOnJoin);
                packet.Write((byte)Player.whoAmI);
                packet.Write((byte)Toggler.Toggles.Count);

                for (int i = 0; i < Toggler.Toggles.Count; i++)
                {
                    packet.Write(Toggler.Toggles.Values.ElementAt(i).ToggleBool);
                }

                packet.Send();
            }
        }

        public void AddBuffNoStack(int buff, int duration)
        {
            if (!Player.HasBuff(buff) && ReallyAwfulDebuffCooldown <= 0)
            {
                Player.AddBuff(buff, duration);
                int d = Player.FindBuffIndex(buff);
                if (d != -1) //if debuff successfully applied
                    ReallyAwfulDebuffCooldown = Player.buffTime[d] + 240;
            }
        }

        public void TryAdditionalAttacks(int damage, DamageClass damageType)
        {
            // :)
        }

        public Rectangle GetPrecisionHurtbox()
        {
            Rectangle hurtbox = Player.Hitbox;
            hurtbox.X += hurtbox.Width / 2;
            hurtbox.Y += hurtbox.Height / 2;
            hurtbox.Width = Math.Min(hurtbox.Width, hurtbox.Height);
            hurtbox.Height = Math.Min(hurtbox.Width, hurtbox.Height);
            hurtbox.X -= hurtbox.Width / 2;
            hurtbox.Y -= hurtbox.Height / 2;
            return hurtbox;
        }
        public bool ForceEffect(ModItem modItem)
        {
            //This will probably be made less ugly in a future refactor update.
            bool CheckForces(int type)
            {
                int force = BaseEnchant.Force[type];
                if (force <= 0)
                    return BaseEnchant.CraftsInto[type] > 0 && CheckForces(BaseEnchant.CraftsInto[type]); //check force of enchant it crafts into, recursively
                return ForceEffects.Contains(force);
            }
            bool CheckWizard(int type)
            {
                if (WizardedItem != null && !WizardedItem.IsAir && WizardedItem.type == type)
                    return true;
                return (BaseEnchant.CraftsInto[type] > 0 && CheckWizard(BaseEnchant.CraftsInto[type]));
            }
            if (TerrariaSoul)
                return true;

            if (Main.gamePaused || modItem == null || modItem.Item == null || modItem.Item.IsAir)
                return false;

            if (modItem is BaseEnchant)
            {
                if (CheckWizard(modItem.Item.type) || CheckForces(modItem.Item.type))
                    return true;
            }

            if (modItem is BaseSoul || modItem is BaseForce)
                return true;

            return false;
        }
        public bool ForceEffect<T>() where T : BaseEnchant => ForceEffect(ModContent.GetInstance<T>());
        public bool ForceEffect(int? enchType)
        {
            if (enchType == null || enchType <= 0)
                return false;

            ModItem item = ModContent.GetModItem((int)enchType);
            return item != null && ForceEffect(item);
        }
        public override void PreSavePlayer()
        {
            SquireEnchant.ResetMountStats(this);
        }

    }
}
