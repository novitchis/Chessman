# Chessman
A chess analysis Universal Windows Application published to Windows 10 Store since 2016.

[![Join the chat at https://gitter.im/ChessmanDev/Lobby](https://badges.gitter.im/ChessmanDev/Lobby.svg)](https://gitter.im/ChessmanDev/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![Build status](https://ci.appveyor.com/api/projects/status/78agh8di6142pjs1?svg=true)](https://ci.appveyor.com/project/novitchis/chessman)

<img src="https://raw.githubusercontent.com/novitchis/Chessman/master/public/Desktop_Analysis.PNG" alt="Chessman" />

With the purpose of helping chess players of any level to improve their game, Chessman is a modern Windows 10 application hoping to evolve into a full featured __chess training app__.

*Please don't submit the exact copy of this app to Windows 10 Store, instead you can contribute to this application see CONTRIBUTING.md.*

Implementation
--------------
The application is a Universal Windows Platform that runs on any Windows 10 device. Is desktop first, but other screen sizes will be still supported. 

__Stack:__
- View: XAML and C# and follows (mostly :) ) MVVM.
- C++/CX core chess board backend.
- C++ [Stockfish 7](https://github.com/official-stockfish/Stockfish) chess engine.
- Unit Tests: XUnit.

License
-------
Chessman is free, and distributed under the GNU General Public License (GPL) 3.0. Essentially, this means that you are free to do almost exactly what you want with the program, including distributing it among your friends, making it available for download from your web site, selling it (either by itself or as part of some bigger software package), or using it as the starting point for a software project of your own.

The only real limitation is that whenever you distribute Chessman in some way, you must always include the full source code, or a pointer to where the source code can be found. If you make any changes to the source code, these changes must also be made available under the GPL.

For more details, read [COPYING.md](COPYING.md).
