<%@ Control Language="C#" EnableViewState="false" AutoEventWireup="false" Inherits="CKFinder.Settings.ConfigFile" %>
<%@ Import Namespace="CKFinder.Settings" %>
<script runat="server">
    public override bool CheckAuthentication()
    {
        object sessionData = Page.Session["afung.MangaWeb3.Server.Session.finder"];
        string token = Page.Request.QueryString["token"];
        if (sessionData == null || String.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        Dictionary<string, string[][]> dict = (Dictionary<string, string[][]>)sessionData;
        return dict.ContainsKey(token);
    }

    public override void SetConfig()
    {
        object sessionData = Page.Session["afung.MangaWeb3.Server.Session.finder"];
        string token = Page.Request.QueryString["token"];
        if (sessionData == null || String.IsNullOrWhiteSpace(token))
        {
            return;
        }

        Dictionary<string, string[][]> dict = (Dictionary<string, string[][]>)sessionData;
        if (!dict.ContainsKey(token))
        {
            return;
        }

        LicenseName = "";
        LicenseKey = "";
        BaseUrl = "/";
        BaseDir = "\\";
        DisallowUnsafeCharacters = true;
        ForceSingleExtension = true;
        HtmlExtensions = new string[] { "html", "htm", "xml", "js" };
        HideFolders = new string[] { ".svn", "CVS" };
        HideFiles = new string[] { ".*" };

        RoleSessionVar = "CKFinder_UserRole";

        AccessControl acl = AccessControl.Add();
        acl.Role = "*";
        acl.ResourceType = "*";
        acl.Folder = "/";

        acl.FolderView = true;
        acl.FolderCreate = false;
        acl.FolderRename = false;
        acl.FolderDelete = false;

        acl.FileView = true;
        acl.FileUpload = false;
        acl.FileRename = false;
        acl.FileDelete = false;

        DefaultResourceTypes = "";

        ResourceType type;

        string[][] finderData = dict[token];
        foreach (string[] folder in finderData)
        {
            type = ResourceType.Add(folder[0]);
            type.Dir = type.Url = folder[1];
            type.MaxSize = 0;
            type.AllowedExtensions = type.DeniedExtensions = new string[] { };
        }
    }
</script>
