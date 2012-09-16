package afung.mangaWeb3.server;

/**
 * ...
 * @author a-fung
 */

class User 
{
	private var id:Int;
	
	private var username:String;
	
	public var Username(get_Username, null):String;
	
	private function get_Username():String
	{
		return username;
	}
	
	private var password:String;
	
	public var Admin(default, default):Bool;

	private function new() 
	{
		id = -1;
	}
	
	public static function CreateNewUser(username:String, password:String, admin:Bool):User
	{
		var newUser:User = new User();
		newUser.username = username;
		newUser.SetPassword(password);
		newUser.Admin = admin;
		
		return newUser;
	}
	
	public function SetPassword(password:String):Void
	{
		this.password = Utility.Md5(password);
	}
	
	public function MatchPassword(password:String):Bool
	{
		return this.password.toLowerCase() == password.toLowerCase();
	}
	
	public function Save():Void
	{
		var userData:Hash<Dynamic> = new Hash<Dynamic>();
		userData.set("username", username);
		userData.set("password", password);
		userData.set("admin", Admin ? 1 : 0);

		if (id == -1)
		{
			Database.Insert("user", userData);
			id = Database.LastInsertId();
		}
		else
		{
			userData.set("id", id);
			Database.Replace("user", userData);
		}
	}
}