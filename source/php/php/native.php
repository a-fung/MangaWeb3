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
		return exec($cmd, Native::$ExecOutput, Native::$ExecReturnVar);
	}
	public static function MySqlSetCharset($charset) {
		mysql_set_charset($charset);
	}
}

?>