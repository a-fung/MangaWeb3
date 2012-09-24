package afung.mangaWeb3.server;

import php.db.Connection;
import php.db.Mysql;
import php.db.ResultSet;

/**
 * ...
 * @author a-fung
 */

class Database 
{
	public static function GetConnection(server:String, port:Int, username:String, password:String, database:String):Connection
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
	
	private static var _connection:Connection = null;
	
	private static function DefaultConnection():Connection
	{
		if (_connection == null)
		{
			_connection = GetConnection(
				Config.MySQLServer,
				Config.MySQLPort,
				Config.MySQLUser,
				Config.MySQLPassword,
				Config.MySQLDatabase);
		}
		
		return _connection;
	}
	
	public static function Quote(str:String):String
	{
		//if (Std.is(str, String))
		//{
		//	str = Util.RemoveNonUtf8Chars(str);
		//}
		return DefaultConnection().quote(str);
	}
	
	public static function Select(table:String, where:String = "", order:String = "", limit:String = "", fields:String = "*") : Array<Hash<Dynamic>>
	{
		if (where != "")
		{
			where = " WHERE " + where;
		}
		
		if (order != "")
		{
			order = " ORDER BY " + order;
		}
		
		if (limit != "")
		{
			limit = " LIMIT " + limit;
		}
		
		var sql:String = "SELECT " + fields + " FROM `" + table + "`" + where + order + limit;
		var resultSet:ResultSet = DefaultConnection().request(sql);
		var rtn:Array<Hash<Dynamic>> = new Array<Hash<Dynamic>>();
		var resultFields:Array<String> = resultSet.getFieldsNames();
		var i:Int = 0;
		while (resultSet.hasNext())
		{
			resultSet.next();
			var row:Hash<Dynamic> = new Hash<Dynamic>();
			var j:Int = 0;
			while (j < resultFields.length)
			{
				row.set(resultFields[j], resultSet.getResult(j));
				j++;
			}
			rtn.insert(i, row);
			i++;
		}
		
		return rtn;
	}
	
	private static function InsertOrReplace(method:String, table:String, data:Hash<Dynamic>):Void
	{
		var field:String = "";
		var value:String = "";
		
		for (key in data.keys())
		{
			field += ",`" + key + "`";
			value += "," + Quote(data.get(key));
		}
		
		if (field == "" || value == "")
		{
			return;
		}
		
		field = field.substr(1);
		value = value.substr(1);
		
		var sql:String = method + " INTO `" + table + "` ( " + field + " ) VALUES ( " + value + " )";
		DefaultConnection().request(sql);
	}
	
	public static function Insert(table:String, data:Hash<Dynamic>):Void
	{
		InsertOrReplace("INSERT", table, data);
	}
	
	public static function Replace(table:String, data:Hash<Dynamic>):Void
	{
		InsertOrReplace("REPLACE", table, data);
	}
	
	public static function Update(table:String, data:Hash<Dynamic>, where:String, limit:String = ""):Void
	{
		var clause:String = "";
		for (key in data.keys())
		{
			clause += ",`" + key + "`=" + Quote(data.get(key));
		}
		
		if (clause == "")
		{
			return;
		}
		
		clause = clause.substr(1);
		
		if(limit != "")
		{
			limit = " LIMIT " + limit;
		}
		
		var sql:String = "UPDATE `" + table + "` SET " + clause + " WHERE " + where + limit;
		DefaultConnection().request(sql);
	}
	
	public static function Clear(table:String):Void
	{
		var sql:String = "TRUNCATE TABLE `" + table + "`";
		DefaultConnection().request(sql);
	}
	
	public static function Delete(table:String, where:String):Void
	{
		var sql:String = "DELETE FROM `" + table + "` WHERE " + where;
		DefaultConnection().request(sql);
	}
	
	public static function ExecuteSql(sql:String):Void
	{
		DefaultConnection().request(sql);
	}
	
	public static function LastInsertId():Int
	{
		return DefaultConnection().lastInsertId();
	}
    
    public static function BuildWhereClauseOr(field:String, values:Array<Dynamic>):String
    {
        if (values == null || values.length == 0 || field == null || field == "")
        {
            return "FALSE";
        }

        var clauseBuilder:StringBuf = new StringBuf();

        for (value in values)
        {
            clauseBuilder.add(" OR `" + field + "`=" + Quote(value.ToString()));
        }

        return "(" + clauseBuilder.toString().substr(4) + ")";
    }
}