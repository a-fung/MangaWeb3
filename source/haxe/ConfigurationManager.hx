package;

import php.NativeArray;

/**
 * ...
 * @author a-fung
 */

extern class ConfigurationManager 
{
    static function __init__():Void untyped {
        __call__("require_once", "config.php");
    }
	
    public static function AppSettings():NativeArray;
}