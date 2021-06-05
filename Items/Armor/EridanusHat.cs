using FargowiltasSouls.Items.Misc;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class EridanusHat : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eridanus Hat");
            Tooltip.SetDefault(@"5% increased damage
5% increased critical strike chance
Increases your max number of minions by 3
Increases your max number of sentries by 2");
            DisplayName.AddTranslation(GameCulture.Chinese, "宇宙英灵帽");
            Tooltip.AddTranslation(GameCulture.Chinese, @"增加5%伤害
增加5%暴击率
+3最大召唤栏
+2最大哨兵栏");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.rare = ItemRarityID.Purple;
            item.value = Item.sellPrice(0, 14);
            item.defense = 20;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<FargoPlayer>().AllDamageUp(0.05f);
            player.GetModPlayer<FargoPlayer>().AllCritUp(5);

            player.maxMinions += 3;
            player.maxTurrets += 2;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<EridanusBattleplate>() && legs.type == ModContent.ItemType<EridanusLegwear>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
        if (Language.ActiveCulture == GameCulture.Chinese)
            {
            player.setBonus = @"厄里达诺斯的祝福强化了你的攻击
被强化的职业每20秒切换一次（战士、射手、法师和召唤师）
当你使用被强化的职业的武器时，厄里达诺斯会协助你作战
被强化的职业的伤害增加50%
增加20%武器使用速度";
            }
            else
            {
            player.setBonus = @"The blessing of Eridanus empowers your attacks
The empowered class changes every 20 seconds
Eridanus fights alongside you when you use the empowered class
50% increased damage for the empowered class
20% increased weapon use speed";
            }
            FargoPlayer fargoPlayer = player.GetModPlayer<FargoPlayer>();
            fargoPlayer.EridanusEmpower = true;

            if (fargoPlayer.EridanusTimer % (60 * 20) == 1) //make dust whenever changing classes
            {
                Main.PlaySound(SoundID.Item4, player.Center);

                int type;
                switch (fargoPlayer.EridanusTimer / (60 * 20))
                {
                    case 0: type = 127; break; //solar
                    case 1: type = 229; break; //vortex
                    case 2: type = 242; break; //nebula
                    default: type = 135; break; //stardust
                }

                const int max = 100; //make some indicator dusts
                for (int i = 0; i < max; i++)
                {
                    Vector2 vector6 = Vector2.UnitY * 20f;
                    vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + Main.LocalPlayer.Center;
                    Vector2 vector7 = vector6 - Main.LocalPlayer.Center;
                    int d = Dust.NewDust(vector6 + vector7, 0, 0, type, 0f, 0f, 0, default(Color), 3f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity = vector7;
                }

                for (int i = 0; i < 50; i++) //make some indicator dusts
                {
                    int d = Dust.NewDust(player.position, player.width, player.height, type, 0f, 0f, 0, default(Color), 2.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].noLight = true;
                    Main.dust[d].velocity *= 24f;
                }

                if (Main.myPlayer == player.whoAmI)
                {
                    for (int i = 0; i < Main.maxProjectiles; i++) //clear minions
                    {
                        if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI
                            && Main.projectile[i].type != ModContent.ProjectileType<Projectiles.Minions.EridanusMinion>()
                            && (Main.projectile[i].minionSlots > 0 || Main.projectile[i].minion))
                        {
                            Main.projectile[i].Kill();
                        }
                    }
                }
            }

            if (++fargoPlayer.EridanusTimer > 60 * 20 * 4) //handle loop
            {
                fargoPlayer.EridanusTimer = 0;
            }

            switch (fargoPlayer.EridanusTimer / (60 * 20)) //damage boost according to current class
            {
                case 0: player.meleeDamage += 0.5f; break;
                case 1: player.rangedDamage += 0.5f; break;
                case 2: player.magicDamage += 0.5f; break;
                default: player.minionDamage += 0.5f; break;
            }

            fargoPlayer.AttackSpeed += .2f;

            if (player.whoAmI == Main.myPlayer)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Minions.EridanusMinion>()] < 1)
                {
                    Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Minions.EridanusMinion>(), 220, 12f, player.whoAmI, -1);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Minions.EridanusRitual>()] < 1)
                {
                    Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Minions.EridanusRitual>(), 0, 0f, player.whoAmI);
                }
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<LunarCrystal>(), 5);
            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
