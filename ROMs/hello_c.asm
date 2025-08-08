_main:
    ; comment
    mov 5, [$51FF]            ; int a = 5
    push [$51FF]              ; left operand
    mov 5, V1                 ; right operand
    pop V0                    ; get left operand back
    cmp V0, V1                ; compute equality check
    bz LX                     ; check if condition
    not
    jmp _end_1                ; if succeeds, continue, else jump to end
    mov 99, V0                ; return value
    ret
_end_1:
    mov 0, V0                 ; return value
    ret
    mov 0, V0                 ; fallback return 0
    ret

