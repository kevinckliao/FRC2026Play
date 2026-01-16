# Extensions

Acknowledging the limitations of the Builder system, the Beta implements an `Extensions System` to increase the possible complexity

Currently, the Extensions System contains the following:
* `Auto Align`
* `Spawn Piece Target`
* `Attach At Startup`

### Auto Align
 * This script should be added to the same object as build frame, and has settings to align, it is NOT season specific and as such uses a simple set of options

### Spawn Piece Target
 * Can be added to any object and simply serves as a reference point for game piece spawning

### Attach at Startup
* Attach at startup is used for advanced Physics implementations. To minimize failures, builder generates all physics elements at runtime, so this script is required to attach custom rigidbody elements to a robot

### Future Extensions
* `Aim at Point` - Rotates the drivetrain to face the target point
* `Point at Point` - Points an arm at the target point
* `SymmetryPlane` - Creates a symmetric copy of specific children


# [Further Reading](FurtherReading.md)
