    jmp _main
_hid_getKeyState_int_scanCode:
    pop V0
    pop [$71FF]
    push V0
    mov $6033, [$7201]
    push [$71FF]
    mov 8, V1
    pop V0
    div V0, V1
    mov AX, [$7203]
    push [$71FF]
    mov 8, V1
    pop V0
    mod V0, V1
    mov AX, [$7205]
    push [$7201]
    push [$7203]
    mov 1, V1
    pop V0
    sub V0, V1
    mov AX, V1
    pop V0
    mul V1, 1
    add AX, V0
    mov AX, V0
    mov [V0], [$7207]
    push [$7207]
    push 1
    mov [$7205], V1
    pop V0
    shl V0, V1
    mov DX, V1
    pop V0
    and V0, V1
    push DX
    mov 0, V1
    pop V0
    cmp V0, V1
    not
    mov LX, V0
    ret
    mov 0, V0
    ret

_hid_getPressedKey:
    mov 0, [$7209]
_body_1:
    push [$7209]
    mov 256, V1
    pop V0
    lt V0, V1
    not LX
    bz
    jmp _end_2
    push [$7209]
    call _hid_getKeyState_int_scanCode
    cmp V0, 0
    not
    bz
    jmp _clause_3
    mov 0, V0
    jmp _end_4
_clause_3:
    push [$7209]
    mov 160, V1
    pop V0
    cmp V0, V1
    not
    cmp LX, 0
    not
    mov LX, V0
_end_4:
    not V0
    bz
    jmp _end_5
    mov [$7209], V0
    ret
_end_5:
    push [$7209]
    inc [$7209]
    pop V0
    jmp _body_1
_end_2:
    mov 0, V0
    ret
    mov 0, V0
    ret

_hid_waitForPressedKey:
_body_6:
    not 1
    bz
    jmp _end_7
    call _hid_getPressedKey
    mov V0, [$720B]
    push [$720B]
    mov 0, V1
    pop V0
    cmp V0, V1
    not
    not LX
    bz
    jmp _end_8
    mov [$720B], V0
    ret
_end_8:
    jmp _body_6
_end_7:
    mov 0, V0
    ret

_hid_getMouseIndex:
    mov $602F, V0
    mov [V0], V0
    ret
    mov 0, V0
    ret

_hid_getMouseState:
    push $602F
    mov 2, V1
    pop V0
    add V0, V1
    mov AX, V0
    mov [V0], V0
    ret
    mov 0, V0
    ret

_screen_setPixel_int_x__int_y__int_c:
    pop V0
    pop [$720D]
    pop [$720F]
    pop [$7211]
    push V0
    mov $402F, [$7213]
    push [$720F]
    mov 128, V1
    pop V0
    mul V0, V1
    push AX
    mov [$720D], V1
    pop V0
    add V0, V1
    mov AX, [$7215]
    push [$7213]
    mov [$7215], V1
    pop V0
    mul V1, 1
    add AX, V0
    mov AX, V0
    movb [$7211], [V0]
    mov 0, V0
    ret

_screen_setPixel_int_i__int_c:
    pop V0
    pop [$7217]
    pop [$7219]
    push V0
    mov $402F, [$721B]
    push [$721B]
    mov [$7217], V1
    pop V0
    mul V1, 1
    add AX, V0
    mov AX, V0
    movb [$7219], [V0]
    mov 0, V0
    ret

_screen_clear_int_c:
    pop V0
    pop [$721D]
    push V0
    mov $402F, [$721F]
    mov 0, [$7221]
_body_9:
    push [$7221]
    push 128
    mov 64, V1
    pop V0
    mul V0, V1
    mov AX, V1
    pop V0
    lt V0, V1
    not LX
    bz
    jmp _end_10
    push [$721F]
    mov [$7221], V1
    pop V0
    mul V1, 1
    add AX, V0
    mov AX, V0
    movb [$721D], [V0]
    push [$7221]
    inc [$7221]
    pop V0
    jmp _body_9
_end_10:
    mov 0, V0
    ret

_main:
    mov 0, [$7223]
_body_11:
    not 1
    bz
    jmp _end_12
    call _hid_getMouseIndex
    mov V0, [$7225]
    call _hid_getMouseState
    mov V0, [$7227]
    push [$7227]
    mov 1, V1
    pop V0
    and V0, V1
    not DX
    bz
    jmp _end_13
    push [$7223]
    mov 3, V1
    pop V0
    mod V0, V1
    push AX
    mov 1, V1
    pop V0
    add V0, V1
    push AX
    push [$7225]
    call _screen_setPixel_int_i__int_c
_end_13:
    push 67
    call _hid_getKeyState_int_scanCode
    not V0
    bz
    jmp _end_14
    push 8
    call _screen_clear_int_c
_end_14:
    push 65
    call _hid_getKeyState_int_scanCode
    not V0
    bz
    jmp _end_15
    push [$7223]
    inc [$7223]
    pop V0
_body_16:
    push 65
    call _hid_getKeyState_int_scanCode
    not V0
    bz
    jmp _end_17
    jmp _body_16
_end_17:
_end_15:
    jmp _body_11
_end_12:
    mov 0, V0
    ret

