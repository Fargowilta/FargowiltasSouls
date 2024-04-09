using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class TinEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(162, 139, 78);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 30000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<TinEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TinHelmet)
                .AddIngredient(ItemID.TinChainmail)
                .AddIngredient(ItemID.TinGreaves)
                .AddIngredient(ItemID.TinBow)
                .AddIngredient(ItemID.Revolver)
                .AddIngredient(ItemID.TopazStaff)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
    public class TinEffect : AccessoryEffect
    {

        public override Header ToggleHeader => Header.GetHeader<TerraHeader>();
        public override int ToggleItemType => ModContent.ItemType<TinEnchant>();
        public override bool IgnoresMutantPresence => true;

        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.Eternity)
            {
                if (modPlayer.TinEternityDamage > 47.5f)
                    modPlayer.TinEternityDamage = 47.5f;

                if (player.HasEffect<EternityTin>())
                {
                    player.GetDamage(DamageClass.Generic) += modPlayer.TinEternityDamage;
                    player.statDefense += (int)(modPlayer.TinEternityDamage * 100); //10 defense per .1 damage
                }
            }

            if (modPlayer.TinProcCD > 0)
                modPlayer.TinProcCD--;
        }
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            TinOnHitEnemy(player, hitInfo);
        }
        // increase crit
        public static void TinOnHitEnemy(Player player, NPC.HitInfo hitInfo)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (hitInfo.Crit)
                modPlayer.TinCritBuffered = true;

            if (modPlayer.TinCritBuffered && modPlayer.TinProcCD <= 0)
            {
                modPlayer.TinCritBuffered = false;
                modPlayer.TinCrit += 5;
                if (modPlayer.TinCrit > modPlayer.TinCritMax)
                    modPlayer.TinCrit = modPlayer.TinCritMax;
                else
                    CombatText.NewText(modPlayer.Player.Hitbox, Color.Yellow, Language.GetTextValue("Mods.FargowiltasSouls.Items.TinEnchant.CritUp"));


                void TryHeal(int healDenominator, int healCooldown)
                {
                    int amountToHeal = hitInfo.Damage / healDenominator;
                    if (modPlayer.TinCrit >= 100 && modPlayer.HealTimer <= 0 && !modPlayer.Player.moonLeech && !modPlayer.MutantNibble && amountToHeal > 0)
                    {
                        modPlayer.HealTimer = healCooldown;
                        modPlayer.Player.statLife = Math.Min(modPlayer.Player.statLife + amountToHeal, modPlayer.Player.statLifeMax2);
                        modPlayer.Player.HealEffect(amountToHeal);
                    }
                }

                if (modPlayer.Eternity)
                {
                    modPlayer.TinProcCD = 1;
                    TryHeal(10, 1);
                    modPlayer.TinEternityDamage += .05f;
                }
                else if (modPlayer.TerrariaSoul)
                {
                    modPlayer.TinProcCD = 15;
                    TryHeal(25, 10);
                }
                else if (modPlayer.ForceEffect<TinEnchant>())
                {
                    modPlayer.TinProcCD = 30;
                }
                else
                {
                    modPlayer.TinProcCD = 60;
                }
            }
        }
        public override void OnHurt(Player player, Player.HurtInfo info)
        {
            TinHurt(player);
        }
        //reset crit
        public static void TinHurt(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            float oldCrit = modPlayer.TinCrit;
            if (modPlayer.Eternity)
            {
                modPlayer.TinCrit = 50;
                modPlayer.TinEternityDamage = 0;
            }
            else if (modPlayer.TerrariaSoul)
            {
                modPlayer.TinCrit = 20;
            }
            else if (modPlayer.ForceEffect<TinEnchant>())
            {
                modPlayer.TinCrit = 10;
            }
            else
            {
                modPlayer.TinCrit = 5;
            }

            double diff = Math.Round(oldCrit - modPlayer.TinCrit, 1);
            if (diff > 0)
                CombatText.NewText(modPlayer.Player.Hitbox, Color.OrangeRed, Language.GetTextValue("Mods.FargowiltasSouls.Items.TinEnchant.CritReset", diff), true);
        }
        public override void PostUpdateMiscEffects(Player player)
        {
            TinPostUpdate(player);
        }
        //set max crit and current crit with no interference from accessory order
        public static void TinPostUpdate(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            modPlayer.TinCritMax = Math.Max(FargoSoulsUtil.HighestCritChance(modPlayer.Player) * 2, modPlayer.ForceEffect<TinEnchant>() ? 50 : 15);

            if (modPlayer.TinCritMax > 100)
                modPlayer.TinCritMax = 100;

            FargoSoulsUtil.AllCritEquals(modPlayer.Player, modPlayer.TinCrit);
        }
    }
}
