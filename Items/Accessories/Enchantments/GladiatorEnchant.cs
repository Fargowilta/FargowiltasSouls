using FargowiltasSouls.Projectiles.Souls;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class GladiatorEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Gladiator Enchantment");
            Tooltip.SetDefault(
@"Spears will rain down on struck enemies
Grants knockback immunity when you are facing the attack
'Are you not entertained?'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "角斗士魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
            // @"长矛将倾泄在被攻击的敌人身上
            // '难道你不高兴吗？'");
        }

        protected override Color nameColor => new Color(156, 146, 78);
        public override string wizardEffect => Language.GetTextValue("Mods.FargowiltasSouls.WizardEffect.Gladiator");

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Green;
            Item.value = 40000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            GladiatorEffect(player);
        }

        public static void GladiatorEffect(Player player)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.GladiatorEnchantActive = true;

            if (modPlayer.GladiatorCD > 0)
            {
                modPlayer.GladiatorCD--;
            }
        }

        public static void GladiatorSpearDrop(FargoSoulsPlayer modPlayer, Item item, Projectile projectile, NPC target, int damage)
        {
            Player player = modPlayer.Player;
            int spearDamage = projectile != null ? projectile.damage : item != null ? item.damage : damage;
            spearDamage /= 4;

            if (spearDamage > 0)
            {
                if (!modPlayer.TerrariaSoul)
                    spearDamage = Math.Min(spearDamage, FargoSoulsUtil.HighestDamageTypeScaling(player, 300));

                for (int i = 0; i < 4; i++)
                {
                    Vector2 spawn = new Vector2(target.Center.X + Main.rand.NextFloat(-300, 300), target.Center.Y - Main.rand.Next(600, 801));

                    Vector2 speed = target.Center + target.velocity * i * 5 * Main.rand.NextFloat(0.5f, 1.5f) - spawn;
                    speed.Normalize();
                    speed *= 15f * Main.rand.NextFloat(0.8f, 1.2f);

                    Projectile.NewProjectile(player.GetSource_Misc(""), spawn, speed, ModContent.ProjectileType<GladiatorJavelin>(), spearDamage, 4f, Main.myPlayer);
                }

                modPlayer.GladiatorCD = modPlayer.WillForce ? 10 : 30;
            }
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GladiatorHelmet)
                .AddIngredient(ItemID.GladiatorBreastplate)
                .AddIngredient(ItemID.GladiatorLeggings)
                .AddIngredient(ItemID.Spear)
                .AddIngredient(ItemID.Gladius)
                .AddIngredient(ItemID.BoneJavelin, 300)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
