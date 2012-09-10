package afung.mangaWeb3.server.install;

import afung.mangaWeb3.server.AjaxBase;
import afung.mangaWeb3.server.handler.HandlerBase;
import afung.mangaWeb3.server.install.handler.CheckMySqlSettingRequestHandler;
import afung.mangaWeb3.server.install.handler.InstallRequestHandler;
import afung.mangaWeb3.server.install.handler.PreInstallCheckRequestHandler;

/**
 * ...
 * @author a-fung
 */

class InstallAjax extends AjaxBase
{
	public function new()
	{
		super();
	}
	
	public static function main():Void
	{
		new InstallAjax().Page_Load();
	}
	
	private static var handlers:Array<HandlerBase> = null;
	
	private function Page_Load():Void
	{
		if (handlers == null)
		{
			handlers = [
				new PreInstallCheckRequestHandler(),
				new CheckMySqlSettingRequestHandler(),
				new InstallRequestHandler(),
			];
		}
		
        HandleRequest(handlers);
	}
}