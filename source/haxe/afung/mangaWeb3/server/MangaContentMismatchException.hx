package afung.mangaWeb3.server;

import php.Exception;

/**
 * ...
 * @author a-fung
 */

class MangaContentMismatchException extends Exception
{
    public function new(file:String) 
    {
        super("The file (" + file + ") has content not match that in the database");
    }
}