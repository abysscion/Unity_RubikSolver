# Unity Rubik's Cube solver
Unity visualization for a Rubik's solver project.

![Example](/Images/example.gif)

## Description
This project is about making a program able to solve any Rubik's Cube mix in a fractures of time. It uses [Layer by layer](https://en.wikipedia.org/wiki/Layer_by_Layer) method (also known as Beginners method) containing 5 steps to solve Rubik's Cube.

## Usage
Program uses FRUBLD world-wide notation, familiarize yourself if needed: [link](https://ruwix.com/the-rubiks-cube/notation/)
Enter sequence in left lower input field. If sequence is valid (for example: "D2 U' U' R L") field will be colored in green and button "Launch" will be active. After clicking on launch button, sequence will be executed. Now, clicking on "Solve" button will show solution for this mix in left upper corner. You able to copy and paste this solution in input field to launch it and check if solution is correct :)
Alternatively you able to simply press shuffle button to apply random rotations and then solve mixed cube.

![Example1](/Images/example1.gif)

## Features

- Rotate cube as you want with your right mouse button to get desired point of view!
- Launch: put some rotation commands in input field and lauch them on cube!
- Shuffle: mix cube with random rotations!
- Solve: get instant solution to the given mix!
- Reset: return cube sides at solved state!
- Stop: tired to see how cube is doing rotations? Stop it immidiately!
