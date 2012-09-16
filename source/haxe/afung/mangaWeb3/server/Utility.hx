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
		try
		{
			var json:Dynamic = Json.parse(jsonString);
			switch(Type.typeof(json))
			{
				case TObject:
					return json;
				default:
					return null;
			}
		}
		catch (e:Dynamic)
		{
			return null;
		}
	}
	
	public static function Md5(input:String):String
	{
		return untyped __call__("md5", input);
	}
}