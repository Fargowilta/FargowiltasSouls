using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Content.Projectiles.BossWeapons;
using FargowiltasSouls.Content.Projectiles.ChallengerItems;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System;
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
            player.AddEffect<TungstenShockwaveEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TungstenHelmet)
                .AddIngredient(ItemID.TungstenChainmail)
                .AddIngredient(ItemID.TungstenGreaves)
                .AddIngredient(ItemID.TungstenBroadsword)
                .AddIngredient(ItemID.Ruler)
                if (Main.remixWorld && !Main.getGoodWorld) //gfb should remain as is
                {
                    .AddIngredient(ItemID.Keybrand)
                }
                else
                {
                .AddIngredient(ItemID.Katana)
                }

                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
    public class TungstenShockwaveEffect : AccessoryEffect
    {

        public override Header ToggleHeader => Header.GetHeader<TerraHeader>();
        public override int ToggleItemType => ModContent.ItemType<TungstenEnchant>();
        public override bool ExtraAttackEffect => true;
        public override bool MutantsPresenceAffects => true;
        public override void PostUpdateMiscEffects(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.TungstenCD > 0)
                modPlayer.TungstenCD--;
        }
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            if (!HasEffectEnchant(player))
                return;
            bool weaponAttack = false;
            if (item != null)
                weaponAttack = true;
            if (projectile != null && projectile.FargoSouls().ItemSource)
                weaponAttack = true;
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.TungstenCD == 0 && weaponAttack)
            {
                int damage = baseDamage;
                if (damage < 30)
                    damage = 30;
                int falloffMin = 200;
                if (damage > falloffMin)
                {
                    damage -= falloffMin;
                    damage = (int)(damage * MathF.Exp(-damage / falloffMin));
                    damage += falloffMin;
                }
                float ai1 = player.ForceEffect<TungstenShockwaveEffect>() ? 1 : 0;
                Projectile.NewProjectile(GetSource_EffectItem(player), target.Center, player.DirectionTo(target.Center), ModContent.ProjectileType<TungstenShockwave>(), damage, 3f, player.whoAmI, target.whoAmI, ai1);
                modPlayer.TungstenCD = LumUtils.SecondsToFrames(2.5f);
                if (ai1 == 1)
                    modPlayer.TungstenCD /= 3;
            }
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
            player.whipRangeMultiplier += 0.2f;
        }
        public const float SizeMult = 1.5f;
        public static float TungstenIncreaseWeaponSize(FargoSoulsPlayer modPlayer)
        {
            return SizeMult;
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
            ModContent.ProjectileType<SlimeKingSlasherProj>(),
            ModContent.ProjectileType<SlimeSlingingSlasherProj>()
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
            ModContent.ProjectileType<ReleasedMechFlail>(),
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

        public static float TungstenIncreaseProjSize(Projectile projectile, FargoSoulsPlayer modPlayer, IEntitySource source)
        {
            bool terraForce = !modPlayer.Player.HasEffectEnchant<TungstenEffect>();
            //if (terraForce)
            //modPlayer.TungstenCD = 40; // effectively just removes the CD effect

            if (TungstenNeverAffectsProj(projectile))
            {
                return 0f;
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
                if (source != null && source is EntitySource_Parent parent && parent.Entity is Projectile sourceProj)
                {
                    if (sourceProj.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TungstenScale != 1)
                    {
                        canAffect = true;
                        hasCD = false;
                    }
                }
            }
            //Main.NewText(projectile.Name + " " + canAffect + " " + FargoSoulsUtil.IsProjSourceItemUseReal(projectile, source) + modPlayer.TungstenCD);
            if (canAffect)
            {
                //bool forceEffect = modPlayer.ForceEffect<TungstenEnchant>();
                float scaleIncrease = SizeMult - 1;
                if (TungstenNerfedProj(projectile))
                    scaleIncrease /= 2;
                return scaleIncrease;
            }
            return 0f;
        }

        public static void TungstenModifyDamage(Player player, ref NPC.HitModifiers modifiers)
        {
            return;
        }
    }
}
