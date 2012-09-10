package;

import php.NativeArray;

/**
 * ...
 * @author a-fung
 */

extern class Native
{
    static function __init__():Void untyped {
        __call__("require_once", "php/native.php");
    }
	public function new():Void;
	
	public static var ExecOutput:NativeArray;
	public static var ExecReturnVar:Int;
	
	public static function ExtensionLoaded(name:String):Bool;
	public static function ClassExists(name:String):Bool;
	public static function Exec(cmd:String):String;
	public static function MySqlSetCharset(charset:String):Void;
	public static function AddSlashes(str:String):String;
}