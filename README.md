# LampSim


Simulation I created to supplement my Senior Design 2 group's final project. Our final project was an Animatronic Lamp, similar to the Pixar Lamp. The lamp was controlled by voice command via Alexa and one of its functions was to track an object of interest using 4 axis of movement.

This project is the simulation that was used to train the lamp to move on its 4 degrees of freedom. The training was accomplished using Imitation Learning and [Unity's ML-Agents](https://github.com/Unity-Technologies/ml-agents).

Running the simulation is a bit of a process that is described here in the [ml-agent documentation](https://github.com/Unity-Technologies/ml-agents/tree/master/docs).

**NOTE: The process to run the simulation is likely to change quite often as ml-agents is an SDK still in development.**

**There are many parts to this simulation, but the main "training" component can be found [here](https://github.com/TimothyA86/LampSim/blob/master/Assets/Scripts/LampAgent.cs)**

### Input and output of the trained model
The lamp model is fed an input containing an x and y offset with in its camera's view and its current servo positions.

*input = (s0, s1, s2, s3, x, y)*

The lamp model then outputs what its final servo positions should be to zero the x and y offset.

*output = (s0, s1, s2, s3)*

**All input and output components are normalized between -1 and 1**

## Languages
* C#

## SDKs
* [ml-agents](https://github.com/Unity-Technologies/ml-agents)