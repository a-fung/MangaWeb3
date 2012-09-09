package afung.mangaWeb3.server;
import haxe.Json;

/**
 * ...
 * @author a-fung
 */

class Utility 
{
	public static function ParseJson(jsonString:String):Dynamic
	{
		return Json.parse(jsonString);
	}
}