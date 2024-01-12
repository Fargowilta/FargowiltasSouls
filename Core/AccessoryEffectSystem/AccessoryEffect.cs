using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace FargowiltasSouls.Core.AccessoryEffectSystem
{
    //Proof of concept file for an accessory effect rework

    /// <summary>
    /// Contains the behavior for an accessory effect. <para/>
    /// Each accessory effect needs a localized toggle description as Mods.YourMod.Toggler.YourAccessoryEffectName.<para/>
    /// This type is not instanced per player. Put instanced things (such as fields) in an EffectFields.
    /// </summary>
    public abstract class AccessoryEffect : ModType
    {
        /// <summary>
        /// Whether the effect has a toggle. <para\>
        /// ToggleHeader is unused if this is false.
        /// </summary>
        public abstract bool HasToggle { get; }
        public string ToggleDescription => Language.GetTextValue($"Mods.{Mod}.Toggler.{Name}");
        public abstract Header ToggleHeader { get; }

        public virtual bool MinionEffect => false;
        public virtual bool ExtraAttackEffect => false;
        public virtual bool IgnoresMutantPresence => false;

        protected sealed override void Register()
        {
            AccessoryEffectLoader.Register(this);
            ModTypeLookup<AccessoryEffect>.Register(this);
        }
        /// <summary>
        /// The item associated with this effect. Null if none is found.
        /// </summary>
        public Item EffectItem(Player player) => player.AccessoryEffects().EffectItems.TryGetValue(this, out Item value) ? value : null;

        #region Overridables
        public virtual void PreUpdate(Player player) { }
        public virtual void PostUpdateEquips(Player player) { }
        public virtual void UpdateBadLifeRegen(Player player) { }
        public virtual void PostUpdate(Player player) { }
        public virtual void PostUpdateMiscEffects(Player player) { }
        public virtual void TryAdditionalAttacks(Player player, int damage, DamageClass damageType) { }
        public virtual void ModifyHitNPCWithProj(Player player, Projectile proj, NPC target, ref NPC.HitModifiers modifiers) { }
        public virtual void ModifyHitNPCWithItem(Player player, Item item, NPC target, ref NPC.HitModifiers modifiers) { }
        public virtual void ModifyHitNPCBoth(Player player, NPC target, ref NPC.HitModifiers modifiers, DamageClass damageClass) { }
        public virtual void ModifyHitInfo(Player player, NPC target, ref NPC.HitInfo hitInfo, DamageClass damageClass) { }
        public virtual void OnHitNPCWithProj(Player player, Projectile proj, NPC target, NPC.HitInfo hit, int damageDone) { }
        public virtual void OnHitNPCWithItem(Player player, Item item, NPC target, NPC.HitInfo hit, int damageDone) { }
        public virtual void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item) { }
        public virtual void MeleeEffects(Player player, Item item, Rectangle hitbox) { }
        public virtual float ModifyUseSpeed(Player player, Item item) { return 0f; }
        public virtual float ContactDamageDR(Player player, NPC npc, ref Player.HurtModifiers modifiers) { return 0f; }
        public virtual float ProjectileDamageDR(Player player, Projectile projectile, ref Player.HurtModifiers modifiers) { return 0f; }
        public virtual void ModifyHitByNPC(Player player, NPC npc, ref Player.HurtModifiers modifiers) { }
        public virtual void ModifyHitByProjectile(Player player, Projectile projectile, ref Player.HurtModifiers modifiers) { }
        public virtual void OnHitByNPC(Player player, NPC npc, Player.HurtInfo hurtInfo) { }
        public virtual void OnHitByProjectile(Player player, Projectile proj, Player.HurtInfo hurtInfo) { }
        public virtual void OnHitByEither(Player player, NPC npc, Projectile proj) { }
        public virtual bool CanBeHitByNPC(Player player, NPC npc) { return true; }
        public virtual bool CanBeHitByProjectile(Player player, Projectile projectile) { return true; }
        public virtual void ModifyHurt(Player player, ref Player.HurtModifiers modifiers) { }
        public virtual void OnHurt(Player player, Player.HurtInfo info) { }
        public virtual bool PreKill(Player player, double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource) { return true; }
        public virtual void DrawEffects(Player player, PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright) { }
        #endregion


    }
    /// <summary>
    /// Contains the parts of an accessory effect that should be instanced - for example fields.
    /// </summary>
    public abstract class EffectFields : ModType<ModPlayer, EffectFields>, IIndexed
    {
        public ModPlayer ModPlayer => Entity;
        public Player Player => Entity.Player;
        public ushort Index { get; internal set; }
        protected override ModPlayer CreateTemplateEntity() => null;
        public override EffectFields NewInstance(ModPlayer entity)
        {
            var inst = base.NewInstance(entity);

            inst.Index = Index;
            return inst;
        }

        protected sealed override void Register()
        {
            AccessoryEffectLoader.Register(this);
            ModTypeLookup<EffectFields>.Register(this);
        }
        #region Overridables
        /// <summary>
        /// Runs in ModPlayer ResetEffects (once per frame). See ModPlayer.ResetEffects for more info.
        /// </summary>
        public virtual void ResetEffects() { }
        public virtual void UpdateDead() { }
        #endregion
    }
}