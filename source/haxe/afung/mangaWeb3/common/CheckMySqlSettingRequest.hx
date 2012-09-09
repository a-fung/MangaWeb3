package afung.mangaWeb3.common;

/**
 * ...
 * @author a-fung
 */

class CheckMySqlSettingRequest extends JsonRequest
{
	public var server:String;
	public var port:Int;
	public var username:String;
	public var password:String;
	public var database:String;
}