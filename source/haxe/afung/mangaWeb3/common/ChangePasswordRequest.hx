package afung.mangaWeb3.common;

/**
 * ...
 * @author a-fung
 */

class ChangePasswordRequest extends JsonRequest
{
    public var currentPassword:String;
    public var newPassword:String;
    public var newPassword2:String;
}