<?php

@session_start();

function CheckAuthentication()
{
	if (!isset($_SESSION['afung.MangaWeb3.Server.Session.finder']) || !isset($_GET['token']))
	{
		return false;
	}
	
	$dict = $_SESSION['afung.MangaWeb3.Server.Session.finder'];
	$token = $_GET['token'];
	
	return isset($dict[$token]);
}

if (!isset($_SESSION['afung.MangaWeb3.Server.Session.finder']) || !isset($_GET['token']))
{
	die();
}

$dict = $_SESSION['afung.MangaWeb3.Server.Session.finder'];
$token = $_GET['token'];

if(!isset($dict[$token]))
{
	die();
}

$config['LicenseName'] = '';
$config['LicenseKey'] = '';
$baseUrl = '/';
$baseDir = '/';
$config['CheckDoubleExtension'] = true;
$config['DisallowUnsafeCharacters'] = true;
$config['FilesystemEncoding'] = 'UTF-8';
$config['HtmlExtensions'] = array('html', 'htm', 'xml', 'js');
$config['HideFolders'] = array(".svn", "CVS");
$config['HideFiles'] = array(".*");

$config['RoleSessionVar'] = 'CKFinder_UserRole';

$config['AccessControl'][] = array(
		'role' => '*',
		'resourceType' => '*',
		'folder' => '/',

		'folderView' => true,
		'folderCreate' => false,
		'folderRename' => false,
		'folderDelete' => false,

		'fileView' => true,
		'fileUpload' => false,
		'fileRename' => false,
		'fileDelete' => false);

$config['DefaultResourceTypes'] = '';

$finderData = $dict[$token];
for ($i = 0; $i < count($finderData); $i++)
{
	$config['ResourceType'][] = array(
			'name' => $finderData[$i][0],
			'url' => $finderData[$i][1],
			'directory' => $finderData[$i][1],
			'maxSize' => 0,
			'allowedExtensions' => '',
			'deniedExtensions' => '');
}
