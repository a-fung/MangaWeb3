package afung.mangaWeb3.common;

/**
 * ...
 * @author a-fung
 */

class JsonRequest 
{
	public var type:String;
    
    public function new()
    {
        var fullName:String = Type.getClassName(Type.getClass(this));
        type = fullName.substr(fullName.lastIndexOf(".") + 1);
    }
}