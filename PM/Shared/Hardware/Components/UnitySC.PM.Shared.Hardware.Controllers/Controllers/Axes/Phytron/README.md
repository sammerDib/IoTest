# Some commands

- `XP20R`: get the position (in pulses) on axis X
- `XA-50000`: go to -50_000 pulses on axis X
- `XP14R`: get parameter P14 on axis X
- `XP14S4000`: set parameter P14 on axis X to 4000
- `XP15R`: read acceleration

# Consistent values between speed and acceleration
- min speed: 1 pulse/s
- min acceleleration: 1 pulse/s²

- max speed: 40_000 pulse/s
- max acceleleration: 500_000 pulse/s²


|        | speed  | acceleration |
|--------|--------|--------------|
| slow   | 10_000 | 200_000      |
| normal | 20_000 | 300_000      |
| fast   | 40_000 | 500_000      |