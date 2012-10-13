# Script to gather files for PHP version

# Create output folder
$path = $(get-location).ToString() + "\output";
$dir = new-object "System.IO.DirectoryInfo" -argumentlist $path;
if (!$dir.Exists) { $dir.Create(); }

$path = $(get-location).ToString() + "\output\php";
$dir = new-object "System.IO.DirectoryInfo" -argumentlist $path;
if ($dir.Exists) { 
	Remove-Item $path\* -recurse;
	Start-Sleep -s 1;
}
else {
	$dir.Create();
}


# copy files
copy LICENSE $path
copy NOTICE $path


# create folders
mkdir $path\cover
mkdir $path\mangacache
mkdir $path\temp


# HTML files
copy source\aspnetserver\*.html $path
copy source\aspnetserver\favicon.ico $path


# CSS files
mkdir $path\css
copy source\aspnetserver\css\*.css $path\css


# js files
mkdir $path\js
copy source\aspnetserver\js\afung.MangaWeb3.Client.Admin.js $path\js
copy source\aspnetserver\js\afung.MangaWeb3.Client.Install.js $path\js
copy source\aspnetserver\js\afung.MangaWeb3.Client.js $path\js
copy source\aspnetserver\js\afung.MangaWeb3.Common.js $path\js
copy source\aspnetserver\js\mscorlib.js $path\js

copy source\aspnetserver\js\bootstrap.min.js $path\js
copy source\aspnetserver\js\jquery-1.8.2.min.js $path\js
copy source\aspnetserver\js\jquery.touch.js $path\js
copy source\aspnetserver\js\json2.js $path\js

copy source\php\js\servertype.js $path\js


# debug js files
if ($true) {
copy source\aspnetserver\js\afung.MangaWeb3.Client.Admin.debug.js $path\js
copy source\aspnetserver\js\afung.MangaWeb3.Client.Install.debug.js $path\js
copy source\aspnetserver\js\afung.MangaWeb3.Client.debug.js $path\js
copy source\aspnetserver\js\afung.MangaWeb3.Common.debug.js $path\js
copy source\aspnetserver\js\mscorlib.debug.js $path\js
}


# img files
mkdir $path\img
copy source\aspnetserver\img\*.* $path\img


# lang files
mkdir $path\lang
copy source\aspnetserver\lang\*.* $path\lang


# template files
mkdir $path\template
copy source\aspnetserver\template\*.* $path\template


# files needed during install
copy source\aspnetserver\empty.pdf $path
copy source\aspnetserver\install.sql $path


# php files
Copy-Item source\php\* $path -recurse -force;


# compiled php files
del source\haxephp\index.php
Copy-Item source\haxephp\* $path -recurse -force;


# ckfinder
if ($true) {
mkdir $path\ckfinder
Copy-Item external_libraries\ckfinder_php\* $path\ckfinder -recurse -force;

copy source\ckfinder\config.js $path\ckfinder -force
copy source\ckfinder\config.php $path\ckfinder -force
}


