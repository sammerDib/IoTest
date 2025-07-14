# Reliance axis controller

## Overview

This controller can move the stage through 2 axes:
- X for left/right motion
- Y for back/front motion

Each axis can be represented like that:
!                                       ! <-- optical bound
|---------------O----------------------------|
^-- moving part              ^-- hardware bound

There are 4 bounds (2 optical + 2 hard), where optical bounds are triggered before hard ones.
Optical bounds are very close to hard ones (around 1mm).

When moving part pass through an optical bound, motor stopped is triggered, and controller sends `error : CW Limit!!` or `error : CCW Limit!!` (in addition to requested information).
`CW` means clockwise and `CCW` means counterclockwise.

WARNING: do not update motor origin (with command `|2.1` for motor #1). This would break `FPMS`. The origin should only be indirectly set with search origin procedure (command `|.1` for motor #1).

Motions distance is given in pulse unit. One motor rotation represents many pulses (depends on the resolution), and one motor rotation shift the moving part along the axis, depending on the motor rotation distance (mm/rotation):

`[distance] = [pulses] / [resolution] * [motor rotation distance]`

In addition, absolute coordinates depends on the referential:

- hardware referential: used by the hardware controller, where the origin (0), and the orientation can be redefined during origin search routine (`|.[motor_id]`).
- motor referential: used in all methods exposed by all software controllers, where the origin is in the middle of the axes, and positive coordinates for X and Y are respectively, left and front.

Conversions are done trough a translation vector.

`[coordinate in motor referential] = [coordinate in hardware referential] + [translation vector value]`

TODO: explain search origin procedure

search origin start:
<------------------------------->
------------O-------------------------------O------------


search origin running:
<------------------------------->
----O-------------------------------O--------------------

# Glossary

- Pulse: step of the motor
- Resolution: number of pulses per motor revolution
- Motor rotation distance: linear distance travelled by the moving part during one motor revolution.

# How to communicate with the controller?

- Instruction are send with strings
- Each instructions should end with `\r\n`.

## List of commands

- TODO: Link to source code
- `P.[motor_id]=[position]`: prepare next direct motion with position [position] (in pulses) for motor with id [motor_id]
- `S.[motor_id]=[speed]`: prepare next direct motion with speed [speed] (in pulses/s) for motor with id [motor_id]
- `A.[motor_id]=[acceleration]`: prepare next direct motion with acceleration [acceleration] (in pulses/s-2) for motor with id [motor_id] during start move
- `^.[motor_id]`: execute last prepared direct motion for motor with id [motor_id]
- `[parameter].[motor_id]=[value]`: set parameter [parameter] for motor with id [motor_id] with value [value]
    * `K37`: motor resolution and speed factor (see MotorResolution for more details)
    * `K42`: (search origin) speed. Should be as lowest as possible (around `100`) to ensure good precision during search origin procedure (and avoid to hit hardware bound, which may set the motor in security state. The only workaround is an electrical reboot, then)
    * `K43`: (search origin) acceleration
    * `K45`: (search origin) direction (`1` for counter clock wise). Electrical bound which is at very bottom of the tool is not easily accessible by operator, so we should not use search origin procedure in this direction. That's why `K45` should always be `1`.
    * `K46`: (search origin) method (`2` for optical sensor bound detection)
    * `K47`: (search origin) stopper torque (not used with `K46=2`)
    * `K48`: (search origin) offset distance (`0` stop before optical bound, in pulses)
- `|.[motor_id]`: execute origin search routine for motor with id [motor_id]
- `(.[motor_id]`: enable motor with id [motor_id]

## Queries and responses

- queries start with `?`
- TODO: Link to source code
- `?96.[motor_id]`: current position of motor with id [motor_id]
  -> expected response: `Px.[motor_id]=123`: current position is 123 pulses
- `?99.[motor_id]`: current status of motor with id [motor_id]
  -> expected response: `Ux.[motor_id]=8`: current status is "in position" (`0` means "in motion")

# Troubleshooting

ℹ Resolutions should be done with `Control Room` software provided by Myostat constructor (usually already installed on the PC).

Symptom: `?99.1` (motor status) returns `Ux.1=4` (over torque error)
Details: Motor cannot move anymore
Resolution: `(.1` (enable motor #1)

Symptom: `?99.1` always returns `Ux.1=2` (overspeed)
Details: ???
Resolution: emergency stop with `*` (will stop all motors), followed by `*1` to reset the controller.

Symptom: `?99.1` or `?96.1` (motor position) returns nothing
Details: motor #1 is in security state
Resolution: electrical reboot of the controller (for TMAP, CUB reboot)

Symptom: `?99.1` always returns `Ux.1=0` (in motion)
Details: can occured sometimes during search origin procedure
Resolution: stop motor with `*` (will stop all motors), followed by `*1` to reset the controller.

# Warning

`|4` command seems buggy (when sent, motor 2 does not respond anymore). Use at your own risks!

# Annexes

[Official documentation](https://docs.myostat.ca/display/MYOS/CM1-C+User+Guide)