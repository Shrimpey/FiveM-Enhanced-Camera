# Enhanced Camera

## Overview of the menu
This menu recreates vehicle camera from scratch and introduces a number of customizable parameters for user to tune. On top of that there is a drone camera with a few modes to simulate drone flight.

Main features of this menu:
- **Lead camera** - main camera, rotates around car dependant on angular velocity of the vehicle
- **Chase camera** - focuses on closest vehicle and points towards it
- **Drone camera** - simulates drone physics, allows choice between different modes (race, zero-G, spectator, homing)
- **Customizable parameters** - FOV, XYZ position offsets, interpolation values and more
- **Saving/loading** - save and load camera parameters or choose from default presets

## Installation

Go to [releases](https://github.com/Shrimpey/EnhancedCamera/releases) and download latest zipped release. Unzip and place ``enhancedcamera`` folder inside your server's ``resources`` folder, then edit your server.cfg to include line ``start enhancedcamera``.

## Parameters overview

It's best to just experiment with parameters to see how they affect camera handling (for lead/chase camera you can start by spawning a preset from submenu).
#### Lead and chase camera parameters:
- **Lock position offset** - determines whether camera changes position compared to vehicle (or just rotates)
- **Linear position offset** - experimental feature, camera changes position along the line drawn behind vehicle instead of doing circular motion around the car
- **Lock rotation to camera plane** - changes the way that camera rotates around car (mostly visible on uneven ground)
- **Modifier** - this modifier * angular velocity = target rotation. Higher values make camera move further from lock. (-1,1)
- **Yaw interpolation** - lower values - smoother movement along yaw axis (0,1)
- **Roll interpolation** - lower values - smoother movement along roll axis (0,1)
- **Pitch interpolation** - lower values - smoother movement along pitch axis (0,1)
- **Camera offset** - offsets chase camera target towards its velocity vector. (0,5)
- **Position interpolation** - lower values - smoother movement, higher delay. (0,1)
- **FOV** - changes field of view, may affect performance with higher values (20,120)
- **X/Y/Z Offset** - static position offset in XYZ direction
- **Max angle to lock** - for chase camera only, max angle from velocity vector to keep the lock on, if angle exceeds this limit, camera switches back to normal. (25,360)

## Examples

## Credits
- [Tom Grobbe](https://github.com/TomGrobbe) - [MenuAPI](https://github.com/TomGrobbe/MenuAPI) used for GUI, code snippets from [vMenu](https://github.com/TomGrobbe/vMenu) used for saving/loading camera parameters
- [QuadrupleTurbo](https://github.com/QuadrupleTurbo) - providing new ideas for features, massive help with playtesting
- [No Name Drift](https://nonamedrift.com/) and [Velocity](http://www.velocitydrift.com/) servers - playtesting and feedback
