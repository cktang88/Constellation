# Constellation
A 2D real-time strategy game that rewards long-term planning more than quick reflexes. A good strategy hinges on building an efficient road network, and knowing when to expand and when to reinforce.

![Gameplay image](http://i.imgur.com/qW8UbAZ.png)

###Objective
Conquering the galaxy by colonizing every star.

###Features
* Multiple maps
* Challenging AI for single-player mode
* Vector graphics

###Try it out!
Download the project, then run the executable at "Constellation/bin/Debug/Constellation.exe". Have fun!

##Controls
* **Z** - Build a new road
* **C** - Upgrade a road
* **X** - Destroy a road

##Rules

####Army growth
One army unit is automatically produced at each star that is occupied each time tick, for the owner of that star.

####Roads
Roads are the only way to connect stars. Roads can only be constructed from a planet you currently control, though it can end at any other star regardless of affiliation. Note that roads can be used by any player given they own the star at either endpoint. Building each new road costs a certain number of armies, which is deducted from the star from which the road originates.

####Upgrading a road
Roads can be upgraded up to three times. Each upgrade allows armies to move faster across that road. However, each upgrade has a cost, which is deducted from the star from which the road originates.

<img src="http://i.imgur.com/XJBHvDl.png" width="200" padding = "30">

####Sending armies
Any player can send armies from a given star using any road connected to that star. Note that this means a road connecting two stars allows armies of both stars to be sent to each other. Once an army is on a road, it cannot change directions. Two armies of different players heading in opposite directions on a road will battle when they meet.

<img src="http://i.imgur.com/CXAN0F4.png" width="200" padding="30">

####Battling
The outcome of a battle is realistically determined via Lancaster's Law. Essentially, this means that larger armies will suffer much less causualties than smaller armies.

####Star colonization
Additionally, roads built from an occupied star to an unoccupied star will colonize the unoccupied star. Stars can also be occupied by sending an army to that star via a road. The army will battle the hostile armies on the star, and the winner controls the star.

###Notes
Internet multiplayer via UDP sockets is in progress!
