# Pixel Planet Prefabs
![Alt text](https://i.imgur.com/2TttBpM.png "Pixel Planet Prefabs")


## Overview
This is Unity project based off of the [Pixel Planet Generator][1] project by [Deep-Fold][2]. Instead of exporting spiresheets, planets are set with prefabs so they can be loaded in a scene dynamically.

I am no Unity expert so please open up issues (or make pull requests!). Available under MIT license. Credit is appreciated, but not required.


## How to Use
Planets can be controlled by dropping the prefab in your scene (`Assets/Planets/[Planet]/[Planet].prefab`), selecting the parent prefab, and tweaking the script properties in the inspector.

![Alt text](https://media1.giphy.com/media/TjdsIT8trKLT2SUiuu/giphy.gif?cid=790b761188f77322c709eceb94500121fc904285a2ef585f&rid=giphy.gif&ct=g "Pixel Planet Prefabs")

### Generic Properties
| Property     | Description                                                |
|--------------|------------------------------------------------------------|
| Size         | Adjusts the transform's scale                              |
| Rotation     | Adjusts the transform's rotation (planet tilt)             |
| Speed        | Adjusts the animation speed of the planet's rotation       |
| Color        | Changes the planet layer colors                            |
| Seed         | Changes the planet layer noise pattern via seed number     |
| Light Origin | 2D point where light is casting on the planet from         |
| Pixels       | Adjust the pixels per unit (0 for no pixelation)           |

### Planet-specific Properties
| Property        | Planet                            | Description                        |
|-----------------|-----------------------------------|------------------------------------|
| Cloud Cover     | Continents, Gas Giant, Icelands   | Amount of clouds convering surface |
| Water Flow      | Continents, Icelands              | Amount of water convering surface  |
| Water Flow      | Volcanoes                         | Amount of lava convering surface   |
| Ring Enabled    | Gas Giant Ringed                  | Show/hide ring layer               |
| Craters Enabled | Volcanoes                         | Show/hide craters layer            |


## Credits
* [Pixel Planets by Deep-Fold][1] - original project
* [UniPixelPlanet by hmcGit][3] - Unity ported project

[1]: https://deep-fold.itch.io/pixel-planet-generator
[2]: https://deep-fold.itch.io/
[3]: https://github.com/hmcGit/UniPixelPlanet
