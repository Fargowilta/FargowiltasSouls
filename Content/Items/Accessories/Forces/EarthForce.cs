using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Forces
{
    public class EarthForce : BaseForce
    {
        public override void SetStaticDefaults()
        {
            Enchants[Type] =
            [
                ModContent.ItemType<CobaltEnchant>(),
                ModContent.ItemType<PalladiumEnchant>(),
                ModContent.ItemType<MythrilEnchant>(),
                ModContent.ItemType<OrichalcumEnchant>(),
                ModContent.ItemType<AdamantiteEnchant>(),
                ModContent.ItemType<TitaniumEnchant>()
            ];
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SetActive(player);
            //player.AddEffect<AncientCobaltEffect>(Item);
            //player.AddEffect<CobaltEffect>(Item);
            //player.AddEffect<PalladiumEffect>(Item);
            //player.AddEffect<PalladiumHealing>(Item);
            //player.AddEffect<MythrilEffect>(Item);
            //player.AddEffect<OrichalcumEffect>(Item);
            //player.AddEffect<AdamantiteEffect>(Item);
            //player.AddEffect<TitaniumEffect>(Item);

            player.AddEffect<EarthForceEffect>(Item);
            
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            foreach (int ench in Enchants[Type])
                recipe.AddIngredient(ench);
            recipe.AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"));
            recipe.Register();
        }
    }
    public class EarthForceEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<EarthHeader>();
        public override int ToggleItemType => ModContent.ItemType<EarthForce>();

        //modify this number and equation to balance out the timer stuff
        public static int EarthMaxCharge = 400;
        public static float GetEarthForceLerpValue(Player player)
        {
            //values will be at max when timer is between 300-400
            //values will be scaling between 100-300
            //values will not be changed from player's default when between 0 and 100
            //(unless you change it)
            return MathHelper.Clamp((player.FargoSouls().EarthTimer - 100) / 200f, 0, 1);
        }
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer farg = player.FargoSouls();
            if (!player.controlUseItem && farg.EarthTimer < EarthMaxCharge)
            {
                farg.EarthTimer += 2;
            }else if (player.controlUseItem && farg.EarthTimer > 0)
            {
                farg.EarthTimer--;
            }
            
            float lerper = GetEarthForceLerpValue(player);
            //player.GetDamage(DamageClass.Generic) *= MathHelper.Lerp(1, 0.3f, lerper);
            
            //mythril takes priority if equipped
            if (!player.HasEffect<MythrilEffect>())
            {
                farg.AttackSpeed *= MathHelper.Lerp(1, 2f, lerper);
            }
            player.lifeRegen += (int)MathHelper.Lerp(0, 8, lerper);
            player.endurance += (int)MathHelper.Lerp(0, 0.1f, lerper);
            //Main.NewText(player.GetAttackSpeed(DamageClass.Generic));

            //one below or two below because it increments by 2 so it could skip this if it was just one number
            if (farg.EarthTimer >= EarthMaxCharge -2 && farg.EarthTimer < EarthMaxCharge && !player.controlUseItem && Main.myPlayer == player.whoAmI)
            {
                float pitch = 0;
                if (player.HasEffect<MythrilEffect>()) pitch = -0.2f;
                SoundEngine.PlaySound(new SoundStyle($"{nameof(FargowiltasSouls)}/Assets/Sounds/ChargeSound") with { Pitch = pitch}, player.Center);
                for (int i = 0; i < 5; i++)
                {
                    Vector2 position = player.Center + new Vector2(0, Main.rand.NextFloat(20, 40)).RotatedByRandom(MathHelper.TwoPi);
                    Particle green = new SparkParticle(position, (player.Center - position).SafeNormalize(Vector2.Zero) * 0.5f, Color.LightSeaGreen, 0.5f, 20);
                    green.Spawn();
                    position = player.Center + new Vector2(0, Main.rand.NextFloat(20, 40)).RotatedByRandom(MathHelper.TwoPi);
                    Particle red = new SparkParticle(position, (player.Center - position).SafeNormalize(Vector2.Zero) * 0.5f, Color.Red, 0.5f, 20);
                    red.Spawn();
                    position = player.Center + new Vector2(0, Main.rand.NextFloat(20, 40)).RotatedByRandom(MathHelper.TwoPi);
                    Particle white = new SparkParticle(position, (player.Center - position).SafeNormalize(Vector2.Zero) * 0.5f, Color.White, 0.5f, 20);
                    white.Spawn();
                }
            }
        }
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            float lerper = GetEarthForceLerpValue(player);
            int debuffDamage = (int)(baseDamage * MathHelper.Lerp(1, 0.3f, lerper));
            //divide by 2.3 because want to deal that damage over the course of ~6.6 seconds, deal a bit more than the actual missing damage to compensate for constant re-application of debuff without increasing the duration
            // Change damage to average of old and new damage to make it less affected by random extreme variation in damage
            target.FargoSouls().EarthDoTValue = (int)MathHelper.Lerp(target.FargoSouls().EarthDoTValue, debuffDamage / 2.3f, 0.5f);
            target.AddBuff(ModContent.BuffType<EarthPoison>(), 400);
            //reduce iframes so that the accessory actually increases dps for real
            if (projectile != null && !projectile.usesIDStaticNPCImmunity && !projectile.usesLocalNPCImmunity && projectile.penetrate != 1 && projectile.FargoSouls().AdamModifier == 3)
            {
                target.immune[player.whoAmI] = 3;
            }
        }
        
        public static void EarthSplit(Projectile projectile, Player player)
        {
            float lerper = GetEarthForceLerpValue(player);
            FargoSoulsPlayer farg = player.FargoSouls();
            if (!FargoSoulsUtil.OnSpawnEnchCanAffectProjectile(projectile, false) || AdamantiteEffect.AdamIgnoreItems.Contains(player.HeldItem.type))
            {
                return;
            }
            float angleDif = MathHelper.Lerp(2, 30, lerper);
            foreach (Projectile p in FargoSoulsGlobalProjectile.SplitProj(projectile, 3, MathHelper.ToRadians(angleDif), 1))
            {
                if (p.Alive())
                {
                    p.FargoSouls().HuntressProj = projectile.FargoSouls().HuntressProj;
                }
            }
            projectile.damage = (int)(projectile.damage * 0.4f);

        }
    }
}
