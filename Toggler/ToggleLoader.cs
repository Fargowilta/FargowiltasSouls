using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FargowiltasSouls.Toggler
{
    public static class ToggleLoader
    {
        public static Dictionary<string, Toggle> LoadedToggles;
        public static List<int> HeaderToggles;
        public static Dictionary<string, (string name, int item)> LoadedHeaders;

        public static void Load()
        {
            LoadedToggles = new Dictionary<string, Toggle>();
            HeaderToggles = new List<int>();
            LoadedHeaders = new Dictionary<string, (string name, int item)>();
            LoadTogglesFromAssembly(FargowiltasSouls.Instance.Code);
        }

        public static void LoadTogglesFromAssembly(Assembly assembly)
        {
            Type[] types = assembly.GetTypes();
            List<ToggleCollection> collections = new List<ToggleCollection>();

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

            IEnumerable<ToggleCollection> orderedCollections = collections.OrderBy((collection) => collection.Priority);
            FargowiltasSouls.Instance.Logger.Info($"ToggleCollections found: {orderedCollections.Count()}");

            foreach (ToggleCollection collection in orderedCollections)
            {
                List<Toggle> toggleCollectionChildren = collection.Load(LoadedToggles.Count - 1);

                foreach (Toggle toggle in toggleCollectionChildren)
                {
                    RegisterToggle(toggle);
                }
            }
        }

        public static void Unload()
        {
            if (LoadedToggles != null)
                LoadedToggles.Clear();
            if (HeaderToggles != null)
                HeaderToggles.Clear();
            if (LoadedHeaders != null)
                LoadedHeaders.Clear();
        }

        public static void RegisterToggle(Toggle toggle)
        {
            if (LoadedToggles.ContainsKey(toggle.InternalName)) throw new Exception("Toggle with internal name " + toggle.InternalName + " is already registered");

            LoadedToggles.Add(toggle.InternalName, toggle);

            if (LoadedHeaders.ContainsKey(toggle.InternalName))
                HeaderToggles.Add(LoadedToggles.Values.ToList().FindIndex((t) => t.InternalName == toggle.InternalName));
        }
    }
}
