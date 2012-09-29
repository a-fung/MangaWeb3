package afung.mangaWeb3.server;

import php.Lib;

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
            var json:Dynamic = untyped __call__("json_decode", jsonString);
            if (!untyped __call__("is_object", json))
            {
                return null;
            }
            
            json = ProcessJson(json);
            return json;
        }
        catch (e:Dynamic)
        {
            return null;
        }
    }
    
    private static function ProcessJson(json:Dynamic):Dynamic
    {
        var fields:Array<String> = Reflect.fields(json);
        var obj = { };
        
        for (fieldName in fields)
        {
            var field:Dynamic = Reflect.field(json, fieldName);
            
            if (untyped __call__("is_object", field))
            {
                field = ProcessJson(field);
            }
            else if (untyped __call__("is_array", field))
            {
                var haxeArray:Array<Dynamic> = Lib.toHaxeArray(field);
                ProcessJsonArray(haxeArray);
                field = haxeArray;
            }
            
            Reflect.setField(obj, fieldName, field);
        }
        
        return obj;
    }
    
    private static function ProcessJsonArray(haxeArray:Array<Dynamic>):Void
    {
        for (i in 0...haxeArray.length)
        {
            if (untyped __call__("is_object", haxeArray[i]))
            {
                ProcessJson(haxeArray[i]);
            }
            else if (untyped __call__("is_array", haxeArray[i]))
            {
                var haxeArray2:Array<Dynamic> = Lib.toHaxeArray(haxeArray[i]);
                ProcessJsonArray(haxeArray2);
                haxeArray[i] = haxeArray2;
            }
        }
    }
    
    public static function Md5(input:String):String
    {
        return untyped __call__("md5", input);
    }
    
    public static function ArrayContains(array:Array<Dynamic>, value:Dynamic):Bool
    {
        for (e in array)
        {
            if (e == value)
            {
                return true;
            }
        }
        
        return false;
    }
}