package afung.mangaWeb3.server;

import php.db.Connection;
import php.db.Mysql;

/**
 * ...
 * @author a-fung
 */

class Database 
{
	public static function GetConnnection(server:String, port:Int, username:String, password:String, database:String):Connection
	{
		var connection:Connection = Mysql.connect({
			host : server,
			port : port,
			user : username,
			pass : password,
			socket : null,
			database : database
		});
		
		Native.MySqlSetCharset("utf8");
		
		return connection;
	}
}