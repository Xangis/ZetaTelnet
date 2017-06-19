ZetaTelnet
==========

A basic telnet application for Windows originally released at http://zetacentauri.com
and downloaded more than 60,000 times before being open sourced.

![ZetaTelnet Screenshot](https://github.com/Xangis/ZetaTelnet/blob/master/images/zetatelnet3.gif)

A prebuilt installer is available in the installer directory here:

https://github.com/Xangis/ZetaTelnet/blob/master/Installer/ZetaTelnet3.01Setup.exe

This application was written using C# .NET 2.0 and should run on any system that
supports .NET 2.0.  A project is included for Visual Studio 11. It doesn't have any
complicated dependencies, so you can create a project for an earlier version simply
by including the source files.

This program has only very basic telnet support and doesn't support secure sockets
(SSH) or any terminal protocols such as VT100 or VT52. I'd be happy to accept any
pull requests if you'd like to add them.

Installer scripts for InnoSetup and NSIS are included in the installer directory, but 
you'll probably have to modify the paths in order for them to work because they
assume things about development environment that is specific to me.
