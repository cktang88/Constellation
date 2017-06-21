# Constellation
A 2D real-time strategy game that rewards long-term planning more than quick reflexes. A good strategy hinges on building an efficient road network, and knowing when to expand and when to reinforce.

To play, run ```/Constellation/bin/Debug/Constellation.exe```. Have fun!

![Gameplay image](http://i.imgur.com/qW8UbAZ.png)

### Objective
Conquer the entire map by colonizing every star.

### Features
* Multiple maps
* Challenging AI for single-player mode
* Future goals: networking for multiplayer

## Controls
* **Z** - Build a new road
* **C** - Upgrade a road
* **X** - Destroy a road
* All units and road building happens via mouse controls.

## Rules

#### Army growth
Each time tick, one army unit is automatically produced at each colonized star.

#### Roads
Roads are the only way to move armies between stars. Roads can only be constructed from a star you currently occupy, though it can end at any other star. Note that roads can be used by any player given they own a star at either endpoint. Building a new road costs a certain number of armies, which is deducted from the star from which the road originates.

#### Upgrading a road
Roads can be upgraded up to three times. Each upgrade allows armies to move faster across that road. However, each upgrade has a cost, which is deducted from the star from which the road originates.

<img src="http://i.imgur.com/XJBHvDl.png" width="200" padding = "30">

#### Sending armies
Armies can be sent from a star using any road connected to that star. Thus, two connected stars can send armies to each other. Armies on roads cannot change directions. Two hostile armies heading in opposite directions on a road will automatically battle when meeting.

<img src="http://i.imgur.com/CXAN0F4.png" width="200" padding="30">

#### Battling
Battle outcomes are realistically determined by [Lancaster's Law](https://en.wikipedia.org/wiki/Lanchester%27s_laws). This means that larger armies will suffer much fewer losses than smaller armies.

#### Colonizing stars
**Neutral stars** - Roads built from an occupied star to an unoccupied(neutral) star will *colonize* the unoccupied star.

**Enemy stars** - Enemy-controlled stars can be colonized by sending an invading army, building a new road as needed. The army will battle the hostile armies on the star, and the winner controls the star.
