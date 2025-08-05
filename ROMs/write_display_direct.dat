.org $1FFF

program:
MOV $0000, V0
MUL 32, 64 ; Set AX to 32 * 64
MOV AX, V1 ; Copy AX to V1
MOV 1, V2 ; Color value 1

reset:
MOV 0, V0
INC V2 ; Increment color value

draw:
GT V0, V1 ; V0 > V1?
BZ        ; If number exceeds, branch to reset
JMP reset

MOVB V2, [V0+$402F] ; Set pixel at V0 to color 6
INC V0 ; Increment V0

JMP draw ; Repeat drawing