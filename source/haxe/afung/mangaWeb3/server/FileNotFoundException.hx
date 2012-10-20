package afung.mangaWeb3.server;

import php.Exception;

/**
 * ...
 * @author a-fung
 */

class FileNotFoundException extends Exception
{
    public function new(file:String)
    {
        super("File (" + file + ") not found");
    }
}