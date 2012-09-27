package ;

import php.Exception;

/**
 * ...
 * @author a-fung
 */

extern class RarException extends Exception
{
    public static function isUsingExceptions():Bool;
    public static function setUsingExceptions(using_exceptions:Bool):Void;
}