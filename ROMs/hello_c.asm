    jmp _main
_add_int_a__int_b:            ; int add(int a, int b)
    pop V0                    ; Pop return address to register, to preserve during parameter access
    pop [$51FF]               ; param int a
    pop [$5201]               ; param int b
    push V0                   ; Restore function return address
    push [$51FF]              ; left operand
    mov [$5201], V1           ; right operand
    pop V0                    ; get left operand back
    add V0, V1                ; compute addition
    mov AX, V0                ; return value
    ret
    mov 0, V0                 ; fallback return 0
    ret

_main:                        ; int main()
    push 2                    ; arg int a
    push 1                    ; arg int b
    call _add_int_a__int_b    ; int add(int a, int b)
    mov V0, V0                ; return value
    ret
    mov 0, V0                 ; fallback return 0
    ret

