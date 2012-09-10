package afung.mangaWeb3.common;

/**
 * ...
 * @author a-fung
 */

class InstallRequest extends JsonRequest
{
	public var mysqlServer:String;
	public var mysqlPort:Int;
	public var mysqlUser:String;
	public var mysqlPassword:String;
	public var mysqlDatabase:String;

	public var sevenZipPath:String;
	public var pdfinfoPath:String;
	public var mudrawPath:String;

	public var zip:Bool;
	public var rar:Bool;
	public var pdf:Bool;

	public var admin:String;
	public var password:String;
	public var password2:String;
	
	public function new() 
	{
	}
}