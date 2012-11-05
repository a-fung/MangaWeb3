// FoldersWidget.cs
//

using System;
using System.Collections.Generic;
using System.Html;
using afung.MangaWeb3.Client.Module;
using afung.MangaWeb3.Common;
using jQueryApi;
using System.Diagnostics;

namespace afung.MangaWeb3.Client.Widget
{
    public class FoldersWidget
    {
        private jQueryObject attachedObject;
        private bool inTransition = false;

        public FoldersWidget(jQueryObject parent, FolderJson[] folders, string folderPath)
        {
            string separator = Environment.ServerType == ServerType.AspNet ? "\\" : "/";
            attachedObject = Template.Get("client", "folders-table", true).AppendTo(parent).Attribute("data-path", folderPath);

            ((List<FolderJson>)(object)folders).Sort(delegate(FolderJson a, FolderJson b)
            {
                return Utility.NaturalCompare(a.name, b.name);
            });

            foreach (FolderJson folder in folders)
            {
                string subfolderPath = folderPath + separator + folder.name;
                jQueryObject row = Template.Get("client", "folders-trow", true).AppendTo(attachedObject.Children());
                jQueryObject btn = jQuery.Select(".folders-btn", row).Click(FolderButtonClick).Attribute("data-path", subfolderPath).Text(folder.count == 0 ? folder.name : String.Format("{0} ({1})", folder.name, folder.count));
                jQueryObject expandBtn = jQuery.Select(".folders-expand-btn", row).Click(ExpandButtonClick);

                if (folder.subfolders != null && folder.subfolders.Length > 0)
                {
                    expandBtn.Attribute("data-path", subfolderPath).Children().AddClass("icon-plus");
                    new FoldersWidget(jQuery.Select(".folders-tcell", row), folder.subfolders, subfolderPath);
                }
            }

            if (folderPath != "")
            {
                attachedObject.Hide();
                if (Settings.UseAnimation)
                {
                    attachedObject.AddClass("fade");
                }
            }
        }

        private void FolderButtonClick(jQueryEvent e)
        {
            e.PreventDefault();
            string dataPath = jQuery.FromElement(e.Target).GetAttribute("data-path");
            if (!String.IsNullOrEmpty(dataPath))
            {
                MangaFilter filter = new MangaFilter();
                filter.folder = dataPath.Substr(1);
                MangasModule.Instance.Refresh(filter);
            }
        }

        private void ExpandButtonClick(jQueryEvent e)
        {
            e.PreventDefault();
            jQueryObject target = jQuery.FromElement(e.Target);
            while (!target.Is("a")) target = target.Parent();
            string dataPath = target.GetAttribute("data-path");
            if (!String.IsNullOrEmpty(dataPath) && !inTransition)
            {
                jQueryObject table = jQuery.Select("table[data-path=\"" + dataPath.Replace("\\", "\\\\") + "\"]", attachedObject);
                if (target.Children().HasClass("icon-plus"))
                {
                    table.Show();
                    target.Children().RemoveClass("icon-plus").AddClass("icon-minus");

                    if (Settings.UseAnimation && table.Is(":visible"))
                    {
                        inTransition = true;
                        int targetHeight = table.GetHeight();
                        table.Height(0);

                        Window.SetTimeout(
                            delegate
                            {
                                Utility.OnTransitionEnd(
                                    table.AddClass("height-transition").Height(targetHeight),
                                    delegate
                                    {
                                        Utility.OnTransitionEnd(
                                            table.RemoveClass("height-transition").CSS("height", "").AddClass("in"),
                                            delegate
                                            {
                                                inTransition = false;
                                            });
                                    });
                            },
                            1);
                    }
                }
                else if (target.Children().HasClass("icon-minus"))
                {
                    target.Children().AddClass("icon-plus").RemoveClass("icon-minus");

                    if (Settings.UseAnimation && table.Is(":visible"))
                    {
                        inTransition = true;

                        Utility.OnTransitionEnd(
                            table.RemoveClass("in").Height(table.GetHeight()),
                            delegate
                            {
                                Utility.OnTransitionEnd(
                                    table.AddClass("height-transition").Height(0),
                                    delegate
                                    {
                                        table.Hide().CSS("height", "");
                                        inTransition = false;
                                    });
                            });
                    }
                    else
                    {
                        table.Hide();
                    }
                }
            }
        }
    }
}
