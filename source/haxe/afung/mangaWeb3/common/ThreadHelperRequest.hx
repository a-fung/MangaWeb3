package afung.mangaWeb3.common;

/**
 * ...
 * @author a-fung
 */

class ThreadHelperRequest extends JsonRequest
{
    public var methodName:String;
    public var parameters:Array<Dynamic>;
    
    public function new()
    {
		var fullName:String = Type.getClassName(Type.getClass(this));
        type = fullName.substr(fullName.lastIndexOf(".") + 1);
    }
}