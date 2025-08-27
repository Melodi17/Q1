    jmp _main
_setPixel_int_x__int_y__int_c:
    pop V0
    pop [$71FF]
    pop [$7201]
    pop [$7203]
    push V0
    mov $402F, [$7205]
    push [$7201]
    mov 128, V1
    pop V0
    mul V0, V1
    push AX
    mov [$71FF], V1
    pop V0
    add V0, V1
    mov AX, [$7207]
    push [$7205]
    mov [$7207], V1
    pop V0
    mul V1, 1
    add AX, V0
    mov AX, V0
    movb [$7203], [V0]
    mov 0, V0
    ret

_setPixelIndex_int_i__int_c:
    pop V0
    pop [$7209]
    pop [$720B]
    push V0
    mov $402F, [$720D]
    push [$720D]
    mov [$7209], V1
    pop V0
    mul V1, 1
    add AX, V0
    mov AX, V0
    movb [$720B], [V0]
    mov 0, V0
    ret

_clear_int_c:
    pop V0
    pop [$720F]
    push V0
    mov $402F, [$7211]
    push 64
    mov 128, V1
    pop V0
    mul V0, V1
    mov AX, [$7213]
    mov 0, [$7215]
_body_1:
    push [$7215]
    mov [$7213], V1
    pop V0
    lt V0, V1
    not LX
    bz
    jmp _end_2
    push [$7211]
    mov [$7215], V1
    pop V0
    mul V1, 1
    add AX, V0
    mov AX, V0
    movb [$720F], [V0]
    push [$7215]
    inc [$7215]
    pop V0
    jmp _body_1
_end_2:
    mov 0, V0
    ret

_main:
_body_3:
    not 1
    bz
    jmp _end_4
    mov $602F, [$7217]
    push [$7217]
    mov 0, V1
    pop V0
    mul V1, 2
    add AX, V0
    mov AX, V0
    mov [V0], [$7219]
    push [$7217]
    mov 1, V1
    pop V0
    mul V1, 2
    add AX, V0
    mov AX, V0
    mov [V0], [$721B]
    push [$721B]
    push 1
    mov 1, V1
    pop V0
    cmp V0, V1
    mov LX, V1
    pop V0
    and V0, V1
    not DX
    bz
    jmp _end_5
    push 1
    push [$7219]
    call _setPixelIndex_int_i__int_c
_end_5:
    jmp _body_3
_end_4:
    mov 0, V0
    ret

