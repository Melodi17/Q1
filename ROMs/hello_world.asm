.org $1FFF
JMP program

font:
    .include "font.bin"

program:
    MOV 0, V0 ; Load character #12
    MOV 0, V1 ; X position
    MOV 0, V2 ; Y position
    MOV 1, V3 ; Color (1 = white)
    CALL write_char ; Call function to write character
    SUS

write_char:
    ; V0: Character to write
    ; V1: X position
    ; V2: Y position
    ; V3: Color
    ; Write a character to the screen at (V1, V2) with color V3
    
    MUL V0, 64 ; V0 = Character * 64 (character size)
    MOV AX, V0 ; Store result back into character
    
    MUL V2, 64
    ADD AX, V1
    MOV AX, V4 ; V4 = X position + Y position * 64 (screen index)
    
    MOV V4, [$EFFF] ; Debug
    
    MOV [V0 + $2003], V5 ; Read character (1FFF + 4 since jump address is 1FFF)
    MOV V5, [$EFFF] ; Debug
    AND V5, $01 ; Mask to get first bit (color)
    MOV DX, V5
    MOV V5, [$EFFF] ; Debug
    MUL V5, V3 ; V5 = Character * Color (color index)
    MOV AX, V5 ; Store result into V5
    
    MOV V5, [$EFFF] ; Debug
    
    MOVB V5, [V4 + $402F] ; Write character to screen memory
    RET