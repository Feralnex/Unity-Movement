# Movement
Movement asset shows how to handle an input to control a character's movement.

## About
Asset uses input from a Virtual Joystick to move the character "Player" without root motion and additionally handles motion animations.

Prefab Player represents a potential character object in the game and consists of the components Animator, Rigidbody, Capsule Collider, Movement, Movement Controller and GroundDetector.

The Movement component is responsible for the character's movement logic (position and rotation) and for updating the values in the Animator. It provides two parameters:
- Speed Curve - curve defining the position change multiplier,
- Rotation Curve - curve defining the rotation change multiplier.

The Movement Controller component is responsible for reading the input data from the Virtual Joystick and passing it to the Movement component. It provides several parameters:
- VirtualJoystick - reference to the VirtualJoystick component,
- JumpButton - a button that will initiate a jump,
- Speed Multiplayer - a float value that determines an additional multiplier for changing positions,
- Rotation Multiplayer - a float value that determines an additional multiplier of the rotation change,
- Jump Force - float value specifying the force of the jump in the y axis direction,
- Animation Speed - a float value that determines the speed of motion animation playback.

The GroundDetector component is responsible for detecting whether an object is standing on a given surface, or whether it is in the air or in the water (i.e. in a fluid).

Feel free to explore the asset to analyze the "Player" animator controller and the rest of the additional scripts.

Unity version:
- 2021.2.7f1.

Assets used:
- [Virtual Joystick](https://github.com/Feralnex/Unity-Virtual-Joystick),
- [Mixamo character and animations](https://www.mixamo.com).

## Usage
1. Place the Virtual Joystick prefab in Canvas and set it as you see fit or use a different input source after modifying the MovementController component first. Additionally, place a Button in Canvas that will act as a jumping button.
2. Add a prefab Player to the scene and add references to the VirtualJoystick component and a Button (jump button) to it.

An "Example" scene is prepared, which shows how an asset can be used.