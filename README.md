Introduction
------------

This toolbar provides a launcher for functionality within TortoiseGit.

Installing the lightweight toolbar will allow access to these common source control functions from anywhere in Visual Studio, allowing you to quickly hit a shortcut or click a button to launch Commit, Push, Pull, Log, Resolve or Bash windows.

This toolbar simply launches TortoiseGit commands. Further integration with Git is not planned - for more complex usage scenarios you should take a look at the [Visual Studio Tools for Git](http://visualstudiogallery.msdn.microsoft.com/abafc7d6-dcaa-40f4-8a5e-d6724bdb980c).

Installation instructions
--------------------------

1.  Install the [VISX File](http://visualstudiogallery.msdn.microsoft.com/6a2ae0fa-bd4e-4712-9170-abe92c63c05c)
2.  You may need to manually enable the toolbar in Visual Studio: Views -> Toolbars -> TortoiseGit 

Default shortcuts
------------------

You can opt into these default shortcuts by removing the default shortcut for CTRL+G (Go To Line).

To customise these shortcuts open up Tools -> Options -> Keyboard and show commands containing TortoiseGit.

Solution level shortcuts:

* `(G)it (B)ash` - CTRL+G, B
* `(G)it Clea(n)up` - CTRL+G, N
* `(G)it (C)ommit` - CTRL+G, C
* `(G)it (F)etch` - CTRL+G, F
* `(G)it (L)og` - CTRL+G, L
* `(G)it (M)erge` - CTRL+G, M
* `(G)it (P)ull` - CTRL+G, P
* `(G)it P(u)sh` - CTRL+G, U
* `(G)it (R)ebase` - CTRL+G, R
* `(G)it (R)ebase-Con(t)inue` - CTRL+G, T
* `(G)it R(e)solve` - CTRL+G, E
* `(G)it Re(v)ert` - CTRL+G, V
* `(G)it Stash-P(o)p` - CTRL+G, O
* `(G)it Stash-S(a)ve` - CTRL+G, A
* `(G)it (S)witch` - CTRL+G, S
* `(G)it S(y)nc` - CTRL+S, Y

Currently opened file shortcuts:

* `(G)it File (L)og` - CTRL+SHIFT+ALT+G, L
* `(G)it File (B)lame` - CTRL+SHIFT+ALT+G, B
* `(G)it File (D)iff` - CTRL+SHIFT+ALT+G, D

Customisation
--------------

By clicking the dropdown arrow next to the toolbar and selecting `Add/Remove Buttons`, you can check/uncheck different commands to create your own customised list. For example, if you rarely use the TortoiseGit rebase UI, you can easily hide it from the toolbar.