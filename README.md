MangaWeb3
====================

A web app for reading manga on all your devices.

Set up a server. Add your manga (or other books). Open the web app from your favourite browser.
The server will read your manga and resize to fit your browser.

The web app works on desktop browsers, phone browsers, tablet browsers and even browsers on Kindle.

There are two favours of server.
The ASP.NET version runs on Windows and the PHP version runs on Unix systems.

Features
--------

- Reads Zip, RAR and PDF files
- Multi-platform (both server and client)
- Resize each page to fit the browser
- Password protect a collection of your manga
- Client app support touch devices (iOS, Android, Windows 8)
- Load pages only before you are going to read it
- Multi-language client app and easy to add new language
- Easy customization by editing template HTML files and CSS files


Demo
----

- [ASP.NET Version hosted on Windows Azure Virtual Machine](http://demoaspnet.mangaweb.info)
- [PHP Version hosted on DreamHost](http://demophp.mangaweb.info)
- [PHP Version on my Raspberry Pi](http://demopi.mangaweb.info:9001) (Uptime is not guaranteed.)


Server Requirement (ASP.NET)
----------------------------

- IIS (Tested with 7 and 8)
- ASP.NET 4.0 or 4.5
- MySQL Server (Tested with 5.5)
- (Optional) 7-zip
- (Optional) Microsoft Visual C++ 2010 Redistributable Package


Server Requirement (PHP)
------------------------

- A server to run php (Tested with Apache 2.0)
- PHP 5.1.0 or above (Tested with 5.3)
- MySQL Server (Tested with 5.5)
- MySQL extension in PHP
- GD extension in PHP
- (Optional) Zip extension in PHP
- (Optional) RAR extension in PHP
- (Optional) pdfinfo and pdfdraw

Browser Support
---------------

- Internet Explorer 9+
- Firefox 10+
- Chrome 12+
- Safari 4+
- Safari in iOS 3.2+
- Chrome for Android
- Firefox for Android
- Kindle browser (Tested on Kindle 4 NT only)


Quick Start
===========

Compiling the source
--------------------

You need the following tools
- Visual Studio
- [Script#](http://scriptsharp.com/) (7.4.0 or above)
- [Haxe compiler](http://haxe.org/download)
- [LESS](http://lesscss.org/)

To compile:
- Open `source/afung.MangaWeb3.sln` in Visual Studio and compile
- Use `haxe -cp . -php source/haxephp -main afung.mangaWeb3.server.ServerAjax` to compile the Haxe source to PHP
- Use a LESS compiler to compile `source\less\mangaweb.less` and copy `mangaweb.css` to `source\aspnetserver\css`
- run `gather_aspnet.ps1` in Windows PowerShell and files under `output\aspnet` are ready to deploy to a Windows server
- run `gather_php.ps1` in Windows PowerShell and files under `output\php` are ready to deploy to a Unix server
- (Optional) replace `output\aspnet\bin\PDFLibNet.dll` with `external_libraries\x86\PDFLibNet.dll` if you are going to deploy it to a 32-bit Windows machine

Pre-compiled binaries and scripts
---------------------------------

Pre-compiled packages can be downloaded [here](https://github.com/a-fung/MangaWeb3/downloads)


Deploy to a Windows server
--------------------------

- Create an empty database in MySQL first
- Copy all the files to your server
- Make a new application or a new website pointing to where you placed to files
- Give full control permission to `empty.pdf` file and `cover` and `mangacache` directories to your IIS Application Pool
- Open `install.html` from your browser and specify MySQL server information, location of 7z.dll and information to create administrator
- (Optional) Delete `install.html`, `bin\afung.MangaWeb3.Server.Install.dll`, `js\afung.MangaWeb3.Client.Install.js`, `template\install.html` from your server
- (Optional) Copy ASP.NET Version of CKFinder to `ckfinder` directory (do not replace `config.js` and `config.ascx`) and copy `CKFinder.dll` to `bin` directory
- Now your server is ready to add manga and to be used. `admin.html` is the administration page. `index.html` is the client app.

Deploy to a Unix server
-----------------------

- Create an empty database in MySQL first
- Copy all the files to your server
- Give write permission to `temp`, `cover` and `mangacache` directories
- Open `install.html` from your browser and specify MySQL server information and information to create administrator
- (Optional) Delete `install.html`, `lib\afung\mangaWeb3\server\install`, `js\afung.MangaWeb3.Client.Install.js`, `template\install.html` from your server
- (Optional) Copy PHP Version of CKFinder to `ckfinder` directory (do not replace `config.js` and `config.php`)
- Now your server is ready to add manga and to be used. `admin.html` is the administration page. `index.html` is the client app.

----------------------------------------------------------------------------------------------

Develop & Test Enviroment
=========================

- Windows 7
  - Visual Studio 2010 Ultimate
  - Script# 7.4.0
  - Haxe
  - FlashDevelop 4
  - SimpLESS 1.4
  - IIS 7
  - ASP.NET 4.0
  - MySQL 5.5
  - Internet Explorer 9
  - Firefox (Latest)
  - Chrome (Latest)
- Debian 6
  - Apache 2.0
  - PHP 5.3
  - MySQL 5.5
- Raspbian “wheezy” (2012-09-18)
  - Apache 2.0
  - PHP 5.3
  - MySQL 5.5
