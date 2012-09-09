package afung.mangaWeb3.server;

import afung.mangaWeb3.common.ErrorResponse;
import afung.mangaWeb3.common.PreInstallCheckRequest;
import afung.mangaWeb3.server.install.InstallAjax;

/**
 * ...
 * @author a-fung
 */

class ServerAjax extends AjaxBase
{
	public function new()
	{
		super();
	}
	
	public static function main() 
	{
		new ServerAjax().Page_Load();
	}
	
	private function Page_Load():Void
	{
	}
}