using FargowiltasSouls.Core.AccessoryEffectSystem;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FargowiltasSouls.Core.Toggler
{
    public static class ToggleLoader
    {
        public static Dictionary<AccessoryEffect, Toggle> LoadedToggles
        {
            get;
            set;
        }
        public static HashSet<Header> LoadedHeaders
        {
            get;
            set;
        }


        public static void Load()
        {
            //LoadTogglesFromAssembly(FargowiltasSouls.Instance.Code);
        }

        public static void Unload()
        {
            LoadedToggles?.Clear();
            LoadedHeaders?.Clear();
        }


        public static void LoadTogglesFromAssembly(Assembly assembly)
        {
            // Toggles are now registered from the AccessoryEffect system. Headers are now registered from each derived class of the Header baseclass.

            #region Collection Loading (outdated)
            /*
            Type[] types = assembly.GetTypes();
            List<ToggleCollection> collections = new();

            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];
                if (typeof(ToggleCollection).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    ToggleCollection toggles = (ToggleCollection)Activator.CreateInstance(type);

                    if (toggles.Active)
                    {
                        collections.Add(toggles);
                    }

                }
            }

            var orderedCollections = collections.OrderBy((collection) => collection.Priority);
            FargowiltasSouls.Instance.Logger.Info($"ToggleCollections found: {orderedCollections.Count()}");

            foreach (ToggleCollection collection in orderedCollections)
            {
                List<Toggle> toggleCollectionChildren = collection.Load();

                foreach (Toggle toggle in toggleCollectionChildren)
                {
                    RegisterToggle(toggle);
                }
            }
            */
            #endregion
        }

        public static void RegisterToggle(Toggle toggle)
        {

            LoadedToggles ??= new Dictionary<AccessoryEffect, Toggle>();
            if (LoadedToggles.ContainsKey(toggle.Effect)) throw new Exception("Toggle of effect " + toggle.Effect.Name + " is already registered");

            LoadedToggles.Add(toggle.Effect, toggle);

        }
        public static void RegisterHeader(Header header)
        {

            LoadedHeaders ??= new HashSet<Header>();
            //if (LoadedHeaders.Contains(header)) throw new Exception("Header with internal name " + header.Name + " is already registered");

            LoadedHeaders.Add(header);
        }
    }
}
