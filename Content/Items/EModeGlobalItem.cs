using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Systems;
using Terraria.DataStructures;
using Microsoft.CodeAnalysis;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Content.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.Localization;

namespace FargowiltasSouls.Content.Items
{
	public class EModeGlobalItem : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(item, tooltips);
            if (!WorldSavingSystem.EternityMode)
            {
                return;
            }
            EmodeItemBalance.BalanceTooltips(item, ref tooltips);
            if (item.prefix >= PrefixID.Hard && item.prefix <= PrefixID.Warding)
            {
                int life = 5;
                foreach (TooltipLine tooltip in tooltips)
                {
                    if (tooltip.Name == "PrefixAccDefense")
                    {
                        if (!Main.hardMode)
                        {
                            List<char> text = tooltip.Text.ToList();
                            text[1] = (char)((int)text[1] - 1);
                            tooltip.Text = new string(text.ToArray());
                        }
                        
                        tooltip.Text += "\n" + Language.GetTextValue("Mods.FargowiltasSouls.Items.Extra.DefensePrefixMaxLife", life);
                    }
                }
            }
        }
        public override void PickAmmo(Item weapon, Item ammo, Player player, ref int type, ref float speed, ref StatModifier damage, ref float knockback)
        {
            if (!WorldSavingSystem.EternityMode)
                return;
            //ammo nerf
            //if (ammo.ammo == AmmoID.Arrow || ammo.ammo == AmmoID.Bullet || ammo.ammo == AmmoID.Dart)
            //{
            //    damage -= (int)Math.Round(ammo.damage * player.GetDamage(DamageClass.Ranged).Additive * 0.5, MidpointRounding.AwayFromZero); //always round up
            //}
        }
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            base.UpdateAccessory(item, player, hideVisual);
            if (!WorldSavingSystem.EternityMode)
            {
                return;
            }
            if (item.prefix >= PrefixID.Hard && item.prefix <= PrefixID.Warding)
            {
                if (!Main.hardMode)
                {
                    player.statDefense -= 1;
                }
                player.statLifeMax2 += 5;
            }
            if (item.type == ItemID.JungleRose)
            {
                player.FargoSouls().HasJungleRose = true;
            }
        }
        public override void HoldItem(Item item, Player player)
        {
            if (!WorldSavingSystem.EternityMode)
            {
                base.HoldItem(item, player);
                return;
            }
            EModePlayer ePlayer = player.Eternity();
            if (item.type == ItemID.MythrilHalberd || item.type == ItemID.MythrilSword)
            {
                if (!player.ItemAnimationActive)
                {
                    if (ePlayer.MythrilHalberdTimer < 121)
                        ePlayer.MythrilHalberdTimer++;
                }
                if (player.itemAnimation == 1) //reset on last frame
                {
                    ePlayer.MythrilHalberdTimer = 0;
                }

                if (ePlayer.MythrilHalberdTimer == 120 && player.whoAmI == Main.myPlayer)
                {
                    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/ChargeSound"), player.Center);
                }
            }
            else
            {
                ePlayer.MythrilHalberdTimer = 0;
            }
            base.HoldItem(item, player);
        }
        public override bool CanUseItem(Item item, Player player)
        {
            if (!WorldSavingSystem.EternityMode)
            {
                if (item.type == ItemID.OrichalcumSword) //reset stats to default
                {
                    Item dummy = new Item(ItemID.OrichalcumSword);
                    item.shoot = dummy.shoot;
                    item.shootSpeed = dummy.shootSpeed;
                    dummy.active = false;
                    dummy = null;
                }
                return base.CanUseItem(item, player);
            }
                

            EModePlayer ePlayer = player.Eternity();

            if (item.damage <= 0 && (item.type == ItemID.RodofDiscord || item.type == ItemID.ActuationRod || item.type == ItemID.WireKite || item.type == ItemID.WireCutter || item.type == ItemID.Wrench || item.type == ItemID.BlueWrench || item.type == ItemID.GreenWrench || item.type == ItemID.MulticolorWrench || item.type == ItemID.YellowWrench || item.type == ItemID.Actuator))
            {
                //either player is affected by lihzahrd curse, or cursor is targeting a place in temple (player standing outside)
                if (player.FargoSouls().LihzahrdCurse || Framing.GetTileSafely(Main.MouseWorld).WallType == WallID.LihzahrdBrickUnsafe && !player.buffImmune[ModContent.BuffType<LihzahrdCurseBuff>()])
                    return false;
            }

            if (item.type == ItemID.RodofDiscord && LumUtils.AnyBosses())
            {
                player.chaosState = true;
            }

            if (item.type == ItemID.OrichalcumSword)
            {
                item.shoot = ProjectileID.FlowerPetal;
                item.shootSpeed = 5;
            }
            if (item.type == ItemID.CobaltSword)
            {
                ePlayer.CobaltHitCounter = 0;
            }

            if (item.type == ItemID.RodOfHarmony && LumUtils.AnyBosses())
            {
                player.hurtCooldowns[0] = 0;
                var defense = player.statDefense;
                float endurance = player.endurance;
                player.statDefense.FinalMultiplier *= 0;
                player.endurance = 0;
                player.Hurt(PlayerDeathReason.ByCustomReason(Language.GetTextValue("Mods.FargowiltasSouls.DeathMessage.RodOfHarmony", player.name)), player.statLifeMax2 / 7, 0, false, false, 0, false);
                player.statDefense = defense;
                player.endurance = endurance;
                
            }
            //TODO: mana pot rework
            /*
            if (item.healMana > 0)
            {
                return !player.HasBuff(BuffID.ManaSickness);
            }
            */
            return base.CanUseItem(item, player);
        }
        //TODO: mana pot rework
        /*
        public override void GetHealMana(Item item, Player player, bool quickHeal, ref int healValue)
        {
            if (WorldSavingSystem.EternityMode)
            {
                healValue = (int)(healValue * 1.5f);
            }
            base.GetHealMana(item, player, quickHeal, ref healValue);
        }
        */
        public override bool? UseItem(Item item, Player player)
        {
            if (!WorldSavingSystem.EternityMode)
                return base.UseItem(item, player);
            EModePlayer ePlayer = player.Eternity();

            if (item.type == ItemID.MechdusaSummon && Main.zenithWorld)
            {
                Main.time = 18000;
            }
            
            return base.UseItem(item, player);
        }
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            if (player.Eternity().MythrilHalberdTimer >= 120 && (item.type == ItemID.MythrilSword))
            {
                damage *= 8 * player.FargoSouls().AttackSpeed;
            }
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!WorldSavingSystem.EternityMode)
                return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
            switch (item.type)
            {
                case ItemID.OrichalcumSword:
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            int p = Projectile.NewProjectile(player.GetSource_ItemUse(item), position, velocity.RotatedByRandom(MathHelper.Pi / 14), type, (int)(damage * 0.75f), knockback / 2, Main.myPlayer);
                            /*
                            if (p != Main.maxProjectiles)
                            {
                                Projectile proj = Main.projectile[p];
                                if (proj != null && proj.active)
                                {
                                    proj.scale = item.scale;
                                }
                            }
                            */
                        }
                        return false;
                    }
            }
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }
        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!WorldSavingSystem.EternityMode)
                return;
            EModePlayer ePlayer = player.Eternity();
            switch (item.type)
            {
                case ItemID.CobaltSword:
                    if (ePlayer.CobaltHitCounter < 2) //only twice per swing
                    {
                        Projectile p = FargoSoulsUtil.NewProjectileDirectSafe(player.GetSource_OnHit(target), target.position + Vector2.UnitX * Main.rand.Next(target.width) + Vector2.UnitY * Main.rand.Next(target.height), Vector2.Zero, ModContent.ProjectileType<CobaltExplosion>(), (int)(hit.Damage * 0.4f), 0f, Main.myPlayer);
                        if (p != null)
                            p.FargoSouls().CanSplit = false;
                        ePlayer.CobaltHitCounter++;
                    }
                    break;
                case ItemID.PalladiumSword:
                    {
                        if (target.type != NPCID.TargetDummy && !target.friendly) //may add more checks here idk
                            player.AddBuff(BuffID.RapidHealing, 60 * 5);
                        break;
                    }
            }
        }
        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (!WorldSavingSystem.EternityMode)
                return;
            if (!NPC.downedBoss3 && item.type == ItemID.WaterBolt)
            {
                type = ProjectileID.WaterGun;
                damage = 0;
            }
            if (!NPC.downedBoss2 && item.type == ItemID.SpaceGun)
            {
                type = ProjectileID.ConfettiGun;
                damage = 0;
            }

            if (player.Eternity().MythrilHalberdTimer >= 120 && (item.type == ItemID.MythrilHalberd))
            {
                damage = (int)(damage * 8 * player.FargoSouls().AttackSpeed);
                player.Eternity().MythrilHalberdTimer = 0;
            }
        }
    }
}