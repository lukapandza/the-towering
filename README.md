# the-towering

<img src="https://github.com/lukapandza/the-towering/blob/main/showcase.gif"></img>

This project was made during a winter independent study to explore some areas of video game programming that I find particularly interesting.

* The hexagonal grid map is stored as a weighted graph, allowing for the implementation of pathfinding with an arbitrary number of agents using Dijkstra's algorithm
* The visual assets are all hand drawn and cloned by randomly selecting from a set when the game objects are instantiated
* The targeting system relies on the moving agent to register themselves with all defensive towers in range when they cross into a new tile
* The towers hold their targets in a queue, staying with the target that enterd the range first until it is killed or leaves the range
