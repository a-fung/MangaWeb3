# Script to gather files for ASP.NET version

# Create output folder
$path = $(get-location).ToString() + "\output";
$dir = new-object "System.IO.DirectoryInfo" -argumentlist $path;
if (!$dir.Exists) { $dir.Create(); }

$path = $(get-location).ToString() + "\output\aspnet";
$dir = new-object "System.IO.DirectoryInfo" -argumentlist $path;
if ($dir.Exists) { 
	$dir.Delete($true);
	Start-Sleep -s 1;
}
$dir.Create();


# copy files
copy LICENSE $path


# HTML files
copy source\aspnetserver\*.html $path


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
copy source\aspnetserver\js\jquery-1.8.1.min.js $path\js

copy source\aspnetserver\js\servertype.js $path\js


# debug js files
if ($false) {
copy source\aspnetserver\js\afung.MangaWeb3.Client.Admin.debug.js $path\js
copy source\aspnetserver\js\afung.MangaWeb3.Client.Install.debug.js $path\js
copy source\aspnetserver\js\afung.MangaWeb3.Client.debug.js $path\js
copy source\aspnetserver\js\afung.MangaWeb3.Common.debug.js $path\js
copy source\aspnetserver\js\mscorlib.debug.js $path\js
}


# img files
mkdir $path\img
copy source\aspnetserver\img\*.* $path\img


# aspx files
copy source\aspnetserver\*.aspx $path
copy source\aspnetserverinstall\*.aspx $path


# binary files
mkdir $path\bin
copy source\aspnetserver\bin\*.dll $path\bin
copy source\aspnetserverinstall\bin\afung.MangaWeb3.Server.Install.dll $path\bin


# web.config
copy source\aspnetserver\web.config $path


