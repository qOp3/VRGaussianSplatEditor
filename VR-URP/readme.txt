### Lab3

In this lab project, I created an interaction mechanism to observe large 3D models (e.g., 3D-scanned environments) in Unity.

When dealing with large models, especially scanned environments, if the user is inside the model (within its collider), it's difficult to move and rotate the model. Moving the entire environment can also cause motion sickness.

Instead of moving the model, I implemented a 6DOF (6 Degrees of Freedom) control system for the camera, this is similar to Creative Mode in Minecraft. By using a single controller to smoothly move the camera and turning their head to observe the surroundings, the user can minimize motion sickness during navigation.

The user can press the trigger and move the controller; the displacement of the controller determines the direction and speed of the camera's movement.
The movement can be controlled by both left and right controller.

When the triggers on both controllers are pressed at the same time, the user enters scaling mode, where the distance between the controllers determines the scaling of the model. While in scaling mode, the user can also rotate the model by using the left thumbstick.