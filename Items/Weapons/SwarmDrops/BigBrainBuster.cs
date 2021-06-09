using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Weapons.SwarmDrops
{
    public class BigBrainBuster : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Big Brain Buster");
            Tooltip.SetDefault("Repeated summons increase the size and damage of the minion \nThis caps at 6 slots\nMinions do reduced damage when not holding a summon weapon\n'The reward for slaughtering many...'");
            DisplayName.AddTranslation(GameCulture.Chinese, "大脑破坏者");
            Tooltip.AddTranslation(GameCulture.Chinese, "重复召唤会增加大脑仆从的大小和伤害而非召唤额外的召唤物 \n在使用的召唤栏超过6格后，再次召唤时所获得的大小和伤害会降低\n未手持召唤武器时会降低大脑仆从的伤害\n'屠戮众多的奖励...'");
            ItemID.Sets.StaffMinionSlotsRequired[item.type] = 2;
        }

        public override void SetDefaults()
        {
            item.damage = 200;
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
            item.shoot = mod.ProjectileType("BigBrainProj");
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
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.owner == player.whoAmI && proj.minionSlots > 0 && proj.active)
                    {
                        usedslots += proj.minionSlots;
                        if (usedslots < player.maxMinions && proj.type == type)
                        {
                            proj.minionSlots++;
                        }
                    }
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
