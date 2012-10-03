package afung.mangaWeb3.server;

import php.Lib;
import php.NativeArray;

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
                haxeArray[i] = ProcessJson(haxeArray[i]);
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
    
    public static function IsValidStringForDatabase(str:String):Bool
    {
        return str == Remove4PlusBytesUtf8Chars(str);
    }
    
    public static function Remove4PlusBytesUtf8Chars(str:String):String
    {
        var i:Int = 0;
        while (i < str.length)
        {
            var code:Int = str.charCodeAt(i);
            
            if (code >= 0 && code < 128)
            {
                // valid one byte char
                i++;
                continue;
            }
            else 
            {
                var extraBytes:Int = 0;
                if (code >= 192 && code < 224)
                {
                    extraBytes = 1;
                }
                else if (code >= 224 && code < 240)
                {
                    extraBytes = 2;
                }
                else if (code >= 240 && code < 248)
                {
                    extraBytes = 3;
                }
                else if (code >= 248 && code < 252)
                {
                    extraBytes = 4;
                }
                else if (code >= 252 && code < 254)
                {
                    extraBytes = 5;
                }
                
                if (extraBytes > 0 && extraBytes < 3 && i + extraBytes < str.length)
                {
                    var valid:Bool = true;
                    var j:Int = 1;
                    while (j <= extraBytes)
                    {
                        var code2:Int = str.charCodeAt(i + j);
                        if (code2 >= 128 && code2 < 192)
                        {
                        }
                        else
                        {
                            valid = false;
                            break;
                        }
                        j++;
                    }
                    
                    if (valid)
                    {
                        i += 1 + extraBytes;
                        continue;
                    }
                }
                else if (i + extraBytes >= str.length)
                {
                    extraBytes = str.length - i - 1;
                }
                
                // invalid byte or longer than MySQL is accepting
                str = str.substr(0, i) + "?" + str.substr(i + 1 + extraBytes);
                i++;
                continue;
            }
        }
        return str;
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
    
    public static function ArrayStringContains(array:Array<String>, value:String):Bool
    {
        for (e in array)
        {
            if (e.toLowerCase() == value.toLowerCase())
            {
                return true;
            }
        }
        
        return false;
    }
    
    public static function JoinNativeArray(original:NativeArray):String
    {
        var buf:StringBuf = new StringBuf();
        for (i in 0...untyped __call__("count", original))
        {
            buf.add(Std.string(original[i]));
            buf.addChar("\n".code);
        }
        return buf.toString();
    }
}