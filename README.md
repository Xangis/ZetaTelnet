ZetaTelnet
==========

A basic telnet application for Windows.

A compiled version of this program with installer is available here:

http://zetacentauri.com/software_zetatelnet.htm

This application was written using C# .NET 2.0 and should run on any system that
supports .NET 2.0.  A project is included for Visual Studio 11. It doesn't have any
complicated dependencies, so you can create a project for an earlier version simply
by including the source files.

This program has only very basic telnet support and doesn't support secure sockets
(SSH) or any terminal protocols such as VT100 or VT52. I'd be happy to accept any
pull requests if you'd like to add them.

Installer scripts for InnoSetup and NSIS are included, but you'll probably have to
modify the paths in order for them to work.
