package ;

/**
 * ...
 * @author a-fung
 */

extern class ZipArchive 
{
    public function new():Void;
    public function addEmptyDir(dirname:String):Bool;
    public function addFile(filename:String, localname:String = null, start:Int = 0, length:Int = 0):Bool;
    public function addFromString(localname:String, content:String):Bool;
    public function close():Bool;
    public function deleteIndex(index:Int):Bool;
    public function deleteName(name:String):Bool;
    public function extractTo(destination:String, entries:Dynamic = null):Bool;
    public function getArchiveComment(flags:Int = 0):String;
    public function getCommentIndex(index:Int, flags:Int = 0):String;
    public function getCommentName(name:String, flags:Int = 0):String;
    public function getFromIndex(index:Int, length:Int = 0, flags:Int = 0):Dynamic;
    public function getFromName(name:String, length:Int = 0, flags:Int = 0):Dynamic;
    public function getNameIndex(index:Int, flags:Int = 0):String;
    public function getStatusString():String;
    public function getStream(name:String):Dynamic;
    public function locateName(name:String, flags:Int = 0):Dynamic;
    public function open(filename:String, flags:Int = 0):Dynamic;
    public function renameIndex(index:Int, newname:String):Bool;
    public function renameName(name:String, newname:String):Bool;
    public function setArchiveComment(comment:String):Dynamic;
    public function setCommentIndex(index:Int, comment:String):Dynamic;
    public function setCommentName(name:String, comment:String):Dynamic;
    public function statIndex(index:Int, flags:Int = 0):Dynamic;
    public function statName(name:String, flags:Int = 0):Dynamic;
    public function unchangeAll():Bool;
    public function unchangeArchive():Bool;
    public function unchangeIndex(index:Int):Bool;
    public function unchangeName(name:String):Bool;
    
    public var status:Dynamic;
    public var statusSys:Dynamic;
    public var numFiles:Int;
    public var filename:String;
    public var comment:Dynamic;
    
    public static inline function zip_close(zip:Dynamic):Void return untyped __call__("zip_close", zip)
    public static inline function zip_entry_close(zip_entry:Dynamic):Void return untyped __call__("zip_entry_close", zip_entry)
    public static inline function zip_entry_compressedsize(zip_entry:Dynamic):Int return untyped __call__("zip_entry_compressedsize", zip_entry)
    public static inline function zip_entry_compressionmethod(zip_entry:Dynamic):String return untyped __call__("zip_entry_compressionmethod", zip_entry)
    public static inline function zip_entry_filesize(zip_entry:Dynamic):Int return untyped __call__("zip_entry_filesize", zip_entry)
    public static inline function zip_entry_name(zip_entry:Dynamic):String return untyped __call__("zip_entry_name", zip_entry)
    public static inline function zip_entry_open(zip:Dynamic, zip_entry:Dynamic, mode:String = null):Bool return untyped __call__("zip_entry_open", zip, zip_entry, mode)
    public static inline function zip_entry_read(zip_entry:Dynamic, length:Int = 0):String return untyped __call__("zip_entry_read", zip_entry, length)
    public static inline function zip_open(filename:String):Dynamic return untyped __call__("zip_open", filename)
    public static inline function zip_read(zip:Dynamic):Dynamic return untyped __call__("zip_read", zip)
}