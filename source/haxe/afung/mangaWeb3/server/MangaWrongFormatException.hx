package afung.mangaWeb3.server;

import php.Exception;

/**
 * ...
 * @author a-fung
 */

class MangaWrongFormatException extends Exception
{
    public function new(file:String)
    {
        super("The file (" + file + ") is not in the right format");
    }
}