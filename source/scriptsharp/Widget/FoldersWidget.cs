// FoldersWidget.cs
//

using System;
using System.Collections.Generic;
using afung.MangaWeb3.Client.Module;
using afung.MangaWeb3.Common;
using jQueryApi;

namespace afung.MangaWeb3.Client.Widget
{
    public class FoldersWidget
    {
        private jQueryObject attachedObject;

        public FoldersWidget(jQueryObject parent, FolderJson[] folders, string folderPath)
        {
            string separator = Environment.ServerType == ServerType.AspNet ? "\\" : "/";
            attachedObject = Template.Get("client", "folders-table", true).AppendTo(parent).Attribute("data-path", folderPath);

            if (folderPath != "")
            {
                attachedObject.Hide();
            }

            foreach (FolderJson folder in folders)
            {
                string subfolderPath = folderPath + separator + folder.name;
                jQueryObject row = Template.Get("client", "folders-trow", true).AppendTo(attachedObject.Children());
                jQuery.Select(".folders-btn", row).Text(folder.name).Click(FolderButtonClick).Attribute("data-path", subfolderPath);
                jQuery.Select(".folders-expand-btn", row).Click(ExpandButtonClick);

                if (folder.subfolders != null && folder.subfolders.Length > 0)
                {
                    jQuery.Select(".folders-expand-btn", row).Attribute("data-path", subfolderPath).Children().AddClass("icon-plus");
                    new FoldersWidget(jQuery.Select(".folders-tcell", row), folder.subfolders, subfolderPath);
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
            if (!String.IsNullOrEmpty(dataPath))
            {
                jQueryObject table = jQuery.Select("table[data-path=\"" + dataPath.Replace("\\", "\\\\") + "\"]", attachedObject);
                if (target.Children().HasClass("icon-plus"))
                {
                    table.Show();
                    target.Children().RemoveClass("icon-plus").AddClass("icon-minus");
                }
                else if (target.Children().HasClass("icon-minus"))
                {
                    table.Hide();
                    target.Children().AddClass("icon-plus").RemoveClass("icon-minus");
                }
            }
        }
    }
}
