using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.SwarmDrops
{
    public class BigBrainBuster : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Big Brain Buster");            
            ItemID.Sets.StaffMinionSlotsRequired[item.type] = 1;
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            string Tooltip = Language.GetTextValue("Mods.FargowiltasSouls.BigBrainBuster.Tooltip1") + $"{Projectiles.Minions.BigBrainProj.MaxMinionSlots}" + Language.GetTextValue("Mods.FargowiltasSouls.BigBrainBuster.Tooltip2");
            tooltips.Add(new TooltipLine(mod, "tooltip", Tooltip));
        }

        public override void SetDefaults()
        {
            item.damage = 222;
            item.summon = true;
            item.mana = 10;
            item.width = 26;
            item.height = 28;
            item.useTime = 36;
            item.useAnimation = 36;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 3;
            item.rare = ItemRarityID.Purple;
            item.UseSound = SoundID.Item44;
            item.shoot = ModContent.ProjectileType<Projectiles.Minions.BigBrainProj>();
            item.shootSpeed = 10f;
            //item.buffType = mod.BuffType("BigBrainMinion");
            //item.buffTime = 3600;
            item.autoReuse = true;
            item.value = Item.sellPrice(0, 10);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            player.AddBuff(mod.BuffType("BigBrainMinion"), 2);
            Vector2 spawnPos = player.Center - Main.MouseWorld;
            if (player.ownedProjectileCounts[type] == 0)
            {
                Projectile.NewProjectile(player.Center, Vector2.Zero, type, damage, knockBack, player.whoAmI, 0, spawnPos.ToRotation());
            }
            else
            {
                float usedslots = 0;
                int brain = -1;
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.owner == player.whoAmI && proj.minionSlots > 0 && proj.active)
                    {
                        usedslots += proj.minionSlots;
                        if (proj.type == type)
                        {
                            brain = i;
                            if (usedslots < player.maxMinions)
                                proj.minionSlots++;
                        }
                    }
                }

                if (player.GetModPlayer<FargoPlayer>().TikiMinion && usedslots > player.GetModPlayer<FargoPlayer>().actualMinions && FargoSoulsUtil.ProjectileExists(brain, type) != null)
                {
                    Main.projectile[brain].GetGlobalProjectile<Projectiles.FargoGlobalProjectile>().tikiMinion = true;
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BrainStaff");
            recipe.AddIngredient(ModLoader.GetMod("Fargowiltas").ItemType("EnergizerBrain"));
            recipe.AddIngredient(ItemID.LunarBar, 10);

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}