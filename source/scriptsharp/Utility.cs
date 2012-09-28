// Utility.cs
//

using System;
using System.Collections.Generic;
using System.Html;
using jQueryApi;

namespace afung.MangaWeb3.Client
{
    public class Utility
    {
        public static int[] GetSelectedCheckboxIds(string className)
        {
            int[] ids = { };
            jQuery.Select(String.Format(".{0}:checked", className)).Each(delegate(int index, Element element)
            {
                string id = jQuery.FromElement(element).GetValue();
                if (!String.IsNullOrEmpty(id))
                {
                    ids[ids.Length] = int.Parse(id, 10);
                }
            });

            return ids;
        }

        public static void FixDropdownTouch(jQueryObject dropdown)
        {
            dropdown.Bind("touchstart", delegate(jQueryEvent e) { e.StopPropagation(); });
        }
    }
}
