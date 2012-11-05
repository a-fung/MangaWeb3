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
        private static Dictionary<string, bool> inTransitions = new Dictionary<string, bool>();

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
                jQueryObject btn = jQuery.Select(".folders-btn", row).Click(FolderButtonClick).Attribute("data-path", subfolderPath).Text(folder.count == 0 ? folder.name : String.Format("{0} ({1})", folder.name, folder.count)).Attribute("data-count", folder.count.ToString());
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
            bool emptyFolder = jQuery.FromElement(e.Target).GetAttribute("data-count") == "0";
            if (!String.IsNullOrEmpty(dataPath))
            {
                MangaFilter filter = new MangaFilter();
                filter.folder = dataPath.Substr(1);
                MangasModule.Instance.Refresh(filter, emptyFolder);

                if (emptyFolder)
                {
                    ExpandCollapseFolder(dataPath, true);
                }
            }
        }

        private void ExpandButtonClick(jQueryEvent e)
        {
            e.PreventDefault();
            jQueryObject target = jQuery.FromElement(e.Target);
            while (!target.Is("a")) target = target.Parent();
            string dataPath = target.GetAttribute("data-path");

            if (!String.IsNullOrEmpty(dataPath))
            {
                ExpandCollapseFolder(dataPath, target.Children().HasClass("icon-plus"));
            }
        }

        private static void ExpandCollapseFolder(string dataPath, bool expand)
        {
            if (!String.IsNullOrEmpty(dataPath) && !inTransitions[dataPath])
            {
                jQueryObject table = jQuery.Select("table[data-path=\"" + dataPath.Replace("\\", "\\\\") + "\"]");
                jQueryObject expandButton = jQuery.Select("a.folders-expand-btn[data-path=\"" + dataPath.Replace("\\", "\\\\") + "\"]");
                if (table.Length == 0)
                {
                    return;
                }

                if (expand)
                {
                    if (table.Is(":visible"))
                    {
                        return;
                    }

                    table.Show();
                    expandButton.Children().RemoveClass("icon-plus").AddClass("icon-minus");

                    if (Settings.UseAnimation)
                    {
                        if (table.Is(":visible"))
                        {
                            inTransitions[dataPath] = true;
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
                                                    inTransitions[dataPath] = false;
                                                });
                                        });
                                },
                                1);
                        }
                        else
                        {
                            table.AddClass("in");
                        }
                    }
                }
                else
                {
                    if (!table.Is(":visible"))
                    {
                        return;
                    }

                    expandButton.Children().AddClass("icon-plus").RemoveClass("icon-minus");

                    if (Settings.UseAnimation && table.Is(":visible"))
                    {
                        inTransitions[dataPath] = true;

                        Utility.OnTransitionEnd(
                            table.RemoveClass("in").Height(table.GetHeight()),
                            delegate
                            {
                                Utility.OnTransitionEnd(
                                    table.AddClass("height-transition").Height(0),
                                    delegate
                                    {
                                        table.Hide().CSS("height", "");
                                        inTransitions[dataPath] = false;
                                    });
                            });
                    }
                    else
                    {
                        table.Hide().RemoveClass("in");
                    }
                }
            }
        }

        public static void ExpandToFolder(string folder)
        {
            int i = 0, j = 0;
            string separator = Environment.ServerType == ServerType.AspNet ? "\\" : "/";
            folder = separator + folder;

            while ((i = folder.IndexOf(separator, j)) != -1)
            {
                string path = folder.Substr(0, i);
                j = i + 1;

                if (path.Length != 0)
                {
                    ExpandCollapseFolder(path, true);
                }
            }

        }
    }
}
