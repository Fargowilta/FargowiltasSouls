using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Globals;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls //lets everything access it without using
{
	public static partial class FargoExtensionMethods
    {
        /// <summary>
        /// Adjusts a TooltipLine to account for prefixes. <br />
        /// Inteded to be used specifically for item names. <br />
        /// This only modifies it in the inventory.
        /// </summary>
        public static TooltipLine ArticlePrefixAdjustment(this TooltipLine itemName, string[] localizationArticles)
        {
            List<string> splitName = itemName.Text.Split(' ').ToList();

            for (int i = 0; i < localizationArticles.Length; i++)
                if (splitName.Remove(localizationArticles[i]))
                {
                    splitName.Insert(0, localizationArticles[i]);
                    break;
                }

            itemName.Text = string.Join(" ", splitName);
            return itemName;
        }

        /// <summary>
        /// Uses <see cref="Enumerable.First{TSource}(IEnumerable{TSource}, System.Func{TSource, bool})"/> to find the specified tooltip line. <br />
        /// Returns true if the tooltipLine isn't null and false if it is. <br />
        /// Assumes Terraria as the mod.
        /// </summary>
        public static bool TryFindTooltipLine(this List<TooltipLine> tooltips, string tooltipName, out TooltipLine tooltipLine)
        {
            tooltips.TryFindTooltipLine(tooltipName, "Terraria", out tooltipLine);

            return tooltipLine != null;
        }

        /// <summary>
        /// Uses <see cref="Enumerable.First{TSource}(IEnumerable{TSource}, System.Func{TSource, bool})"/> to find the specified tooltip line. <br />
        /// Returns true if the tooltipLine isn't null and false if it is.
        /// </summary>
        public static bool TryFindTooltipLine(this List<TooltipLine> tooltips, string tooltipName, string tooltipMod, out TooltipLine tooltipLine)
        {
            tooltipLine = tooltips.First(line => line.Name == tooltipName && line.Mod == tooltipMod);

            return tooltipLine != null;
        }

        private static readonly FieldInfo _damageFieldHitInfo =
            typeof(NPC.HitInfo).GetField("_damage", BindingFlags.Instance | BindingFlags.NonPublic);

        public static void Null(ref this NPC.HitInfo hitInfo)
        {
            // hitInfo.Damage = 0;
            
            // https://stackoverflow.com/questions/27226731/getfield-setvalue-doesnt-work
            object unboxedHitInfo = hitInfo;
            _damageFieldHitInfo.SetValue(unboxedHitInfo, 0);
            hitInfo = (NPC.HitInfo)unboxedHitInfo;
            hitInfo.Knockback = 0;
            hitInfo.Crit = false;
            // TODO: should we?
            // hitInfo.HideCombatText = true;
            hitInfo.InstantKill = false;
        }

        public static void Null(ref this NPC.HitModifiers hitModifiers)
        {
            // doesn't work because tModLoader maxes it with 1
            // statModifier = statModifier.Scale(0f);
            
            // will break if basically any mod registers this hook as well
            hitModifiers.ModifyHitInfo += (ref NPC.HitInfo hitInfo) => hitInfo.Null();
        }

        private static readonly FieldInfo _damageFieldHurtInfo =
            typeof(Player.HurtInfo).GetField("_damage", BindingFlags.Instance | BindingFlags.NonPublic);

        public static void Null(ref this Player.HurtInfo hurtInfo)
        {
            // hurtInfo.Damage = 0;
            object unboxedHurtInfo = hurtInfo;
            _damageFieldHurtInfo.SetValue(unboxedHurtInfo, 0);
            hurtInfo = (Player.HurtInfo)unboxedHurtInfo;
            hurtInfo.Knockback = 0;
        }

        public static void Null(ref this Player.HurtModifiers hurtModifiers)
        {
            // will break if basically any mod registers this hook as well
            hurtModifiers.ModifyHurtInfo += (ref Player.HurtInfo hurtInfo) => hurtInfo.Null();
        }

        public static void AddDebuffImmunities(this NPC npc, List<int> debuffs)
        {
            foreach (int buffType in debuffs)
            {
                NPCID.Sets.SpecificDebuffImmunity[npc.type][buffType] = true;
            }
        }

        public static FargoSoulsGlobalNPC FargoSouls(this NPC npc)
            => npc.GetGlobalNPC<FargoSoulsGlobalNPC>();
        public static EModeGlobalNPC Eternity(this NPC npc)
            => npc.GetGlobalNPC<EModeGlobalNPC>();
        public static FargoSoulsGlobalProjectile FargoSouls(this Projectile projectile)
            => projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>();
        public static EModeGlobalProjectile Eternity(this Projectile projectile)
            => projectile.GetGlobalProjectile<EModeGlobalProjectile>();
        public static FargoSoulsPlayer FargoSouls(this Player player)
            => player.GetModPlayer<FargoSoulsPlayer>();
        public static EModePlayer Eternity(this Player player)
            => player.GetModPlayer<EModePlayer>();
        public static AccessoryEffectPlayer AccessoryEffects(this Player player) 
            => player.GetModPlayer<AccessoryEffectPlayer>();
        public static bool ForceEffect<T>(this Player player) where T : AccessoryEffect
            => player.FargoSouls().ForceEffect(player.EffectItem<T>().ModItem);

        public static T As<T>(this NPC npc) where T : ModNPC => npc.ModNPC as T;
        public static T As<T>(this Projectile projectile) where T : ModProjectile => projectile.ModProjectile as T;
        public static bool Alive(this Player player) => player != null && player.active && !player.dead && !player.ghost;
        public static bool Alive(this Projectile projectile) => projectile != null && projectile.active;
        public static bool Alive(this NPC npc) => npc != null && npc.active;
        public static bool TypeAlive(this Projectile projectile, int type) => projectile.Alive() && projectile.type == type;
        public static bool TypeAlive(this NPC npc, int type) => npc.Alive() && npc.type == type;
        public static NPC GetSourceNPC(this Projectile projectile)
            => projectile.GetGlobalProjectile<A_SourceNPCGlobalProjectile>().sourceNPC;

        public static void SetSourceNPC(this Projectile projectile, NPC npc)
            => projectile.GetGlobalProjectile<A_SourceNPCGlobalProjectile>().sourceNPC = npc;

        public static float ActualClassDamage(this Player player, DamageClass damageClass)
            => player.GetTotalDamage(damageClass).Additive * player.GetTotalDamage(damageClass).Multiplicative;

        public static bool IsWithinBounds(this int index, int cap)
        {
            if (index >= 0)
            {
                return index < cap;
            }
            return false;
        }
        /// <summary>
        /// Sets the magnitude of the vector. Does not modify the original vector. Defaults to Vector2.UnitY if vector length is 0.
        /// </summary>
        public static Vector2 SetMagnitude(this Vector2 vector, float magnitude) => vector.SafeNormalize(Vector2.UnitY) * magnitude;

        /// <summary>
        /// Returns total crit chance, including class-specific and generic boosts.
        /// This method returns 0 for summon crit if Spider Enchant isn't active and enabled.
        /// This is here because generic boosts like Rage Potion DO increase the total summon crit chance value, even though it's normally not checked!
        /// </summary>
        /// <param name="player"></param>
        /// <param name="damageClass"></param>
        /// <returns></returns>
        public static float ActualClassCrit(this Player player, DamageClass damageClass)
            => damageClass == DamageClass.Summon
            && !(player.HasEffect<SpiderEffect>())
            ? 0
            : player.GetTotalCritChance(damageClass);

        public static bool FeralGloveReuse(this Player player, Item item)
            => player.autoReuseGlove && (item.CountsAsClass(DamageClass.Melee) || item.CountsAsClass(DamageClass.SummonMeleeSpeed));

        public static bool CountsAsClass(this DamageClass damageClass, DamageClass intendedClass)
        {
            return damageClass == intendedClass || damageClass.GetEffectInheritance(intendedClass);
        }

        public static DamageClass ProcessDamageTypeFromHeldItem(this Player player)
        {
            if (player.HeldItem.damage <= 0 || player.HeldItem.pick > 0 || player.HeldItem.axe > 0 || player.HeldItem.hammer > 0)
                return DamageClass.Summon;
            else if (player.HeldItem.DamageType.CountsAsClass(DamageClass.Melee))
                return DamageClass.Melee;
            else if (player.HeldItem.DamageType.CountsAsClass(DamageClass.Ranged))
                return DamageClass.Ranged;
            else if (player.HeldItem.DamageType.CountsAsClass(DamageClass.Magic))
                return DamageClass.Magic;
            else if (player.HeldItem.DamageType.CountsAsClass(DamageClass.Summon))
                return DamageClass.Summon;
            else if (player.HeldItem.DamageType != DamageClass.Generic && player.HeldItem.DamageType != DamageClass.Default)
                return player.HeldItem.DamageType;
            else
                return DamageClass.Summon;
        }
    }
}