# Custom Effector Shapes
When you are rigging controls for a character in a 3D program like Blender, sometimes you create shape "gizmos" for each control.

In Unity's Animation Rigging package, these are called Effectors. While Unity offers their own pre-made Effector shapes, they are limited to primitives.
In more complex rigs, sometimes you need a shape that isn't primitive, so an animator can more easily determine what function a control is supposed to be used for.

Control Rigging bridges this gap by offering an importer for custom `.lineobj` files.
This importer was created in a [repository by taylorgoolsby](https://github.com/taylorgoolsby/lineobj-importer) and is under the MIT license.

## How to export a .lineobj file from Blender?
1. Export an OBJ file with the following settings:
    ```
    Forward Axis: -Z
    Up Axis: Y
    Triangulated Mesh: Enable
    ```
2. Export the file as `.obj` and rename it to a `.lineobj` file extension.
3. You are now able to import it into Unity.