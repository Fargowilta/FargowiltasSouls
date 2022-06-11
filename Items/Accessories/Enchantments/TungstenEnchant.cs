using FargowiltasSouls.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class TungstenEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Tungsten Enchantment");

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "钨魔石");

            string tooltip =
@"150% increased weapon size but reduces melee speed
Every half second a projectile will be doubled in size
Enlarged projectiles deal 10% more damage and have an additional chance to crit
'Bigger is always better'";
            Tooltip.SetDefault(tooltip);

            //             string tooltip_ch =
            // @"增加150%剑的尺寸
            // 每过0.5秒便会使一个弹幕的尺寸翻倍
            // 尺寸变大的剑和弹幕会额外造成10%伤害并且有额外几率暴击
            // '大就是好'";
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }

        protected override Color nameColor => new Color(176, 210, 178);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 40000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TungstenEffect(player);
        }

        public static void TungstenEffect(Player player)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            modPlayer.TungstenEnchantActive = true;

            if (!modPlayer.TerrariaSoul && player.HeldItem.damage > 0 && player.HeldItem.CountsAsClass(DamageClass.Melee) && !player.HeldItem.noMelee && player.HeldItem.pick == 0 && player.HeldItem.axe == 0 && player.HeldItem.hammer == 0)
            {
                modPlayer.Player.GetAttackSpeed(DamageClass.Melee) -= 0.5f;
            }
        }

        public static float TungstenIncreaseWeaponSize(FargoSoulsPlayer modPlayer)
        {
            //float tungstenScale = modPlayer.TerraForce ? 4f : 2.5f;

            //if (heldItem.damage > 0 && !heldItem.noMelee)
            //{
            //modPlayer.TungstenPrevSizeSave = item.scale;
            //item.scale *= tungstenScale;
            //modPlayer.TungstenEnlargedItem = item;
            //}
            //else if (((modPlayer.Toggler != null && !player.GetToggleValue("Tungsten", false)) || !TungstenEnchant) && modPlayer.TungstenPrevSizeSave != -1)
            //{
            //    heldItem.scale = modPlayer.TungstenPrevSizeSave;
            //}

            return modPlayer.TerraForce ? 4f : 2.5f;
        }

        public static bool TungstenAlwaysAffectProj(Projectile projectile)
        {
            return projectile.aiStyle == ProjAIStyleID.Spear
                || projectile.aiStyle == ProjAIStyleID.Yoyo
                || projectile.aiStyle == ProjAIStyleID.ShortSword
                || projectile.aiStyle == ProjAIStyleID.Flail
                || ProjectileID.Sets.IsAWhip[projectile.type]
                || projectile.type == ProjectileID.MonkStaffT2
                || projectile.type == ProjectileID.Arkhalis
                || projectile.type == ProjectileID.Terragrim
                || projectile.type == ProjectileID.PiercingStarlight;
        }

        public static void TungstenIncreaseProjSize(Projectile projectile, FargoSoulsPlayer modPlayer, IEntitySource source)
        {
            bool canAffect = false;
            bool hasCD = true;
            if (TungstenAlwaysAffectProj(projectile))
            {
                canAffect = true;
                hasCD = false;
            }
            else if (FargoSoulsUtil.OnSpawnEnchCanAffectProjectile(projectile, source))
            {
                if (FargoSoulsUtil.IsProjSourceItemUseReal(projectile, source))
                {
                    if (modPlayer.TungstenCD == 0)
                        canAffect = true;
                }
                else if (source is EntitySource_Parent parent && parent.Entity is Projectile sourceProj)
                {
                    if (sourceProj.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TungstenScale != 1)
                    {
                        canAffect = true;
                        hasCD = false;
                    }
                    else if (sourceProj.minion || sourceProj.sentry || ProjectileID.Sets.IsAWhip[sourceProj.type])
                    {
                        if (modPlayer.TungstenCD == 0)
                            canAffect = true;
                    }
                }
            }

            if (canAffect)
            {
                float scale = modPlayer.TerraForce ? 3f : 2f;

                projectile.position = projectile.Center;
                projectile.scale *= scale;
                projectile.width = (int)(projectile.width * scale);
                projectile.height = (int)(projectile.height * scale);
                projectile.Center = projectile.position;
                FargoSoulsGlobalProjectile globalProjectile = projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>();
                globalProjectile.TungstenScale = scale;
                if (hasCD)
                    modPlayer.TungstenCD = 30;

                if (projectile.aiStyle == ProjAIStyleID.Spear || projectile.aiStyle == ProjAIStyleID.ShortSword)
                    projectile.velocity *= scale;

                //    if (modPlayer.Eternity)
                //    {
                //        modPlayer.TungstenCD = 0;
                //    }
                //    else if (modPlayer.TerraForce || modPlayer.WizardEnchant)
                //    {
                //        modPlayer.TungstenCD /= 2;
                //    }
            }
        }

        public static void TungstenModifyDamage(Player player, ref int damage, ref bool crit, DamageClass damageClass)
        {
            bool forceBuff = player.GetModPlayer<FargoSoulsPlayer>().TerraForce;

            damage = (int)(damage * (forceBuff ? 1.2 : 1.1));

            int max = forceBuff ? 2 : 1;
            for (int i = 0; i < max; i++)
            {
                if (crit)
                    break;

                crit = Main.rand.Next(0, 100) <= player.ActualClassCrit(damageClass);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TungstenHelmet)
                .AddIngredient(ItemID.TungstenChainmail)
                .AddIngredient(ItemID.TungstenGreaves)
                .AddIngredient(ItemID.TungstenBroadsword)
                .AddIngredient(ItemID.Ruler)
                .AddIngredient(ItemID.CandyCaneSword)

                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
