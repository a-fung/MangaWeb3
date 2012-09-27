package ;

/**
 * ...
 * @author a-fung
 */

extern class RarEntry 
{
    public function extract(dir:String, filepath:String = "", password:String = null, extended_data:Bool = false):Bool;
    public function getAttr():Int;
    public function getCrc():String;
    public function getFileTime():String;
    public function getHostOs():Int;
    public function getMethod():Int;
    public function getName():String;
    public function getPackedSize():Int;
    public function getStream(password:String = null):Dynamic;
    public function getUnpackedSize():Int;
    public function getVersion():Int;
    public function isDirectory():Bool;
    public function isEncrypted():Bool;
    public function __toString():String;
}