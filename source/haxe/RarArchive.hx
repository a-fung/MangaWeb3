package ;

import php.NativeArray;

/**
 * ...
 * @author a-fung
 */

extern class RarArchive 
{
    public function close():Bool;
    public function getComment():String;
    public function getEntries():NativeArray;
    public function getEntry(entryname:String):RarEntry;
    public function isBroken():Bool;
    public function isSolid():Bool;
    public static function open(filename:String, password:String = null, volume_callback:Dynamic = null):RarArchive;
    public function setAllowBroken(allow_broken:Bool):Bool;
    public function __toString():String;
}