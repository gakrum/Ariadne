# Ariadne
Maze solver

To run Ariadne maze solver, open the Ariadne.sln file in Visual Studio. Press F5 to run the web API project with IIS Express.

Use Fiddler or PostMan to POST the following (or similar maze string) to the following route: http://localhost/api/solveMaze

{ "mazeString" : "########## #A...#...# #.#.##.#.# #.#.##.#.# #.#....#B# #.#.##.#.# #....#...# ##########" }

The 'solveMaze' web service should return the following output for the above request:

{ "steps" : 14, "solution" : "########## #A@@.#...# #.#@##.#.# #.#@##.#.# #.#@@@@#B# #.#.##@#@# #....#@@@# ##########" }
