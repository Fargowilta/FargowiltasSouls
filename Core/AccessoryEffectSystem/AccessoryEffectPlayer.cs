using FargowiltasSouls.Content.Bosses.AbomBoss;
using FargowiltasSouls.Content.Bosses.DeviBoss;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Expert;
using FargowiltasSouls.Content.Items.Accessories.Masomode;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Items.Armor;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Content.Projectiles.Minions;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Core.Toggler;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.Collision;
using FargowiltasSouls.Content.Items.Consumables;
using FargowiltasSouls.Content.Items.Weapons.Challengers;
using FargowiltasSouls.Content.Items.Weapons.SwarmDrops;
using System.Data;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.UI;
using Terraria.WorldBuilding;
using Microsoft.CodeAnalysis;
using Terraria.ModLoader.Core;
using System.Linq.Expressions;
using System.Reflection;

namespace FargowiltasSouls.Core.AccessoryEffectSystem
{
    public class AccessoryEffectPlayer : ModPlayer
    {
        public HashSet<AccessoryEffect> ActiveEffects = new();
        
        public Dictionary<AccessoryEffect, Item> EffectItems = new();
        internal EffectFields[] EffectFieldsInstances;
        private static readonly Dictionary<MethodInfo, List<AccessoryEffect>> Hooks = new();

        public bool Active(AccessoryEffect effect) => ActiveEffects.Contains(effect);
        public override void Initialize()
        {
            int instanceCount = AccessoryEffectLoader.EffectFields.Count;
            EffectFieldsInstances = new EffectFields[instanceCount];
            for (int i = 0; i < instanceCount; i++)
            {
                EffectFieldsInstances[i] = AccessoryEffectLoader.EffectFields[i];
            }
            Expression<Func<EffectFields, Action>> resetEffects = p => p.ResetEffects;
            HookResetEffects.AddRange(EffectFieldsInstances.WhereMethodIsOverridden(resetEffects.ToMethodInfo()));
            Expression<Func<EffectFields, Action>> updateDead = p => p.UpdateDead;
            HookResetEffects.AddRange(EffectFieldsInstances.WhereMethodIsOverridden(updateDead.ToMethodInfo()));
        }
        public override void SetStaticDefaults()
        {
            foreach (var hook in Hooks)
            {
                hook.Value.AddRange(AccessoryEffectLoader.AccessoryEffects.WhereMethodIsOverridden(hook.Key));
            }
        }
        #region Overrides

        private static List<AccessoryEffect> AddHook<F>(Expression<Func<AccessoryEffect, F>> expr) where F : Delegate
        {
            var effectSet = new List<AccessoryEffect>();
            Hooks.Add(expr.ToMethodInfo(), effectSet);
            return effectSet;
        }
        public List<EffectFields> HookResetEffects = new();
        public override void ResetEffects()
        {
            EffectItems.Clear();
            ActiveEffects.Clear();
            foreach (EffectFields effectFields in HookResetEffects)
            {
                effectFields.ResetEffects();
            }
        }
        public List<EffectFields> HookUpdateDead = new();
        public override void UpdateDead()
        {
            ResetEffects();
            foreach (EffectFields effectFields in HookUpdateDead)
            {
                effectFields.UpdateDead();
            }
        }
        private static List<AccessoryEffect> HookPreUpdate = AddHook<Action<Player>>(p => p.PreUpdate);
        public override void PreUpdate()
        {
            foreach (AccessoryEffect effect in HookPreUpdate)
            {
                if (Active(effect))
                    effect.PreUpdate(Player);
            }
        }
        private static List<AccessoryEffect> HookPostUpdateEquips = AddHook<Action<Player>>(p => p.PostUpdateEquips);
        public override void PostUpdateEquips()
        {
            foreach (AccessoryEffect effect in HookPostUpdateEquips)
            {
                if (Active(effect))
                    effect.PostUpdateEquips(Player);
            }
        }
        private static List<AccessoryEffect> HookUpdateBadLifeRegen = AddHook<Action<Player>>(p => p.UpdateBadLifeRegen);
        public override void UpdateBadLifeRegen()
        {
            foreach (AccessoryEffect effect in HookUpdateBadLifeRegen)
            {
                if (Active(effect))
                    effect.UpdateBadLifeRegen(Player);
            }
        }
        private static List<AccessoryEffect> HookPostUpdate = AddHook<Action<Player>>(p => p.PostUpdate);
        public override void PostUpdate()
        {
            foreach (AccessoryEffect effect in HookPostUpdate)
            {
                if (Active(effect))
                    effect.PostUpdate(Player);
            }
        }
        private static List<AccessoryEffect> HookPostUpdateMiscEffects = AddHook<Action<Player>>(p => p.PostUpdateMiscEffects);
        public override void PostUpdateMiscEffects()
        {
            foreach (AccessoryEffect effect in HookPostUpdateMiscEffects)
            {
                if (Active(effect))
                    effect.PostUpdateMiscEffects(Player);
            }
        }
        private static List<AccessoryEffect> HookTryAdditionalAttacks = AddHook<Action<Player, int, DamageClass>>(p => p.TryAdditionalAttacks);
        public void TryAdditionalAttacks(int damage, DamageClass damageType)
        {
            foreach (AccessoryEffect effect in HookTryAdditionalAttacks)
            {
                if (Active(effect))
                    effect.TryAdditionalAttacks(Player, damage, damageType);
            }
        }
        private delegate void DelegateModifyHitNPCWithProj(Player player, Projectile proj, NPC target, ref NPC.HitModifiers modifiers);
        private static List<AccessoryEffect> HookModifyHitNPCWithProj = AddHook<DelegateModifyHitNPCWithProj>(p => p.ModifyHitNPCWithProj);
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (proj.hostile)
                return;

            foreach (AccessoryEffect effect in HookModifyHitNPCWithProj)
            {
                if (Active(effect))
                    effect.ModifyHitNPCWithProj(Player, proj, target, ref modifiers);
            }

            ModifyHitNPCBoth(target, ref modifiers, proj.DamageType);
        }
        private delegate void DelegateModifyHitNPCWithItem(Player player, Item item, NPC target, ref NPC.HitModifiers modifiers);
        private static List<AccessoryEffect> HookModifyHitNPCWithItem = AddHook<DelegateModifyHitNPCWithItem>(p => p.ModifyHitNPCWithItem);
        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            foreach (AccessoryEffect effect in ActiveEffects)
            {
                if (Active(effect))
                    effect.ModifyHitNPCWithItem(Player, item, target, ref modifiers);
            }

            ModifyHitNPCBoth(target, ref modifiers, item.DamageType);
        }
        private delegate void DelegateHookModifyHitNPCBoth(Player player, NPC target, ref NPC.HitModifiers modifiers, DamageClass damageClass);
        private static List<AccessoryEffect> HookModifyHitNPCBoth = AddHook<DelegateModifyHitNPCWithItem>(p => p.ModifyHitNPCWithItem);
        public void ModifyHitNPCBoth(NPC target, ref NPC.HitModifiers modifiers, DamageClass damageClass)
        {
            modifiers.ModifyHitInfo += (ref NPC.HitInfo hitInfo) =>
            {
                ModifyHitInfo(target, ref hitInfo, damageClass);
            };
            foreach (AccessoryEffect effect in HookModifyHitNPCBoth)
            {
                if (Active(effect))
                    effect.ModifyHitNPCBoth(Player, target, ref modifiers, damageClass);
            }
        }
        private delegate void DelegateHookModifyHitInfo(Player player, NPC target, ref NPC.HitInfo hitInfo, DamageClass damageClass);
        private static List<AccessoryEffect> HookModifyHitInfo = AddHook<DelegateHookModifyHitInfo>(p => p.ModifyHitInfo);
        public void ModifyHitInfo(NPC target, ref NPC.HitInfo hitInfo, DamageClass damageClass)
        {
            foreach (AccessoryEffect effect in HookModifyHitInfo)
            {
                if (Active(effect))
                    effect.ModifyHitInfo(Player, target, ref hitInfo, damageClass);
            }
        }
        private static List<AccessoryEffect> HookOnHitNPCWithProj = AddHook<Action<Player, Projectile, NPC, NPC.HitInfo, int>>(p => p.OnHitNPCWithProj);
        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.type == NPCID.TargetDummy || target.friendly)
                return;

            foreach (AccessoryEffect effect in HookOnHitNPCWithProj)
            {
                if (Active(effect))
                    effect.OnHitNPCWithProj(Player, proj, target, hit, damageDone);
            }

            if (proj.minion)// && proj.type != ModContent.ProjectileType<CelestialRuneAncientVision>() && proj.type != ModContent.ProjectileType<SpookyScythe>())
                TryAdditionalAttacks(proj.damage, proj.DamageType);

            OnHitNPCEither(target, hit, proj.DamageType, projectile: proj);

        }
        private static List<AccessoryEffect> HookOnHitNPCWithItem = AddHook<Action<Player, Item, NPC, NPC.HitInfo, int>>(p => p.OnHitNPCWithItem);
        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.type == NPCID.TargetDummy || target.friendly)
                return;

            foreach (AccessoryEffect effect in HookOnHitNPCWithItem)
            {
                if (Active(effect))
                    effect.OnHitNPCWithItem(Player, item, target, hit, damageDone);
            }

            OnHitNPCEither(target, hit, item.DamageType, item: item);
        }
        private static List<AccessoryEffect> HookOnHitNPCEither = AddHook<Action<Player, NPC, NPC.HitInfo, DamageClass, int, Projectile, Item>>(p => p.OnHitNPCEither);
        private void OnHitNPCEither(NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, Projectile projectile = null, Item item = null)
        {

            //doing this so that damage-inheriting effects dont double dip or explode due to taking on crit boost
            int GetBaseDamage()
            {
                // TODO: I guess? test this
                int baseDamage = hitInfo.SourceDamage;
                if (projectile != null)
                    baseDamage = projectile.damage;
                else if (item != null)
                    baseDamage = Player.GetWeaponDamage(item);
                return baseDamage;
            }
            int baseDamage = GetBaseDamage();
            foreach (AccessoryEffect effect in HookOnHitNPCEither)
            {
                if (Active(effect))
                    effect.OnHitNPCEither(Player, target, hitInfo, damageClass, baseDamage, projectile, item);
            }
        }
        private static List<AccessoryEffect> HookMeleeEffects = AddHook<Action<Player, Item, Rectangle>>(p => p.MeleeEffects);
        public override void MeleeEffects(Item item, Rectangle hitbox)
        {
            foreach (AccessoryEffect effect in HookMeleeEffects)
            {
                if (Active(effect))
                    effect.MeleeEffects(Player, item, hitbox);
            }
        }
        private static List<AccessoryEffect> HookModifyUseSpeed = AddHook<Func<Player, Item, float>>(p => p.ModifyUseSpeed);
        public float ModifyUseSpeed(Item item)
        {
            float speedModifier = 0;
            foreach (AccessoryEffect effect in HookModifyUseSpeed)
            {
                if (Active(effect))
                    speedModifier += effect.ModifyUseSpeed(Player, item);
            }
            return speedModifier;
        }
        private delegate float DelegateContactDamageDR(Player player, NPC npc, ref Player.HurtModifiers modifiers);
        private static List<AccessoryEffect> HookContactDamageDR = AddHook<DelegateContactDamageDR>(p => p.ContactDamageDR);
        public float ContactDamageDR(NPC npc, ref Player.HurtModifiers modifiers)
        {
            float dr = 0;
            foreach (AccessoryEffect effect in HookContactDamageDR)
            {
                if (Active(effect))
                    dr += effect.ContactDamageDR(Player, npc, ref modifiers);
            }
            return dr;
        }
        private delegate float DelegateProjectileDamageDR(Player player, Projectile projectile, ref Player.HurtModifiers modifiers);
        private static List<AccessoryEffect> HookProjectileDamageDR = AddHook<DelegateProjectileDamageDR>(p => p.ProjectileDamageDR);
        public float ProjectileDamageDR(Projectile projectile, ref Player.HurtModifiers modifiers)
        {
            float dr = 0;
            foreach (AccessoryEffect effect in HookProjectileDamageDR)
            {
                if (Active(effect))
                    dr += effect.ProjectileDamageDR(Player, projectile, ref modifiers);
            }
            return dr;
        }
        private delegate void DelegateModifyHitByNPC(Player player, NPC npc, ref Player.HurtModifiers modifiers);
        private static List<AccessoryEffect> HookModifyHitByNPC = AddHook<DelegateModifyHitByNPC>(p => p.ModifyHitByNPC);
        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            foreach (AccessoryEffect effect in HookModifyHitByNPC)
            {
                if (Active(effect))
                    effect.ModifyHitByNPC(Player, npc, ref modifiers);
            }
        }
        private delegate void DelegateModifyHitByProjectile(Player player, Projectile proj, ref Player.HurtModifiers modifiers);
        private static List<AccessoryEffect> HookModifyHitByProjectile = AddHook<DelegateModifyHitByProjectile>(p => p.ModifyHitByProjectile);
        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            foreach (AccessoryEffect effect in HookModifyHitByProjectile)
            {
                if (Active(effect))
                    effect.ModifyHitByProjectile(Player, proj, ref modifiers);
            }
        }
        private static List<AccessoryEffect> HookOnHitByNPC = AddHook<Action<Player, NPC, Player.HurtInfo>>(p => p.OnHitByNPC);
        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            foreach (AccessoryEffect effect in HookOnHitByNPC)
            {
                if (Active(effect))
                    effect.OnHitByNPC(Player, npc, hurtInfo);
            }
            OnHitByEither(npc, null);
        }
        private static List<AccessoryEffect> HookOnHitByProjectile = AddHook<Action<Player, Projectile, Player.HurtInfo>>(p => p.OnHitByProjectile);
        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            foreach (AccessoryEffect effect in HookOnHitByProjectile)
            {
                if (Active(effect))
                    effect.OnHitByProjectile(Player, proj, hurtInfo);
            }
            OnHitByEither(null, proj);
        }
        private static List<AccessoryEffect> HookOnHitByEither = AddHook<Action<Player, NPC, Projectile>>(p => p.OnHitByEither);
        public void OnHitByEither(NPC npc, Projectile proj)
        {
            foreach (AccessoryEffect effect in HookOnHitByEither)
            {
                if (Active(effect))
                    effect.OnHitByEither(Player, npc, proj);
            }
        }
        private static List<AccessoryEffect> HookCanBeHitByNPC = AddHook<Func<Player, NPC, bool>>(p => p.CanBeHitByNPC);
        public override bool CanBeHitByNPC(NPC npc, ref int CooldownSlot)
        {
            foreach (AccessoryEffect effect in HookCanBeHitByNPC)
            {
                if (Active(effect))
                    if (!effect.CanBeHitByNPC(Player, npc))
                        return false;
            }
            return true;
        }
        private static List<AccessoryEffect> HookCanBeHitByProjectile = AddHook<Func<Player, Projectile, bool>>(p => p.CanBeHitByProjectile);
        public override bool CanBeHitByProjectile(Projectile proj)
        {
            foreach (AccessoryEffect effect in HookCanBeHitByProjectile)
            {
                if (Active(effect))
                    if (!effect.CanBeHitByProjectile(Player, proj))
                        return false;
            }
            return true;
        }
        private delegate void DelegateModifyHurt(Player player, ref Player.HurtModifiers modifiers);
        private static List<AccessoryEffect> HookModifyHurt = AddHook<DelegateModifyHurt>(p => p.ModifyHurt);
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)/* tModPorter Override ImmuneTo, FreeDodge or ConsumableDodge instead to prevent taking damage */
        {
            foreach (AccessoryEffect effect in HookModifyHurt)
            {
                if (Active(effect))
                    effect.ModifyHurt(Player, ref modifiers);
            }
        }
        private static List<AccessoryEffect> HookOnHurt = AddHook<Action<Player, Player.HurtInfo>>(p => p.OnHurt);
        public override void OnHurt(Player.HurtInfo info)
        {
            foreach (AccessoryEffect effect in HookOnHurt)
            {
                if (Active(effect))
                    effect.OnHurt(Player, info);
            }
        }
        private delegate bool DelegatePreKill(Player player, double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource);
        private static List<AccessoryEffect> HookPreKill = AddHook<DelegatePreKill>(p => p.PreKill);
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            bool result = true;
            foreach (AccessoryEffect effect in HookPreKill)
            {
                if (Active(effect))
                    if (!effect.PreKill(Player, damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource))
                        result = false;
            }
            return result;
        }
        private delegate void DelegateDrawEffects(Player player, PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright);
        private static List<AccessoryEffect> HookDrawEffects = AddHook<DelegateDrawEffects>(p => p.DrawEffects);
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            foreach (AccessoryEffect effect in HookDrawEffects)
            {
                if (Active(effect))
                    effect.DrawEffects(Player, drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
            }
        }
        #endregion
    }
}
