using System.Collections.Generic;

namespace FargowiltasSouls.Core.Toggler
{
    /// <summary>
    /// Deprecated.
    /// </summary>
    public abstract class ToggleCollection
    {
        public abstract string Mod { get; }
        public abstract string SortCategory { get; }
        public abstract float Priority { get; }

        public abstract bool Active
        {
            get;
        }


        public List<Toggle> Load()
        {
            return new List<Toggle>();
            /*
            // All string (toggles) and int (header) fields
            FieldInfo[] fields = GetType().GetFields();
            // The amount of int fields, ie toggles
            List<Toggle> ret = new();

            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].FieldType != typeof(int)) // Register as toggle if it's a string
                    ret.Add(new Toggle(fields[i].Name, Mod, SortCategory));
                else // ...or as a header if it's an int
                {
                    ToggleLoader.LoadedHeaders.Add(fields[i + 1].Name, (fields[i].Name, (int)fields[i].GetValue(this)));
                }
            }

            // Return the toggles (strings)
            return ret;
            */
        }
    }
}
