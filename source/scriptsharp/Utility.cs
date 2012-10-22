// Utility.cs
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Html;
using System.Runtime.CompilerServices;
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

        public static void OnTransitionEnd(jQueryObject obj, Action trigger)
        {
            if (BootstrapTransition.Support)
            {
                obj.One(
                    BootstrapTransition.TransitionEventEndName,
                    delegate(jQueryEvent e)
                    {
                        trigger();
                    });
            }
            else
            {
                trigger();
            }
        }

        [AlternateSignature]
        public extern static int NaturalCompare(object a, object b);
        public static int NaturalCompare(string a, string b)
        {
            RegularExpression regex = new RegularExpression("[0-9]+");

            string[] aa = regex.Exec(a), bb = regex.Exec(b);

            if (aa != null && bb != null)
            {
                string aaa = a.Substr(0, a.IndexOf(aa[0])), bbb = b.Substr(0, b.IndexOf(bb[0]));

                if (aaa == bbb)
                {
                    if (aa[0] == bb[0])
                    {
                        return NaturalCompare(a.Substr(aaa.Length + aa[0].Length), b.Substr(bbb.Length + bb[0].Length));
                    }
                    else
                    {
                        int ia = int.Parse(aa[0], 10), ib = int.Parse(bb[0], 10);
                        if (ia == ib)
                        {
                            return bb[0].Length - aa[0].Length;
                        }
                        else
                        {
                            return ia - ib;
                        }
                    }
                }
            }

            return a.CompareTo(b);
        }
    }
}
