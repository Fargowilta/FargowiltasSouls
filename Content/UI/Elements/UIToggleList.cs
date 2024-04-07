using System.Reflection;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace FargowiltasSouls.Content.UI.Elements
{
    // I have to reflect here because, to stop it from ordering all its children with its messed up sorting method, I need to override Add and Remove to just not call that method
    // However, _innerList which it accesses is private (bleh)
    public class UIToggleList : UIList
    {
        public readonly static FieldInfo field_innerList;
        public readonly static MethodInfo method_uiElementAppend;
        public readonly static MethodInfo method_uiElementRecalcuate;
        public readonly static MethodInfo method_uiElementRemoveChild;

        static UIToggleList()
        {
            field_innerList = typeof(UIList).GetField("_innerList", BindingFlags.Instance | BindingFlags.NonPublic);
            method_uiElementAppend = typeof(UIElement).GetMethod("Append", BindingFlags.Instance | BindingFlags.Public);
            method_uiElementRecalcuate = typeof(UIElement).GetMethod("Recalculate", BindingFlags.Instance | BindingFlags.Public);
            method_uiElementRemoveChild = typeof(UIElement).GetMethod("RemoveChild", BindingFlags.Instance | BindingFlags.Public);
        }

        public override void Add(UIElement item)
        {
            _items.Add(item);
            method_uiElementAppend.Invoke(field_innerList.GetValue(this), new object[] { item });
            method_uiElementRecalcuate.Invoke(field_innerList.GetValue(this), null);
        }

        public override bool Remove(UIElement item)
        {
            method_uiElementRemoveChild.Invoke(field_innerList.GetValue(this), new object[] { item });
            return _items.Remove(item);
        }
    }
}
