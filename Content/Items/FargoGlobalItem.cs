using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Placables;
using FargowiltasSouls.Content.Items.Weapons.Challengers;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
//using FargowiltasSouls.Content.Buffs.Souls;
//using FargowiltasSouls.Content.Projectiles.Critters;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items
{
	public class FargoGlobalItem : GlobalItem
    {
        public override void SetDefaults(Item item)
        {
            if (item.type == ItemID.Acorn || item.type == ItemID.Bone || item.type == ItemID.RottenChunk)
            {
                item.ammo = item.type;
            }
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (player.manaCost <= 0f) player.manaCost = 0f;
        }

        public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
        {
        }
        
        public override void GrabRange(Item item, Player player, ref int grabRange)
        {
            FargoSoulsPlayer p = player.FargoSouls();
            //ignore money, hearts, mana stars
            if (player.whoAmI == Main.myPlayer && player.HasEffect<IronEffect>() && item.type != ItemID.CopperCoin && item.type != ItemID.SilverCoin && item.type != ItemID.GoldCoin && item.type != ItemID.PlatinumCoin && item.type != ItemID.CandyApple && item.type != ItemID.SoulCake &&
                item.type != ItemID.Star && item.type != ItemID.CandyCane && item.type != ItemID.SugarPlum && item.type != ItemID.Heart)
            {
                int rangeBonus = 160;
                if (p.ForceEffect<IronEnchant>())
                    rangeBonus = 320;
                if (p.TerrariaSoul)
                    rangeBonus = 640;

                grabRange += rangeBonus;
            }
        }
        
        public override bool OnPickup(Item item, Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (modPlayer.GoldEnchantActive && player.whoAmI == Main.myPlayer && player.GetToggleValue("GoldToPiggy", false))
                modPlayer.GoldEnchMoveCoins = true;

            return base.OnPickup(item, player);
        }

        public override void PickAmmo(Item weapon, Item ammo, Player player, ref int type, ref float speed, ref StatModifier damage, ref float knockback)
        {
            if (weapon.CountsAsClass(DamageClass.Ranged) && player.FargoSouls().Jammed)
                type = ProjectileID.ConfettiGun;

            //coin gun is broken as fucking shit codingwise so i'm fixing it
            if (weapon.type == ItemID.CoinGun)
            {
                if (ammo.type == ItemID.CopperCoin || ammo.type == ModContent.Find<ModItem>("Fargowiltas", "CopperCoinBag").Type)
                {
                    type = ProjectileID.CopperCoin;
                }
                if (ammo.type == ItemID.SilverCoin || ammo.type == ModContent.Find<ModItem>("Fargowiltas", "SilverCoinBag").Type)
                {
                    type = ProjectileID.SilverCoin;
                }
                if (ammo.type == ItemID.GoldCoin || ammo.type == ModContent.Find<ModItem>("Fargowiltas", "GoldCoinBag").Type)
                {
                    type = ProjectileID.GoldCoin;
                }
                if (ammo.type == ItemID.PlatinumCoin || ammo.type == ModContent.Find<ModItem>("Fargowiltas", "PlatinumCoinBag").Type)
                {
                    type = ProjectileID.PlatinumCoin;
                }
            }
        }

        public override void OnConsumeItem(Item item, Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (item.healLife > 0)
            {
                if (modPlayer.HallowEnchantItem != null)
                {
                    modPlayer.HallowHealTime = 6 * modPlayer.GetHealMultiplier(item.healLife);
                    HallowEnchant.HealRepel(player, modPlayer.HallowEnchantItem);
                }
                modPlayer.StatLifePrevious += modPlayer.GetHealMultiplier(item.healLife);
            }
        }

        public override bool ConsumeItem(Item item, Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            //if (item.makeNPC > 0 && (p.WoodForce || p.WizardEnchant) && Main.rand.NextBool())
            //{
            //    return false;
            //}

            if (modPlayer.BuilderMode && (item.createTile > TileID.Dirt || item.createWall > 0))
                return false;

            return base.ConsumeItem(item, player);
        }

        public static List<int> TungstenAlwaysAffects = new List<int>
        {
            ItemID.TerraBlade,
            ItemID.NightsEdge,
            ItemID.TrueNightsEdge,
            ItemID.Excalibur,
            ItemID.TrueExcalibur,
            ItemID.PiercingStarlight,
            ItemID.TheHorsemansBlade,
            ModContent.ItemType<TheBaronsTusk>()
        };
        public override void ModifyItemScale(Item item, Player player, ref float scale)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (player.HasEffect<TungstenEffect>()
                && !item.IsAir && item.damage > 0 && (!item.noMelee || TungstenAlwaysAffects.Contains(item.type)) && item.pick == 0 && item.axe == 0 && item.hammer == 0)
            {
                scale *= TungstenEffect.TungstenIncreaseWeaponSize(modPlayer);
            }
        }

        public override void ModifyWeaponKnockback(Item item, Player player, ref StatModifier knockback)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (modPlayer.UniverseSoul || modPlayer.Eternity)
                knockback *= 2;
        }

        public override bool? CanAutoReuseItem(Item item, Player player)
        {
            if (item.ModItem != null)
            {
                if (item.ModItem.CanAutoReuseItem(player) != null)
                    return item.ModItem.CanAutoReuseItem(player);
            }

            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (modPlayer.Berserked)
                return true;
            if (modPlayer.BoxofGizmos)
            {
                if (item.DamageType == DamageClass.Default && item.damage <= 0)
                {
                    return true;
                }
            }
            return base.CanAutoReuseItem(item, player);
        }

        public override bool CanUseItem(Item item, Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (modPlayer.NoUsingItems > 0)
                return false;

            //if (modPlayer.AdamantiteEnchantActive && modPlayer.AdamantiteCD == 0)
            //{
            //// ??? tm
            //}

            //dont use hotkeys in stasis
            if (player.HasBuff(ModContent.BuffType<GoldenStasisBuff>()))
            {
                if (item.type == ItemID.RodofDiscord)
                    player.ClearBuff(ModContent.BuffType<GoldenStasisBuff>());
                else
                    return false;
            }

            if (item.CountsAsClass(DamageClass.Magic) && player.FargoSouls().ReverseManaFlow)
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

            if (item.damage > 0 && player.HasAmmo(item) && !(item.mana > 0 && player.statMana < item.mana) //non weapons and weapons with no ammo begone
                && item.type != ItemID.ExplosiveBunny && item.type != ItemID.Cannonball
                && item.useTime > 0 && item.createTile == -1 && item.createWall == -1 && item.ammo == AmmoID.None && item.hammer == 0 && item.pick == 0 && item.axe == 0)
            {
                modPlayer.TryAdditionalAttacks(item.damage, item.DamageType);
            }

            //            //critter attack timer
            //            if (modPlayer.WoodEnchant && player.altFunctionUse == ItemAlternativeFunctionID.ActivatedAndUsed && item.makeNPC > 0)
            //            {
            //                if (modPlayer.CritterAttackTimer == 0)
            //                {
            //                    Vector2 vel = Vector2.Normalize(Main.MouseWorld - player.Center);
            //                    float damageMultiplier = player.GetDamage(DamageClass.Summon);

            //                    int type = -1;
            //                    int damage = 0;
            //                    int attackCooldown = 0;

            //                    switch (item.type)
            //                    {
            //                        //case ItemID.Bunny:
            //                        //    type = ProjectileID.ExplosiveBunny;
            //                        //    damage = 10;
            //                        //    attackCooldown = 10;
            //                        //    break;

            //                        case ItemID.Bird:
            //                            type = ModContent.ProjectileType<BirdProj>();
            //                            damage = 15;
            //                            attackCooldown = 15;
            //                            break;

            //                        case ItemID.BlueJay:
            //                            type = ModContent.ProjectileType<BlueJayProj>();
            //                            damage = 10;
            //                            attackCooldown = 10;
            //                            break;

            //                        case ItemID.Cardinal:
            //                            type = ModContent.ProjectileType<CardinalProj>();
            //                            damage = 20;
            //                            attackCooldown = 20;
            //                            break;
            //                    }

            //                    if (type != -1)
            //                    {
            //                        Projectile.NewProjectile(player.Center, vel * 2f, type, damage, 2, player.whoAmI);
            //                        modPlayer.CritterAttackTimer = attackCooldown;
            //                    }


            //                }





            //                return false;
            //            }

            if (item.type == ItemID.RodofDiscord && player.chaosState)
            {
                player.FargoSouls().WasHurtBySomething = true; //with abom rebirth, die to chaos state
            }
            /*
            if (item.type == ItemID.PotionOfReturn && FargoSoulsUtil.AnyBossAlive() && WorldSavingSystem.EternityMode)
            {
                return false;
            }
            */
            if (item.damage > 0 && item.DamageType != DamageClass.Default && item.pick == 0 && item.axe == 0 && item.hammer == 0)
            {
                player.FargoSouls().WeaponUseTimer = Math.Max(item.useTime, item.useAnimation) + item.reuseDelay + 6;
            }
            return true;
        }
        public override bool? UseItem(Item item, Player player)
        {
            if (item.type == ItemID.RodofDiscord)
            {
                player.ClearBuff(ModContent.BuffType<GoldenStasisBuff>());

                if (player.FargoSouls().CrystalEnchantActive)
                    player.AddBuff(ModContent.BuffType<FirstStrikeBuff>(), 60);
            }
            return base.UseItem(item, player);
        }

        //        public override bool AltFunctionUse(Item item, Player player)
        //        {
        //            FargoSoulsPlayer modPlayer = player.FargoSouls();

        //            if (modPlayer.WoodEnchant)
        //            {
        //                switch (item.type)
        //                {
        //                    case ItemID.Bunny:
        //                    case ItemID.Bird:
        //                    case ItemID.BlueJay:
        //                    case ItemID.Cardinal:
        //                        return true;

        //                }
        //            }



        //            return base.AltFunctionUse(item, player);
        //        }

        //        public override bool NewPreReforge(Item item)
        //        {
        //            /*if (Main.player[item.owner].FargoSouls().SecurityWallet)
        //            {
        //                switch(item.prefix)
        //                {
        //                    case PrefixID.Warding:  if (SoulConfig.Instance.walletToggles.Warding)  return false; break;
        //                    case PrefixID.Violent:  if (SoulConfig.Instance.walletToggles.Violent)  return false; break;
        //                    case PrefixID.Quick:    if (SoulConfig.Instance.walletToggles.Quick)    return false; break;
        //                    case PrefixID.Lucky:    if (SoulConfig.Instance.walletToggles.Lucky)    return false; break;
        //                    case PrefixID.Menacing: if (SoulConfig.Instance.walletToggles.Menacing) return false; break;
        //                    case PrefixID.Legendary:if (SoulConfig.Instance.walletToggles.Legendary)return false; break;
        //                    case PrefixID.Unreal:   if (SoulConfig.Instance.walletToggles.Unreal)   return false; break;
        //                    case PrefixID.Mythical: if (SoulConfig.Instance.walletToggles.Mythical) return false; break;
        //                    case PrefixID.Godly:    if (SoulConfig.Instance.walletToggles.Godly)    return false; break;
        //                    case PrefixID.Demonic:  if (SoulConfig.Instance.walletToggles.Demonic)  return false; break;
        //                    case PrefixID.Ruthless: if (SoulConfig.Instance.walletToggles.Ruthless) return false; break;
        //                    case PrefixID.Light:    if (SoulConfig.Instance.walletToggles.Light)    return false; break;
        //                    case PrefixID.Deadly:   if (SoulConfig.Instance.walletToggles.Deadly)   return false; break;
        //                    case PrefixID.Rapid:    if (SoulConfig.Instance.walletToggles.Rapid)    return false; break;
        //                    default: break;
        //                }
        //            }*/
        //            return true;
        //        }

        public override void HoldItem(Item item, Player player)
        {
            if (item.type == ItemID.Binoculars) //the amount of nesting here exists to prevent excessive lag
            {
                if (NPC.AnyNPCs(NPCID.TownCat))
                {
                    for (int j = 0; j < Main.maxNPCs; j++)
                    {
                        if (Main.npc[j].active && Main.npc[j].type == NPCID.TownCat)
                        {
                            NPC cat = Main.npc[j];
                            for (int i = 0; i < Main.maxItems; i++)
                            {
                                if (Main.item[i].active && Main.item[i].type == ItemID.CellPhone)
                                {
                                    if (cat.Distance(Main.item[i].Center) < cat.Size.Length() && Main.MouseWorld.Distance(cat.Center) < cat.Size.Length())
                                    {
                                        Item.NewItem(player.GetSource_ItemUse(item), cat.Center, ModContent.ItemType<WiresPainting>());
                                        Main.item[i].active = false;
                                        cat.active = false;
                                        return;
                                    }
                                }

                            }
                        }
                    }
                }
            }
            base.HoldItem(item, player);
        }
        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (modPlayer.Eternity)
                velocity *= 2;
            else if (modPlayer.UniverseSoul)
                velocity *= 1.5f;

        }

        //        public override bool ReforgePrice(Item item, ref int reforgePrice, ref bool canApplyDiscount)
        //        {
        //            if (Main.LocalPlayer.FargoSouls().SecurityWallet)
        //                reforgePrice /= 2;
        //            return true;
        //        }

        //        //summon variants
        //        private static readonly int[] Summon = { ItemID.NimbusRod, ItemID.CrimsonRod, ItemID.BeeGun, ItemID.WaspGun, ItemID.PiranhaGun, ItemID.BatScepter };

        //        public override bool CanRightClick(Item item)
        //        {
        //            if (Array.IndexOf(Summon, item.type) > -1)
        //            {
        //                return true;
        //            }

        //            return base.CanRightClick(item);
        //        }

        //        public override void RightClick(Item item, Player player)
        //        {
        //            int newType = -1;

        //            if (Array.IndexOf(Summon, item.type) > -1)
        //            {
        //                newType = mod.ItemType(ItemID.GetUniqueKey(item.type).Replace("Terraria ", string.Empty) + "Summon");
        //            }

        //            if (newType != -1)
        //            {
        //                int num = Item.NewItem(player.getRect(), newType, prefixGiven: item.prefix);

        //                if (Main.netMode == NetmodeID.MultiplayerClient)
        //                {
        //                    NetMessage.SendData(MessageID.SyncItem, number: num, number2: 1f);
        //                }
        //            }
        //        }

        public override bool WingUpdate(int wings, Player player, bool inUse)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if ((modPlayer.ChloroEnchantActive || modPlayer.ForceEffect<JungleEnchant>()) && player.GetToggleValue("Jungle") && inUse)
            {
                modPlayer.CanJungleJump = false;

                //spwn cloud
                if (modPlayer.JungleCD == 0 && modPlayer.JungleEnchantItem != null)
                {
                    bool jungleForceEffect = modPlayer.ForceEffect<JungleEnchant>();
                    if (modPlayer.JungleEnchantItem != null && modPlayer.ForceEffect(modPlayer.JungleEnchantItem.type))
                         jungleForceEffect = true;
                    int dmg = jungleForceEffect ? 150 : 75;
                    SoundEngine.PlaySound(SoundID.Item62 with { Volume = 0.5f }, player.Center);
                    FargoSoulsUtil.XWay(10, player.GetSource_Accessory(modPlayer.JungleEnchantItem), new Vector2(player.Center.X, player.Center.Y + player.height / 2), ProjectileID.SporeCloud, 3f, FargoSoulsUtil.HighestDamageTypeScaling(player, dmg), 0);

                    modPlayer.JungleCD = 8;
                }
            }

            if (modPlayer.BeeEnchantItem != null && player.GetToggleValue("Bee") && inUse)
            {
                if (modPlayer.BeeCD == 0)
                {
                    int damage = modPlayer.ForceEffect(modPlayer.BeeEnchantItem.type) ? 88 : 22; //22
                    Projectile.NewProjectile(player.GetSource_Accessory(modPlayer.BeeEnchantItem), player.Center, Vector2.Zero, ModContent.ProjectileType<BeeFlower>(), damage, 0.5f, player.whoAmI);
                    modPlayer.BeeCD = 50;
                }
                if (modPlayer.BeeCD > 0)
                    modPlayer.BeeCD--;
            }

            return base.WingUpdate(wings, player, inUse);
        }

        public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            base.VerticalWingSpeeds(item, player, ref ascentWhenFalling, ref ascentWhenRising, ref maxCanAscendMultiplier, ref maxAscentMultiplier, ref constantAscend);

            //Main.NewText($"vertical: {ascentWhenFalling} {ascentWhenRising} {maxCanAscendMultiplier} {maxAscentMultiplier} {constantAscend}");
        }

        public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
        {
            base.HorizontalWingSpeeds(item, player, ref speed, ref acceleration);

            //Main.NewText($"horiz: {speed} {acceleration}");
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            /*if (Array.IndexOf(Summon, item.type) > -1)
            {
                TooltipLine helperLine = new TooltipLine(mod, "help", "Right click to convert");
                tooltips.Add(helperLine);
            }*/
        }

        static int infiniteLoopHackFix;

        public override bool AllowPrefix(Item item, int pre)
        {
            if (!Main.gameMenu && Main.LocalPlayer.active && Main.LocalPlayer.FargoSouls().SecurityWallet)
            {
                switch (pre)
                {
                    #region actually bad

                    case PrefixID.Hard:
                    case PrefixID.Guarding:
                    case PrefixID.Jagged:
                    case PrefixID.Spiked:
                    case PrefixID.Brisk:
                    case PrefixID.Fleeting:
                    case PrefixID.Wild:
                    case PrefixID.Rash:

                    case PrefixID.Broken:
                    case PrefixID.Damaged:
                    case PrefixID.Shoddy:
                    case PrefixID.Weak:

                    case PrefixID.Slow:
                    case PrefixID.Sluggish:
                    case PrefixID.Lazy:
                    case PrefixID.Annoying:

                    case PrefixID.Tiny:
                    case PrefixID.Small:
                    case PrefixID.Dull:
                    case PrefixID.Shameful:
                    case PrefixID.Terrible:
                    case PrefixID.Unhappy:

                    case PrefixID.Awful:
                    case PrefixID.Lethargic:
                    case PrefixID.Awkward:

                    case PrefixID.Inept:
                    case PrefixID.Ignorant:
                    case PrefixID.Deranged:

                    #endregion actually bad

                    #region mediocre

                    case PrefixID.Hasty:
                    case PrefixID.Intrepid:

                    case PrefixID.Intense:
                    case PrefixID.Frenzying:
                    case PrefixID.Dangerous:
                    case PrefixID.Bulky:
                    case PrefixID.Heavy:
                    case PrefixID.Sighted:
                    case PrefixID.Adept:
                    case PrefixID.Taboo:
                    case PrefixID.Furious:
                    case PrefixID.Keen:
                    case PrefixID.Forceful:
                    case PrefixID.Quick:
                    case PrefixID.Nimble:
                    case PrefixID.Nasty:
                    case PrefixID.Manic:
                    case PrefixID.Strong:
                    case PrefixID.Zealous:
                    case PrefixID.Large:
                    case PrefixID.Intimidating:
                    case PrefixID.Unpleasant:

                        #endregion mediocre

                        if (++infiniteLoopHackFix < 30)
                            return false;
                        else
                            break;

                    default:
                        break;
                }
            }

            infiniteLoopHackFix = 0;

            return base.AllowPrefix(item, pre);
        }
    }
}