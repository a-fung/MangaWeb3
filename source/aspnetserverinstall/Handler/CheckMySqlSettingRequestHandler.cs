using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;
using afung.MangaWeb3.Server.Handler;
using MySql.Data.MySqlClient;

namespace afung.MangaWeb3.Server.Install.Handler
{
    public class CheckMySqlSettingRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(CheckMySqlSettingRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            CheckMySqlSettingRequest request = Utility.ParseJson<CheckMySqlSettingRequest>(jsonString);

            MySqlConnection connection = null;
            bool pass = false;
            try
            {
                connection = Database.GetConnection(
                    request.server,
                    request.port,
                    request.username,
                    request.password,
                    request.database);

                new MySqlCommand(String.Format("SHOW FULL TABLES FROM `{0}`", MySqlHelper.EscapeString(request.database)), connection).ExecuteReader().Close();

                pass = true;
            }
            catch (Exception)
            {
            }

            if (connection != null)
            {
                connection.Close();
            }

            CheckMySqlSettingResponse response = new CheckMySqlSettingResponse();
            response.pass = pass;

            ajax.ReturnJson(response);
        }
    }
}