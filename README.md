Introduction
------------

This toolbar provides a launcher for functionality within TortoiseGit.

Installing the lightweight toolbar will allow access to these common source control functions from anywhere in Visual Studio, allowing you to quickly hit a shortcut or click a button to launch Commit, Push, Pull, Log, Resolve or Bash windows.

The toolbar is provided for free and will be soon updated with additional customisable buttons, as well as icons. Further integration with Git is not planned - for more complex usage scenarios you should take a look at the [Visual Studio Tools for Git](http://visualstudiogallery.msdn.microsoft.com/abafc7d6-dcaa-40f4-8a5e-d6724bdb980c).

Installation instructions
--------------------------

1.  Install the [VISX File](http://visualstudiogallery.msdn.microsoft.com/6a2ae0fa-bd4e-4712-9170-abe92c63c05c)
2.  You may need to manually enable the toolbar: Views -> Toolbars -> TortoiseGit 

Default shortcuts
------------------

* (G)it (C)ommit - CTRL+G, C or CTRL+G, CTRL+C
* (G)it (R)esolve - CTRL+G, R or CTRL+G, CTRL+R
* (G)it (P)ull - CTRL+G, P or CTRL+G, CTRL+P
* (G)it P(u)sh - CTRL+G, U or CTRL+G, CTRL+U
* (G)it (L)og - CTRL+G, L or CTRL+G, CTRL+L
* (G)it (B)ash - CTRL+G, B or CTRL+G, CTRL+B

Note that these default shortcuts will override the default usage of CTRL+G (Go To Line). If you don't want to remap that shortcut to something else, you can easily reset Go To Line to CTRL+G and set different shortcuts for the TortoiseGit commands (Tools -> Options -> Keyboard and show commands containing TortoiseGit).