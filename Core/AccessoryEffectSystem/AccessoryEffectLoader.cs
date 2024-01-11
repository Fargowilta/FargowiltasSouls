using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.AccessoryEffectSystem
{
    public static class AccessoryEffectLoader
    {
        public static HashSet<AccessoryEffect> AccessoryEffects = new();
        public static List<EffectFields> EffectFields = new();
        internal static void Register(AccessoryEffect effect)
        {
            AccessoryEffects.Add(effect);
            
            if (effect.HasToggle)
            {
                if (effect.ToggleHeader == null)
                    throw new Exception($"Accessory effect {effect.Name} tried to register a toggle without a valid header");
                ToggleLoader.RegisterToggle(new Toggle(effect, effect.Mod.Name, effect.ToggleHeader.SortCategory, effect.ToggleHeader));
            }
        }
        internal static void Register(EffectFields effect)
        {
            EffectFields.Add(effect);
        }
        
        public static void AddEffect<T>(this Player player, Item item) where T : AccessoryEffect
        {
            AccessoryEffect effect = ModContent.GetInstance<T>();
            AccessoryEffectPlayer effectPlayer = player.AccessoryEffects();
            if (player.FargoSouls().MutantPresence) // todo: implement system for mutant presence
            {

            }
            if (!effect.HasToggle || player.GetToggleValue<T>())
            {
                effectPlayer.ActiveEffects.Add(effect);
                effectPlayer.EffectItems[effect] = item;
            }
        }
        public static bool HasEffect<T>(this Player player) where T : AccessoryEffect => player.HasEffect(ModContent.GetInstance<AccessoryEffect>());
        public static bool HasEffect(this Player player, AccessoryEffect accessoryEffect) => player.AccessoryEffects().ActiveEffects.Contains(accessoryEffect);
        public static Item EffectItem<T>(this Player player) where T : AccessoryEffect => player.AccessoryEffects().EffectItems.TryGetValue(ModContent.GetInstance<T>(), out Item item) ? item : null;
        public static T EffectType<T>() where T : AccessoryEffect => ModContent.GetInstance<T>();
        public static AccessoryEffect EffectType(string internalName)  => ModContent.Find<AccessoryEffect>(internalName);
        public static T GetEffectFields<T>(this Player player) where T : EffectFields =>
            GetEffectFields(ModContent.GetInstance<T>(), player);
        public static T GetEffectFields<T>(T baseInstance, Player player) where T : EffectFields
        => player.AccessoryEffects().EffectFieldsInstances[baseInstance.Index] as T ?? throw new KeyNotFoundException($"Instance of '{typeof(T).Name}' does not exist on the current player.");
    }
}
