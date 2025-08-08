_main:
    ; comment
    mov 5, [$51FF]            ; int a = 5
    add [$51FF], 5            ; compute addition
    mov AX, [$51FF]           ; store back into left param
    mov [$51FF], V0           ; return value
    ret
    mov 0, V0                 ; fallback return 0
    ret

