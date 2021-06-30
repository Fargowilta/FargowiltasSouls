using System;
using System.Collections.Generic;
using FargowiltasSouls.Buffs.Souls;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Critters;
using FargowiltasSouls.Projectiles.Masomode;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items
{
    public class FargoGlobalItem : GlobalItem
    {
        public override void SetDefaults(Item item)
        {
            if (item.type == ItemID.Acorn)
            {
                item.ammo = item.type;
            }
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (player.manaCost <= 0f) player.manaCost = 0f;
        }

        public override void GrabRange(Item item, Player player, ref int grabRange)
        {
            FargoPlayer p = (FargoPlayer)player.GetModPlayer(mod, "FargoPlayer");
            //ignore money, hearts, mana stars
            if (player.GetToggleValue("IronM", false) && player.whoAmI == Main.myPlayer && p.IronEnchant && item.type != ItemID.CopperCoin && item.type != ItemID.SilverCoin && item.type != ItemID.GoldCoin && item.type != ItemID.PlatinumCoin && item.type != ItemID.HermesBoots && item.type != ItemID.CandyApple && item.type != ItemID.SoulCake &&
                item.type != ItemID.Star && item.type != ItemID.CandyCane && item.type != ItemID.SugarPlum && item.type != ItemID.Heart)
            {
                grabRange += (p.TerraForce || p.WizardEnchant) ? 1000 : 250;

                //half as effective on nebula bois
                if (!p.TerrariaSoul && (item.type == ItemID.NebulaPickup1 || item.type == ItemID.NebulaPickup2 || item.type == ItemID.NebulaPickup3))
                {
                    grabRange -= (p.TerraForce || p.WizardEnchant) ? 500 : 125;
                }
            }
        }

        public override void PickAmmo(Item weapon, Item ammo, Player player, ref int type, ref float speed, ref int damage, ref float knockback)
        {
            FargoPlayer modPlayer = (FargoPlayer)player.GetModPlayer(mod, "FargoPlayer");

            if (modPlayer.Jammed)
                type = ProjectileID.ConfettiGun;

            if (FargoSoulsWorld.MasochistMode) //ammo nerf, strongest on arrow/bullet/dart
            {
                double modifier = ammo.ammo == AmmoID.Arrow || ammo.ammo == AmmoID.Bullet || ammo.ammo == AmmoID.Dart ? .80 : .20;
                damage -= (int)Math.Round(ammo.damage * player.rangedDamage * modifier, MidpointRounding.AwayFromZero); //always round up
            }
        }

        public override bool ConsumeItem(Item item, Player player)
        {
            FargoPlayer p = player.GetModPlayer<FargoPlayer>();

            if (item.makeNPC > 0 && (p.WoodForce || p.WizardEnchant) && Main.rand.Next(2) == 0)
            {
                return false;
            }

            if (p.BuilderMode && (item.createTile != -1 || item.createWall != -1) && item.type != ItemID.PlatinumCoin && item.type != ItemID.GoldCoin)
                return false;
            return true;
        }

        public override void GetWeaponKnockback(Item item, Player player, ref float knockback)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            if (modPlayer.UniverseEffect || modPlayer.Eternity) knockback *= 2;
        }

        public override bool CanUseItem(Item item, Player player)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            
            if (modPlayer.IronGuard)
            {
                //Main.NewText($"iron {modPlayer.ironShieldCD}, {modPlayer.ironShieldTimer}");
                modPlayer.IronGuard = false;
                modPlayer.wasHoldingShield = false;
                player.shield_parry_cooldown = 0; //prevent that annoying tick sound
                //check is necessary so if player does a real parry then switches to right click weapon, using it won't reset cooldowns
                if (modPlayer.ironShieldCD == 40 && modPlayer.ironShieldTimer == 20)
                {
                    modPlayer.ironShieldCD = 0;
                    modPlayer.ironShieldTimer = 0;
                }
            }

            //dont use hotkeys in stasis
            if (player.HasBuff(ModContent.BuffType<GoldenStasis>()))
            {
                if (item.type == ItemID.RodofDiscord)
                {
                    player.ClearBuff(ModContent.BuffType<Buffs.Souls.GoldenStasis>());
                }
                else
                {
                    return false;
                }
            }

            if (FargoSoulsWorld.MasochistMode)
            {
                if (item.type == ItemID.RodofDiscord &&
                    (modPlayer.LihzahrdCurse ||
                    (Framing.GetTileSafely(Main.MouseWorld).wall == WallID.LihzahrdBrickUnsafe
                    && !player.buffImmune[ModContent.BuffType<Buffs.Masomode.LihzahrdCurse>()])))
                {
                    return false;
                }

                if (modPlayer.LihzahrdCurse &&
                    (item.type == ItemID.WireKite || item.type == ItemID.WireCutter))
                {
                    return false;
                }
            }

            if (item.magic && player.GetModPlayer<FargoPlayer>().ReverseManaFlow)
            {
                int damage = (int)(item.mana / (1f - player.endurance) + player.statDefense);
                player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " was destroyed by their own magic."), damage, 0);
                player.immune = false;
                player.immuneTime = 0;
            }

            if (modPlayer.BuilderMode && (item.createTile != -1 || item.createWall != -1) && item.type != ItemID.PlatinumCoin && item.type != ItemID.GoldCoin)
            {
                item.useTime = 1;
                item.useAnimation = 1;
            }

            if (modPlayer.AdditionalAttacks && modPlayer.AdditionalAttacksTimer <= 0 //non weapons and weapons with no ammo begone
                && item.damage > 0 && player.HasAmmo(item, true) && !(item.mana > 0 && player.statMana < item.mana)
                && item.type != ItemID.ExplosiveBunny && item.type != ItemID.Cannonball
                && item.useTime > 0 && item.createTile == -1 && item.createWall == -1 && item.ammo == AmmoID.None && item.hammer == 0 && item.pick == 0 && item.axe == 0)
            {
                modPlayer.AdditionalAttacksTimer = 60;

                Vector2 position = player.Center;
                Vector2 velocity = Vector2.Normalize(Main.MouseWorld - position);

                if (modPlayer.BorealEnchant && player.GetToggleValue("Boreal") && player.whoAmI == Main.myPlayer)
                {
                    Vector2 vel = Vector2.Normalize(Main.MouseWorld - player.Center) * 17f;
                    int p = Projectile.NewProjectile(player.Center, vel, ProjectileID.SnowBallFriendly, (int)(item.damage * .5f), 1, Main.myPlayer);

                    int numSnowballs = 3;

                    if (modPlayer.WoodForce || modPlayer.WizardEnchant)
                    {
                        numSnowballs = 5;
                    }

                    if (p != 1000)
                        FargoGlobalProjectile.SplitProj(Main.projectile[p], numSnowballs, MathHelper.Pi / 10, 1);
                }

                if (modPlayer.CelestialRune && player.GetToggleValue("MasoCelest"))
                {
                    if (item.melee && item.pick == 0 && item.axe == 0 && item.hammer == 0) //fireball
                    {
                        Main.PlaySound(SoundID.Item34, position);
                        for (int i = 0; i < 3; i++)
                        {
                            Projectile.NewProjectile(position, velocity.RotatedByRandom(Math.PI / 6) * Main.rand.NextFloat(6f, 10f),
                                ModContent.ProjectileType<CelestialRuneFireball>(), (int)(50f * player.meleeDamage), 9f, player.whoAmI);
                        }
                    }
                    if (item.ranged) //lightning
                    {
                        float ai1 = Main.rand.Next(100);
                        Vector2 vel = Vector2.Normalize(velocity.RotatedByRandom(Math.PI / 4)) * 7f;
                        Projectile.NewProjectile(position, vel, ModContent.ProjectileType<CelestialRuneLightningArc>(),
                            (int)(50f * player.rangedDamage), 1f, player.whoAmI, velocity.ToRotation(), ai1);
                    }
                    if (item.magic) //ice mist
                    {
                        Projectile.NewProjectile(position, velocity * 4.25f, ModContent.ProjectileType<CelestialRuneIceMist>(), (int)(50f * player.magicDamage), 4f, player.whoAmI);
                    }
                    if (item.thrown) //ancient vision
                    {
                        Projectile.NewProjectile(position, velocity * 16f, ModContent.ProjectileType<CelestialRuneAncientVision>(), (int)(50f * player.magicDamage), 0, player.whoAmI);
                    }
                }

                if (modPlayer.PumpkingsCape && player.GetToggleValue("MasoPump"))
                {
                    if (item.melee && item.pick == 0 && item.axe == 0 && item.hammer == 0) //flaming jack
                    {
                        float distance = 2000f;
                        int target = -1;
                        for (int i = 0; i < 200; i++)
                        {
                            if (Main.npc[i].active && Main.npc[i].CanBeChasedBy())
                            {
                                float newDist = Main.npc[i].Distance(player.Center);
                                if (newDist < distance)
                                {
                                    distance = newDist;
                                    target = i;
                                }
                            }
                        }
                        if (target != -1)
                            Projectile.NewProjectile(position, velocity * 8f, ProjectileID.FlamingJack, (int)(75f * player.meleeDamage), 7.5f, player.whoAmI, target, 0f);
                    }
                    if (item.ranged) //jack o lantern
                    {
                        Projectile.NewProjectile(position, velocity * 11f, ProjectileID.JackOLantern, (int)(95f * player.rangedDamage), 8f, player.whoAmI);
                    }
                    if (item.magic) //bat scepter
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 newVel = velocity * 10f;
                            newVel.X += Main.rand.Next(-35, 36) * 0.02f;
                            newVel.Y += Main.rand.Next(-35, 36) * 0.02f;
                            Projectile.NewProjectile(position, newVel, ProjectileID.Bat, (int)(45f * player.magicDamage), 3f, player.whoAmI);
                        }
                    }
                }
            }

            //critter attack timer
            if (modPlayer.WoodEnchant && player.altFunctionUse == ItemAlternativeFunctionID.ActivatedAndUsed && item.makeNPC > 0)
            {
                if (modPlayer.CritterAttackTimer == 0)
                {
                    Vector2 vel = Vector2.Normalize(Main.MouseWorld - player.Center);
                    float damageMultiplier = player.minionDamage;
                    
                    int type = -1;
                    int damage = 0;
                    int attackCooldown = 0;
                    
                    switch (item.type)
                    {
                        //case ItemID.Bunny:
                        //    type = ProjectileID.ExplosiveBunny;
                        //    damage = 10;
                        //    attackCooldown = 10;
                        //    break;

                        case ItemID.Bird:
                            type = ModContent.ProjectileType<BirdProj>();
                            damage = 15;
                            attackCooldown = 15;
                            break;

                        case ItemID.BlueJay:
                            type = ModContent.ProjectileType<BlueJayProj>();
                            damage = 10;
                            attackCooldown = 10;
                            break;

                        case ItemID.Cardinal:
                            type = ModContent.ProjectileType<CardinalProj>();
                            damage = 20;
                            attackCooldown = 20;
                            break;
                    }

                    if (type != -1)
                    {
                        Projectile.NewProjectile(player.Center, vel * 2f, type, damage, 2, player.whoAmI);
                        modPlayer.CritterAttackTimer = attackCooldown;
                    }

                    
                }

                



                return false;
            }

            if (item.type == ItemID.RodofDiscord)
            {
                if (FargoSoulsWorld.MasochistMode && EModeGlobalNPC.AnyBossAlive())
                {
                    player.AddBuff(ModContent.BuffType<Buffs.Masomode.ChaosLife>(), 30);
                    modPlayer.MaxLifeReduction += 100;
                    player.chaosState = true;

                    /*player.statLife -= player.statLifeMax2 / 5;
                    PlayerDeathReason damageSource = PlayerDeathReason.ByOther(13);
                    if (Main.rand.Next(2) == 0)
                        damageSource = PlayerDeathReason.ByOther(player.Male ? 14 : 15);
                    if (player.statLife <= 0 && !player.chaosState) //since chaos state will check and kill anyway, avoid doublekill
                        player.KillMe(damageSource, 1, 0);
                    player.lifeRegenCount = 0;
                    player.lifeRegenTime = 0;*/
                }
            }

            return true;
        }

        public override bool UseItem(Item item, Player player)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            
            if (item.type == ItemID.RodofDiscord)
            {
                player.ClearBuff(ModContent.BuffType<Buffs.Souls.GoldenStasis>());
            }

            if (modPlayer.UniverseEffect && item.damage > 0) item.shootSpeed *= modPlayer.Eternity ? 2f : 1.5f;

            return false;
        }

        public override bool AltFunctionUse(Item item, Player player)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            if (modPlayer.WoodEnchant)
            {
                switch (item.type)
                {
                    case ItemID.Bunny:
                    case ItemID.Bird:
                    case ItemID.BlueJay:
                    case ItemID.Cardinal:
                        return true;

                }
            }



            return base.AltFunctionUse(item, player);
        }

        public override bool NewPreReforge(Item item)
        {
            /*if (Main.player[item.owner].GetModPlayer<FargoPlayer>().SecurityWallet)
            {
                switch(item.prefix)
                {
                    case PrefixID.Warding:  if (SoulConfig.Instance.walletToggles.Warding)  return false; break;
                    case PrefixID.Violent:  if (SoulConfig.Instance.walletToggles.Violent)  return false; break;
                    case PrefixID.Quick:    if (SoulConfig.Instance.walletToggles.Quick)    return false; break;
                    case PrefixID.Lucky:    if (SoulConfig.Instance.walletToggles.Lucky)    return false; break;
                    case PrefixID.Menacing: if (SoulConfig.Instance.walletToggles.Menacing) return false; break;
                    case PrefixID.Legendary:if (SoulConfig.Instance.walletToggles.Legendary)return false; break;
                    case PrefixID.Unreal:   if (SoulConfig.Instance.walletToggles.Unreal)   return false; break;
                    case PrefixID.Mythical: if (SoulConfig.Instance.walletToggles.Mythical) return false; break;
                    case PrefixID.Godly:    if (SoulConfig.Instance.walletToggles.Godly)    return false; break;
                    case PrefixID.Demonic:  if (SoulConfig.Instance.walletToggles.Demonic)  return false; break;
                    case PrefixID.Ruthless: if (SoulConfig.Instance.walletToggles.Ruthless) return false; break;
                    case PrefixID.Light:    if (SoulConfig.Instance.walletToggles.Light)    return false; break;
                    case PrefixID.Deadly:   if (SoulConfig.Instance.walletToggles.Deadly)   return false; break;
                    case PrefixID.Rapid:    if (SoulConfig.Instance.walletToggles.Rapid)    return false; break;
                    default: break;
                }
            }*/
            return true;
        }

        public override bool Shoot(Item item, Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (FargoSoulsWorld.MasochistMode && !NPC.downedBoss3 && item.type == ItemID.WaterBolt)
            {
                type = ProjectileID.WaterGun;
                damage = 0;
            }

            return true;
        }

        public override bool ReforgePrice(Item item, ref int reforgePrice, ref bool canApplyDiscount)
        {
            if (Main.LocalPlayer.GetModPlayer<FargoPlayer>().SecurityWallet)
                reforgePrice /= 2;
            return true;
        }

        //summon variants
        private static readonly int[] Summon = { ItemID.NimbusRod, ItemID.CrimsonRod, ItemID.BeeGun, ItemID.WaspGun, ItemID.PiranhaGun, ItemID.BatScepter };

        public override bool CanRightClick(Item item)
        {
            if (Array.IndexOf(Summon, item.type) > -1)
            {
                return true;
            }

            return base.CanRightClick(item);
        }

        public override void RightClick(Item item, Player player)
        {
            int newType = -1;

            if (Array.IndexOf(Summon, item.type) > -1)
            {
                newType = mod.ItemType(ItemID.GetUniqueKey(item.type).Replace("Terraria ", string.Empty) + "Summon");
            }

            if (newType != -1)
            {
                int num = Item.NewItem(player.getRect(), newType, prefixGiven: item.prefix);

                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.SyncItem, number: num, number2: 1f);
                }
            }
        }

        public override bool WingUpdate(int wings, Player player, bool inUse)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            if (modPlayer.ChloroEnchant && player.GetToggleValue("Jungle") && inUse)
            {
                modPlayer.CanJungleJump = false;

                //spwn cloud
                if (modPlayer.JungleCD == 0)
                {
                    int dmg = (modPlayer.NatureForce || modPlayer.WizardEnchant) ? 150 : 75;
                    Main.PlaySound(SoundID.Item, (int)player.position.X, (int)player.position.Y, 62, 0.5f);
                    FargoGlobalProjectile.XWay(10, new Vector2(player.Center.X, player.Center.Y + (player.height / 2)), ProjectileID.SporeCloud, 3f, modPlayer.HighestDamageTypeScaling(dmg), 0f);

                    modPlayer.JungleCD = 8;
                }
            }

            return base.WingUpdate(wings, player, inUse);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (Array.IndexOf(Summon, item.type) > -1)
            {
                TooltipLine helperLine = new TooltipLine(mod, "help", "Right click to convert");
                tooltips.Add(helperLine);
            }

            if (FargoSoulsWorld.MasochistMode)
            {
                if (item.ammo != AmmoID.None && item.damage > 0)
                {
                    if (item.ammo == AmmoID.Arrow || item.ammo == AmmoID.Bullet || item.ammo == AmmoID.Dart)
                        tooltips.Add(new TooltipLine(mod, "masoNerf", "[c/ff0000:Eternity Mode:] Contributes 80% less damage to weapons"));
                    else if (!(item.type >= 71 && item.type <= 74)) //dont show for coins
                        tooltips.Add(new TooltipLine(mod, "masoNerf", "[c/ff0000:Eternity Mode:] Contributes 20% less damage to weapons"));
                }

                switch (item.type)
                {
                    case ItemID.RodofDiscord:
                        tooltips.Add(new TooltipLine(mod, "masoNerf", "[c/ff0000:Eternity Mode:] During boss fights, every use takes life and max life"));
                        break;

                    case ItemID.ArcheryPotion:
                    case ItemID.MagicQuiver:
                    case ItemID.ShroomiteHelmet:
                    case ItemID.ShroomiteHeadgear:
                    case ItemID.ShroomiteMask:
                        tooltips.Add(new TooltipLine(mod, "masoNerf", "[c/ff0000:Eternity Mode:] Grants additive damage instead of multiplicative"));
                        break;

                    case ItemID.CrystalBullet:
                    case ItemID.HolyArrow:
                        tooltips.Add(new TooltipLine(mod, "masoNerf2", "[c/ff0000:Eternity Mode:] Can only split 4 times per second"));
                        break;

                    case ItemID.ChlorophyteBullet:
                        tooltips.Add(new TooltipLine(mod, "masoNerf2", "[c/ff0000:Eternity Mode:] Reduced speed and duration"));
                        break;

                    case ItemID.WaterBolt:
                        if (!NPC.downedBoss3)
                        {
                            tooltips.Add(new TooltipLine(mod, "masoNerf", "[c/ff0000:Eternity Mode:] Cannot be used until Skeletron is defeated"));
                        }
                        break;

                    case ItemID.TitaniumHeadgear:
                    case ItemID.TitaniumHelmet:
                    case ItemID.TitaniumMask:
                    case ItemID.TitaniumBreastplate:
                    case ItemID.TitaniumLeggings:
                        tooltips.Add(new TooltipLine(mod, "masoNerf", "[c/ff0000:Eternity Mode:] Shadow Dodge activation will temporarily reduce your damage"));
                        break;

                    case ItemID.SpectreHood:
                        tooltips.Add(new TooltipLine(mod, "masoNerf", "[c/ff0000:Eternity Mode:] Healing orbs move slower and disappear quickly"));
                        break;

                    case ItemID.FrozenTurtleShell:
                        tooltips.Add(new TooltipLine(mod, "masoNerf", "[c/ff0000:Eternity Mode:] Damage reduction is 15% instead of 25%"));
                        break;

                    case ItemID.DD2BetsyBow:
                    case ItemID.Uzi:
                    case ItemID.PhoenixBlaster:
                    case ItemID.LastPrism:
                    case ItemID.OnyxBlaster:
                    case ItemID.SkyFracture:
                    case ItemID.Handgun:
                    case ItemID.SpikyBall:
                    case ItemID.SDMG:
                    case ItemID.Xenopopper:
                    case ItemID.NebulaArcanum:
                    case ItemID.PainterPaintballGun:
                    case ItemID.MoltenFury:
                    case ItemID.Phantasm:
                    case ItemID.SnowmanCannon:
                        tooltips.Add(new TooltipLine(mod, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 25%"));
                        break;

                    case ItemID.StarCannon:
                    case ItemID.ElectrosphereLauncher:
                    case ItemID.DaedalusStormbow:
                    case ItemID.BeesKnees:
                    case ItemID.LaserMachinegun:
                        tooltips.Add(new TooltipLine(mod, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 33%"));
                        break;

                    case ItemID.Beenade:
                    case ItemID.Razorpine:
                        tooltips.Add(new TooltipLine(mod, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 33%"));
                        tooltips.Add(new TooltipLine(mod, "masoNerf2", "[c/ff0000:Eternity Mode:] Reduced attack speed by 33%"));
                        break;

                    case ItemID.Tsunami:
                    case ItemID.ChlorophyteShotbow:
                    case ItemID.HellwingBow:
                    case ItemID.DartPistol:
                    case ItemID.DartRifle:
                    case ItemID.Megashark:
                    case ItemID.BatScepter:
                    case ItemID.ChainGun:
                    case ItemID.VortexBeater:
                    case ItemID.RavenStaff:
                    case ItemID.XenoStaff:
                        tooltips.Add(new TooltipLine(mod, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 15%"));
                        break;

                    case ItemID.VampireKnives:
                        tooltips.Add(new TooltipLine(mod, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 25%"));
                        tooltips.Add(new TooltipLine(mod, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced attack speed by 25%"));
                        tooltips.Add(new TooltipLine(mod, "masoNerf2", "[c/ff0000:Eternity Mode:] Reduced lifesteal rate when above 33% life"));
                        break;

                    case ItemID.BlizzardStaff:
                        tooltips.Add(new TooltipLine(mod, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 33%"));
                        tooltips.Add(new TooltipLine(mod, "masoNerf2", "[c/ff0000:Eternity Mode:] Reduced attack speed by 50%"));
                        break;

                    case ItemID.DemonScythe:
                        if (NPC.downedBoss2)
                        {
                            tooltips.Add(new TooltipLine(mod, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 33%"));
                        }
                        else
                        {
                            tooltips.Add(new TooltipLine(mod, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 50% until an evil boss is defeated"));
                            tooltips.Add(new TooltipLine(mod, "masoNerf2", "[c/ff0000:Eternity Mode:] Reduced attack speed by 25% until an evil boss is defeated"));
                        }
                        break;

                    case ItemID.SpaceGun:
                        if (NPC.downedBoss2)
                        {
                            tooltips.Add(new TooltipLine(mod, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 15%"));
                        }
                        else
                        {
                            tooltips.Add(new TooltipLine(mod, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 25% until an evil boss is defeated"));
                            tooltips.Add(new TooltipLine(mod, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced attack speed by 25% until an evil boss is defeated"));
                        }
                        break;

                    case ItemID.BeeGun:
                        tooltips.Add(new TooltipLine(mod, "masoNerf2", "[c/ff0000:Eternity Mode:] Reduced attack speed by 33%"));
                        break;

                    case ItemID.DD2BallistraTowerT1Popper:
                    case ItemID.DD2BallistraTowerT2Popper:
                    case ItemID.DD2BallistraTowerT3Popper:
                    case ItemID.DD2ExplosiveTrapT1Popper:
                    case ItemID.DD2ExplosiveTrapT2Popper:
                    case ItemID.DD2ExplosiveTrapT3Popper:
                    case ItemID.DD2FlameburstTowerT1Popper:
                    case ItemID.DD2FlameburstTowerT2Popper:
                    case ItemID.DD2FlameburstTowerT3Popper:
                    case ItemID.DD2LightningAuraT1Popper:
                    case ItemID.DD2LightningAuraT2Popper:
                    case ItemID.DD2LightningAuraT3Popper:
                        tooltips.Add(new TooltipLine(mod, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced attack speed by 50%"));
                        break;

                    case ItemID.SlimeStaff:
                        tooltips.Add(new TooltipLine(mod, "masoBuff", "[c/00ff00:Eternity Mode:] Can summon more slimes"));
                        break;

                    case ItemID.SpiderStaff:
                    case ItemID.OpticStaff:
                    case ItemID.DeadlySphereStaff:
                        tooltips.Add(new TooltipLine(mod, "masoBuff", "[c/00ff00:Eternity Mode:] Increased hit rate"));
                        break;

                    case ItemID.DD2SquireBetsySword: //flying dragon
                        tooltips.Add(new TooltipLine(mod, "masoBuff", "[c/00ff00:Eternity Mode:] Increased damage by 33%"));
                        tooltips.Add(new TooltipLine(mod, "masoBuff2", "[c/00ff00:Eternity Mode:] Increased attack speed by 33%"));
                        break;

                    case ItemID.MonkStaffT3: //sky dragon's fury
                        tooltips.Add(new TooltipLine(mod, "masoBuff", "[c/00ff00:Eternity Mode:] Increased damage by 25%"));
                        break;
                }

                if (item.summon)
                    tooltips.Add(new TooltipLine(mod, "masoMinionNerf", "[c/ff0000:Eternity Mode:] Damage drastically reduced when used alongside other classes"));
            }
        }
    }
}