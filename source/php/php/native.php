<?php

class Native {
	public function __construct() {}
	static $ExecOutput;
	static $ExecReturnVar;
	public static function ExtensionLoaded($name) {
		return extension_loaded($name);
	}
	public static function ClassExists($name) {
		return class_exists($name);
	}
	public static function Exec($cmd) {
		$output = array();
		$ll = exec($cmd, $output, $rtnVar);
		Native::$ExecOutput = $output;
		Native::$ExecReturnVar = $rtnVar;
		unset($output, $rtnVar);
		return $ll;
	}
	public static function MySqlSetCharset($charset) {
		mysql_set_charset($charset);
	}
	public static function AddSlashes($str) {
		return addslashes($str);
	}
}

?>