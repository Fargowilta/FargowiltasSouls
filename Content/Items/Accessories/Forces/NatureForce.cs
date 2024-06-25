using Fargowiltas.Items.Tiles;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Projectiles.Minions;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Forces
{
    public class NatureForce : BaseForce
    {
        public override void SetStaticDefaults()
        {
            Enchants[Type] =
            [
                ModContent.ItemType<CrimsonEnchant>(),
                ModContent.ItemType<MoltenEnchant>(),
                ModContent.ItemType<RainEnchant>(),
                ModContent.ItemType<FrostEnchant>(),
                ModContent.ItemType<ChlorophyteEnchant>(),
                ModContent.ItemType<ShroomiteEnchant>()
            ];
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SetActive(player);

            player.AddEffect<CrimsonEffect>(Item);

            float bonus = 4f * player.statLife / 50;
            bonus /= 100; // percent to fraction
            player.GetDamage(DamageClass.Generic) += bonus;

            player.AddEffect<NatureBeamEffect>(Item);
        }
        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            float bonus = 4f * Main.LocalPlayer.statLife / 50;
            int i = tooltips.FindIndex(line => line.Name == "Tooltip4");
            if (i != -1)
                tooltips[i].Text = string.Format(tooltips[i].Text, bonus);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            foreach (int ench in Enchants[Type])
                recipe.AddIngredient(ench);
            recipe.AddTile<CrucibleCosmosSheet>();
            recipe.Register();
        }
    }
    public class NatureBeamEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<NatureHeader>();
        public override int ToggleItemType => ModContent.ItemType<NatureForce>();
        public override bool ExtraAttackEffect => true;
        public override bool IgnoresMutantPresence => true;
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            int increment = player.statLife - modPlayer.StatLifePrevious;
            if (increment > 0)
            {
                modPlayer.NatureHealCounter += increment;
            }
            if (modPlayer.NatureHealCD > 0)
                modPlayer.NatureHealCD--;
            if (modPlayer.NatureHealCounter > 5 && modPlayer.NatureHealCD <= 0)
            {

                int damage = 1000;

                //trying to shoot
                float num396 = player.position.X;
                float num397 = player.position.Y;
                float num398 = 700f;
                bool flag11 = false;

                NPC npc = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPC(player.Center, 2000, true, true));
                if (npc != null)
                {
                    num396 = npc.Center.X;
                    num397 = npc.Center.Y;
                    num398 = player.Distance(npc.Center);
                    flag11 = true;
                }

                //shoot
                if (flag11)
                {
                    Vector2 vector29 = new(player.position.X + player.width * 0.5f, player.position.Y + player.height * 0.5f);
                    float num404 = num396 - vector29.X;
                    float num405 = num397 - vector29.Y;
                    float num406 = (float)Math.Sqrt(num404 * num404 + num405 * num405);
                    num406 = 10f / num406;
                    num404 *= num406;
                    num405 *= num406;
                    if (player.whoAmI == Main.myPlayer)
                        Projectile.NewProjectile(GetSource_EffectItem(player), player.Center, new Vector2(num404, num405), ProjectileID.CrystalLeafShot, damage, 1f, player.whoAmI);

                    modPlayer.NatureHealCD = 20;
                    modPlayer.NatureHealCounter -= 5;
                    
                }
            }
        }
    }
}
