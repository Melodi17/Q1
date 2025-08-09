    jmp _main
_main:                        ; int main()
    mov 1, [$51FF]            ; int x = 1
    mov 0, [$5201]            ; int i = 0
_body_1:
    push [$5201]              ; left operand
    mov 10, V1                ; right operand
    pop V0                    ; get left operand back
    lt V0, V1                 ; compute less than check
    not LX
    bz
    jmp _end_2                ; if for condition fails, jump to end
    mult [$51FF], 2           ; compute multiplication
    mov AX, [$51FF]           ; store back into left param
    push [$5201]              ; i++
    inc [$5201]
    pop V0
_end_2:
    mov 0, V0                 ; fallback return 0
    ret

