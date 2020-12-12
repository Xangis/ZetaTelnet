TODO:

- Add a view/window => Clear menu option to clear the window and all scrollback.
- Give some indication whether a session is connected or disconnected.

Changes in Version 3.02:

1. Updated from .NET 2.0 to .NET 4.5.
2. Made the app DPI-aware.

Changes in Version 3.01:

1. Created installer/uninstaller.
2. Minor adjustments to interface.

Changes in Version 3:

1. Default background is now black, but color can be changed under the Options menu.
2. Up and down arrows now let you select previously-sent text to send.
3. Window can now be resized effectively.
4. Better connection error handling.

Changes in Version 2:

1. Added disconnect button.
2. Added menu.
3. Added setting to strip ANSI color codes from received text under Options->Settings.
4. Added save option to save output window text as a Rich Text (.rtf) file.

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
