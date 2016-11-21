# Constellation
A 2D real-time strategy game that rewards long-term planning more than quick reflexes.

![Gameplay image](http://imgur.com/a/yvr14)

###Objective
Conquering the galaxy by colonizing every star.

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

####Sending armies
Any player can send armies from a given star using any road connected to that star. Note that this means a road connecting two stars allows armies of both stars to be sent to each other. Once an army is on a road, it cannot change directions. Two armies of different players heading in opposite directions on a road will battle when they meet.

####Battling
The outcome of a battle is realistically determined via Lancaster's Law. Essentially, this means that larger armies will suffer much less causualties than smaller armies.

####Star colonization
Additionally, roads built from an occupied star to an unoccupied star will colonize the unoccupied star. Stars can also be occupied by sending an army to that star via a road. The army will battle the hostile armies on the star, and the winner controls the star.

###Supported gamplay modes
* local multiplayer
* single player vs AI.

###Notes
Internet multiplayer via UDP sockets is in progress!
