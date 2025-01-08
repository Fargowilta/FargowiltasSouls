using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Content.Projectiles.BossWeapons;
using FargowiltasSouls.Content.Projectiles.ChallengerItems;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class TungstenEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(176, 210, 178);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 40000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<TungstenEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TungstenHelmet)
                .AddIngredient(ItemID.TungstenChainmail)
                .AddIngredient(ItemID.TungstenGreaves)
                .AddIngredient(ItemID.TungstenBroadsword)
                .AddIngredient(ItemID.Ruler)
                .AddIngredient(ItemID.Katana)

                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }

    public class TungstenEffect : AccessoryEffect
    {

        public override Header ToggleHeader => Header.GetHeader<TerraHeader>();
        public override int ToggleItemType => ModContent.ItemType<TungstenEnchant>();
        public override void ModifyHitNPCWithItem(Player player, Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            if ((player.FargoSouls().ForceEffect<TungstenEnchant>() || item.shoot == ProjectileID.None))
            {
                TungstenModifyDamage(player, ref modifiers);
            }
        }
        public override void ModifyHitNPCWithProj(Player player, Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (proj.FargoSouls().TungstenScale != 1)
            {
                TungstenModifyDamage(player, ref modifiers);
            }
        }
        public override void PostUpdateMiscEffects(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.TungstenCD > 0)
                modPlayer.TungstenCD--;
        }
        public static float TungstenIncreaseWeaponSize(FargoSoulsPlayer modPlayer)
        {
            return 1f + (modPlayer.ForceEffect<TungstenEnchant>() && modPlayer.Player.HasEffectEnchant<TungstenEffect>() ? 2f : 1f);
        }

        public static List<int> TungstenAlwaysAffectProjType =
        [
                ProjectileID.MonkStaffT2,
                ProjectileID.Arkhalis,
                ProjectileID.Terragrim,
                ProjectileID.JoustingLance,
                ProjectileID.HallowJoustingLance,
                ProjectileID.ShadowJoustingLance,
                ModContent.ProjectileType<PrismaRegaliaProj>(),
                ModContent.ProjectileType<BaronTuskShrapnel>(),
                ModContent.ProjectileType<UmbraRegaliaProj>(),
                ModContent.ProjectileType<SlimeKingSlasherProj>(),
                ModContent.ProjectileType<SlimeSlingingSlasherProj>(),
                ProjectileID.TerraBlade2,
                ProjectileID.TerraBlade2Shot,
                ProjectileID.NightsEdge,
                ProjectileID.TrueNightsEdge
        ];
        public static List<int> TungstenAlwaysAffectProjStyle =
        [
            ProjAIStyleID.Spear,
            ProjAIStyleID.Yoyo,
            ProjAIStyleID.ShortSword,
            ProjAIStyleID.Flail,
            ProjAIStyleID.SleepyOctopod,
            ProjAIStyleID.NightsEdge,
            ProjAIStyleID.TrueNightsEdge
        ];
        public static List<int> TungstenNerfedProjType = 
        [
            ModContent.ProjectileType<SlimeKingSlasherProj>()
        ];
        public static bool TungstenAlwaysAffectProj(Projectile projectile)
        {
            return ProjectileID.Sets.IsAWhip[projectile.type] ||
                TungstenAlwaysAffectProjType.Contains(projectile.type) ||
                TungstenAlwaysAffectProjStyle.Contains(projectile.aiStyle);
        }
        public static List<int> TungstenNeverAffectProjType =
        [
            ModContent.ProjectileType<FishStickProjTornado>(),
            ModContent.ProjectileType<FishStickWhirlpool>(),
            ProjectileID.ButchersChainsaw,
        ];
        public static List<int> TungstenNeverAffectProjStyle = 
            [
            ProjAIStyleID.Yoyo
            ];
        public static bool TungstenNerfedProj(Projectile projectile) => TungstenNerfedProjType.Contains(projectile.type);
        public static bool TungstenNeverAffectsProj(Projectile projectile)
        {
            return TungstenNeverAffectProjType.Contains(projectile.type) ||
                TungstenNeverAffectProjStyle.Contains(projectile.aiStyle);
        }

        public static void TungstenIncreaseProjSize(Projectile projectile, FargoSoulsPlayer modPlayer, IEntitySource source)
        {
            bool terraForce = !modPlayer.Player.HasEffectEnchant<TungstenEffect>();
            if (terraForce)
                modPlayer.TungstenCD = 40;

            if (TungstenNeverAffectsProj(projectile))
            {
                return;
            }
            bool canAffect = false;
            bool hasCD = true;
            if (TungstenAlwaysAffectProj(projectile) || projectile.FargoSouls().IsAHeldProj)
            {
                canAffect = true;
                hasCD = false;
            }
            else if (FargoSoulsUtil.OnSpawnEnchCanAffectProjectile(projectile, false))
            {
                if (source != null && FargoSoulsUtil.IsProjSourceItemUseReal(projectile, source))
                {
                    if (modPlayer.TungstenCD == 0)
                        canAffect = true;
                }
                else if (source != null && source is EntitySource_Parent parent && parent.Entity is Projectile sourceProj)
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
            //Main.NewText(projectile.Name + " " + canAffect + " " + FargoSoulsUtil.IsProjSourceItemUseReal(projectile, source) + modPlayer.TungstenCD);
            if (canAffect)
            {
                bool forceEffect = modPlayer.ForceEffect<TungstenEnchant>();
                float scale = forceEffect ? 3f : 2f;
                if (terraForce && !modPlayer.EquippedEnchants.Any(e => e.Type == ModContent.ItemType<TungstenEnchant>()))
                    scale = 1.5f;
                else if (TungstenNerfedProj(projectile))
                    scale -= (scale - 1f) / 2f;
                projectile.position = projectile.Center;
                projectile.scale *= scale;
                projectile.width = (int)(projectile.width * scale);
                projectile.height = (int)(projectile.height * scale);
                projectile.Center = projectile.position;
                FargoSoulsGlobalProjectile globalProjectile = projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>();
                globalProjectile.TungstenScale = scale;

                if (projectile.aiStyle == ProjAIStyleID.Spear || projectile.aiStyle == ProjAIStyleID.ShortSword)
                    projectile.velocity *= scale;

                if (hasCD)
                {
                    modPlayer.TungstenCD = 40;

                    if (modPlayer.Eternity)
                        modPlayer.TungstenCD = 0;
                    else if (forceEffect)
                        modPlayer.TungstenCD /= 2;
                }
            }
        }

        public static void TungstenModifyDamage(Player player, ref NPC.HitModifiers modifiers)
        {
            if (!player.HasEffectEnchant<TungstenEffect>())
                return;

            FargoSoulsPlayer modPlayer = player.FargoSouls();
            bool forceBuff = modPlayer.ForceEffect<TungstenEnchant>();
            modifiers.FinalDamage *= forceBuff ? 1.14f : 1.07f;

            /* fuck you tungsten enchant
            int max = forceBuff ? 2 : 1;
            for (int i = 0; i < max; i++)
            {
                // TODO: performance I guess
                // if (crit)
                    // break;

                if (Main.rand.Next(0, 100) <= player.ActualClassCrit(damageClass))
                {
                    modifiers.SetCrit();
                }
            } */
        }
    }
}
