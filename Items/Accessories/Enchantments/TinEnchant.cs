using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class TinEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Tin Enchantment");

            string tooltip =
@"Sets your critical strike chance to 5%
Every crit will increase it by 5% up to double your critical strike chance or 15%
Getting hit resets your crit to 5%
'Return of the Crit'";
            Tooltip.SetDefault(tooltip);

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "锡魔石");
            //             string tooltip_ch =
            // @"将你的基础暴击率设为5%
            // 每次暴击时都会增加5%暴击率，增加的暴击率的最大值为你当前最大暴击率数值x2
            // 被击中后会降低暴击率
            // '暴击回归'";
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }

        protected override Color nameColor => new Color(162, 139, 78);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 30000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TinEffect(player);
        }

        public static void TinEffect(Player player)
        {
            if (!player.GetToggleValue("Tin", false)) return;

            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.TinEnchantActive = true;

            if (modPlayer.Eternity)
            {
                if (modPlayer.TinEternityDamage > 47.5f)
                    modPlayer.TinEternityDamage = 47.5f;

                if (player.GetToggleValue("Eternity", false))
                {
                    player.GetDamage(DamageClass.Generic) += modPlayer.TinEternityDamage;
                    player.statDefense += (int)(modPlayer.TinEternityDamage * 100); //10 defense per .1 damage
                }
            }

            if (modPlayer.TinProcCD > 0)
                modPlayer.TinProcCD--;
        }

        //set max crit and current crit with no interference from accessory order
        public static void TinPostUpdate(FargoSoulsPlayer modPlayer)
        {
            modPlayer.TinCritMax = Math.Max(FargoSoulsUtil.HighestCritChance(modPlayer.Player) * 2, 15);

            if (modPlayer.TinCritMax > 100)
                modPlayer.TinCritMax = 100;

            FargoSoulsUtil.AllCritEquals(modPlayer.Player, modPlayer.TinCrit);
        }

        //increase crit
        public static void TinOnHitEnemy(FargoSoulsPlayer modPlayer, int damage, bool crit)
        {
            if (modPlayer.TinEnchantActive)
            {
                if (crit)
                    modPlayer.TinCritBuffered = true;

                if (modPlayer.TinCritBuffered && modPlayer.TinProcCD <= 0)
                {
                    modPlayer.TinCritBuffered = false;
                    modPlayer.TinCrit += 5;
                    if (modPlayer.TinCrit > modPlayer.TinCritMax)
                        modPlayer.TinCrit = modPlayer.TinCritMax;
                    else
                        CombatText.NewText(modPlayer.Player.Hitbox, Color.Yellow, Language.GetTextValue("Mods.FargowiltasSouls.ItemExtra.TinCritUp"));


                    void TryHeal(int healDenominator, int healCooldown)
                    {
                        int amountToHeal = damage / healDenominator;
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
                    else if (modPlayer.TerraForce)
                    {
                        modPlayer.TinProcCD = 30;
                    }
                    else
                    {
                        modPlayer.TinProcCD = 60;
                    }
                }
            }
        }

        //reset crit
        public static void TinHurt(FargoSoulsPlayer modPlayer)
        {
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
            else if (modPlayer.TerraForce)
            {
                modPlayer.TinCrit = 10;
            }
            else
            {
                modPlayer.TinCrit = 5;
            }

            double diff = Math.Round(oldCrit - modPlayer.TinCrit, 1);
            if (diff > 0)
                CombatText.NewText(modPlayer.Player.Hitbox, Color.OrangeRed, Language.GetTextValue("Mods.FargowiltasSouls.ItemExtra.TinCritReset", diff), true);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TinHelmet)
                .AddIngredient(ItemID.TinChainmail)
                .AddIngredient(ItemID.TinGreaves)
                .AddIngredient(ItemID.TinBow)
                .AddIngredient(ItemID.Musket)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
