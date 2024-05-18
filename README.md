# Procedural Generation

Some implementations of procedural generation algorithms : Marching squares, Perlin worm, Chunk divided world generation, etc.
More features to come : modularity of terrain generation, terrain painting, world division into chunks, vegetation and poi generation, maybe biomes ? 

# Marching squares :

This is a preety straighforward implementation of the algorithm.
The world is divided into tiles. On each tile a 3D model is instanciated based on the result of the marched tile. These models have been created in Blender and separated in 3 parts : bottom, middle and top. This way we can control the height of the terrain just by modifying the y scale of the middle part of the tile.

# River generation

River generation based on perlin worms :

![image](https://github.com/Milhouzer/proceduralgeneration/assets/65489537/15013ea8-003e-4761-8778-013854ade466)

This river has been generated using two sets of parameters, held by scriptable objects :
* River noise settings which are FastNoise settings (see https://github.com/Syomus/ProceduralToolkit/tree/master/Runtime/FastNoiseLib)
* Perlin worm settings which controls the worm settings : length, convergeance, creation speed (for demo purposes)

The generation is not always perfect as it can be shown on the picture : the river sometimes crosses itself, for the moment it is not a big deal, we can imagine lake being created on these intersection points.

![image](https://github.com/Milhouzer/proceduralgeneration/assets/65489537/8a0e0f7c-1635-4ddc-a8c5-b1dc524ca228)

