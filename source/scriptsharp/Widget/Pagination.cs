// Pagination.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using jQueryApi;

namespace afung.MangaWeb3.Client.Widget
{
    public delegate int GetTotalPageDelegate();

    public class Pagination
    {
        private jQueryObject parent;
        private Action<int> changePageCallback;
        private GetTotalPageDelegate getTotalPage;
        private string floatDirection;
        private int currentPage;

        [AlternateSignature]
        public extern Pagination(jQueryObject parent, Action<int> changePageCallback, GetTotalPageDelegate getTotalPage);
        public Pagination(jQueryObject parent, Action<int> changePageCallback, GetTotalPageDelegate getTotalPage, string floatDirection)
        {
            this.parent = parent;
            this.changePageCallback = changePageCallback;
            this.getTotalPage = getTotalPage;
            this.floatDirection = floatDirection;
            currentPage = 1;
        }

        [AlternateSignature]
        public extern void Refresh();
        [AlternateSignature]
        public extern void Refresh(bool resetPage);
        public void Refresh(bool resetPage, int setPage)
        {
            if (resetPage)
            {
                currentPage = 1;
            }
            else if (!Script.IsNullOrUndefined(setPage))
            {
                currentPage = setPage;
            }

            parent.Children().Remove();

            int totalPage = getTotalPage();

            if (totalPage > 1)
            {
                jQueryObject pagination = Template.Get("client", "widget-pagination", true).AppendTo(parent);
                if (Script.IsValue(floatDirection) && !String.IsNullOrEmpty(floatDirection))
                {
                    pagination.AddClass("pagination-" + floatDirection);
                }

                AddPage("«", currentPage == 1 ? null : (int?)(currentPage - 1));
                bool ellipsis = true;
                for (int page = 1; page <= totalPage; page++)
                {
                    if (page <= 3 || (page >= currentPage - 2 && page <= currentPage + 2) || page > totalPage - 3)
                    {
                        AddPage(page.ToString(), page);
                        ellipsis = true;
                    }
                    else if (ellipsis)
                    {
                        ellipsis = false;
                        AddPage("…", null);
                    }
                }

                AddPage("»", currentPage == totalPage ? null : (int?)(currentPage + 1));
            }
        }

        private void AddPage(string text, int? page)
        {
            jQueryObject obj = Template.Get("client", "widget-pagination-page", true).AppendTo(jQuery.Select(".widget-pagination-pagelist", parent));
            jQueryObject btn = jQuery.Select(".widget-pagination-page-btn", obj).Text(text).Click(PageButtonClicked);
            if (page == null)
            {
                obj.AddClass("disabled");
            }
            else if (currentPage == page)
            {
                obj.AddClass("active");
            }
            else
            {
                btn.Attribute("data-page", page.ToString());
            }
        }

        private void PageButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
            string page = jQuery.FromElement(e.Target).GetAttribute("data-page");
            if (!String.IsNullOrEmpty(page))
            {
                currentPage = int.Parse(page, 10);
                changePageCallback(currentPage);
                Refresh();
            }
        }
    }
}
