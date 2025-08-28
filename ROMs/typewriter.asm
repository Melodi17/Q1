    jmp _main
    .include "font.bin"
_screen_setPixel_int_x__int_y__int_c:
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

_screen_setPixel_int_i__int_c:
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

_screen_clear_int_c:
    pop V0
    pop [$720F]
    push V0
    mov $402F, [$7211]
    mov 0, [$7213]
_body_1:
    push [$7213]
    push 128
    mov 64, V1
    pop V0
    mul V0, V1
    mov AX, V1
    pop V0
    lt V0, V1
    not LX
    bz
    jmp _end_2
    push [$7211]
    mov [$7213], V1
    pop V0
    mul V1, 1
    add AX, V0
    mov AX, V0
    movb [$720F], [V0]
    push [$7213]
    inc [$7213]
    pop V0
    jmp _body_1
_end_2:
    mov 0, V0
    ret

_hid_getKeyState_int_scanCode:
    pop V0
    pop [$7215]
    push V0
    mov $6033, [$7217]
    push [$7215]
    mov 8, V1
    pop V0
    div V0, V1
    mov AX, [$7219]
    push [$7215]
    mov 8, V1
    pop V0
    mod V0, V1
    mov AX, [$721B]
    push [$7217]
    push [$7219]
    mov 1, V1
    pop V0
    sub V0, V1
    mov AX, V1
    pop V0
    mul V1, 1
    add AX, V0
    mov AX, V0
    mov [V0], [$721D]
    push [$721D]
    push 1
    mov [$721B], V1
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
    mov 0, [$721F]
_body_3:
    push [$721F]
    mov 256, V1
    pop V0
    lt V0, V1
    not LX
    bz
    jmp _end_4
    push [$721F]
    call _hid_getKeyState_int_scanCode
    cmp V0, 0
    not
    bz
    jmp _clause_5
    mov 0, V0
    jmp _end_6
_clause_5:
    push [$721F]
    mov 160, V1
    pop V0
    cmp V0, V1
    not
    cmp LX, 0
    not
    mov LX, V0
_end_6:
    not V0
    bz
    jmp _end_7
    mov [$721F], V0
    ret
_end_7:
    push [$721F]
    inc [$721F]
    pop V0
    jmp _body_3
_end_4:
    mov 0, V0
    ret
    mov 0, V0
    ret

_hid_waitForPressedKey:
_body_8:
    not 1
    bz
    jmp _end_9
    call _hid_getPressedKey
    mov V0, [$7221]
    push [$7221]
    mov 0, V1
    pop V0
    cmp V0, V1
    not
    not LX
    bz
    jmp _end_10
    mov [$7221], V0
    ret
_end_10:
    jmp _body_8
_end_9:
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

_drawChar_int_ch__int_x__int_y__int_color:
    pop V0
    pop [$7223]
    pop [$7225]
    pop [$7227]
    pop [$7229]
    push V0
    ; adjust for jmp instruction
    push $1FFF
    mov 2, V1
    pop V0
    add V0, V1
    mov AX, [$722B]
    push [$7223]
    mov 8, V1
    pop V0
    mul V0, V1
    mov AX, [$722D]
    mov 0, [$722F]
_body_11:
    push [$722F]
    mov 8, V1
    pop V0
    lt V0, V1
    not LX
    bz
    jmp _end_12
    push [$722B]
    push [$722D]
    mov [$722F], V1
    pop V0
    add V0, V1
    mov AX, V1
    pop V0
    mul V1, 1
    add AX, V0
    mov AX, V0
    mov [V0], [$7231]
    mov 0, [$7233]
_body_13:
    push [$7233]
    mov 8, V1
    pop V0
    lt V0, V1
    not LX
    bz
    jmp _end_14
    push [$7231]
    push 7
    mov [$7233], V1
    pop V0
    sub V0, V1
    mov AX, V1
    pop V0
    shr V0, V1
    push DX
    mov 1, V1
    pop V0
    and V0, V1
    mov DX, [$7235]
    bz [$7235]
    jmp _then_15
    mov [$7229], V0
    jmp _end_16
_then_15:
    mov 8, V0
_end_16:
    mov V0, [$7237]
    push [$7237]
    push [$7227]
    mov [$722F], V1
    pop V0
    add V0, V1
    push AX
    push [$7225]
    mov [$7233], V1
    pop V0
    add V0, V1
    push AX
    call _screen_setPixel_int_x__int_y__int_c
    push [$7233]
    inc [$7233]
    pop V0
    jmp _body_13
_end_14:
    push [$722F]
    inc [$722F]
    pop V0
    jmp _body_11
_end_12:
    mov 0, V0
    ret

_print_int*_text__int_len__int_x__int_y__int_color:
    pop V0
    pop [$7239]
    pop [$723B]
    pop [$723D]
    pop [$723F]
    pop [$7241]
    push V0
    mov 0, [$7243]
_body_17:
    push [$7243]
    mov [$723B], V1
    pop V0
    lt V0, V1
    not LX
    bz
    jmp _end_18
    push [$7239]
    mov [$7243], V1
    pop V0
    mul V1, 2
    add AX, V0
    mov AX, V0
    mov [V0], [$7245]
    push [$7241]
    push [$723F]
    push [$723D]
    push [$7243]
    mov 8, V1
    pop V0
    mul V0, V1
    mov AX, V1
    pop V0
    add V0, V1
    push AX
    push [$7245]
    call _drawChar_int_ch__int_x__int_y__int_color
    push [$7243]
    inc [$7243]
    pop V0
    jmp _body_17
_end_18:
    mov 0, V0
    ret

_main:
    mov 0, [$7247]
    mov 0, [$7249]
_body_19:
    not 1
    bz
    jmp _end_20
    push [$7247]
    mov 128, V1
    pop V0
    cmp V0, V1
    bz
    jmp _end_21
    gt V0, V1
_end_21:
    not LX
    bz
    jmp _end_22
    mov 0, [$7247]
    push [$7249]
    mov 8, V1
    pop V0
    add V0, V1
    push AX
    pop V1
    mov V1, [$7249]
_end_22:
    push [$7249]
    mov 64, V1
    pop V0
    cmp V0, V1
    bz
    jmp _end_23
    gt V0, V1
_end_23:
    not LX
    bz
    jmp _end_24
    mov 0, [$7247]
    mov 0, [$7249]
    push 8
    call _screen_clear_int_c
_end_24:
    call _hid_waitForPressedKey
    mov V0, [$724B]
_body_25:
    push [$724B]
    call _hid_getKeyState_int_scanCode
    not V0
    bz
    jmp _end_26
    jmp _body_25
_end_26:
    push [$724B]
    mov 13, V1
    pop V0
    cmp V0, V1
    not LX
    bz
    jmp _end_27
    push [$7249]
    mov 8, V1
    pop V0
    add V0, V1
    push AX
    pop V1
    mov V1, [$7249]
    mov 0, [$7247]
    jmp _body_19
_end_27:
    push [$724B]
    mov 8, V1
    pop V0
    cmp V0, V1
    not LX
    bz
    jmp _end_28
    push [$7247]
    mov 8, V1
    pop V0
    sub V0, V1
    push AX
    pop V1
    mov V1, [$7247]
    push [$7247]
    mov 0, V1
    pop V0
    lt V0, V1
    not LX
    bz
    jmp _end_29
    mov 0, [$7247]
_end_29:
    push 1
    push [$7249]
    push [$7247]
    push 32
    call _drawChar_int_ch__int_x__int_y__int_color
    jmp _body_19
_end_28:
    push 1
    push [$7249]
    push [$7247]
    push [$724B]
    call _drawChar_int_ch__int_x__int_y__int_color
    push [$7247]
    mov 8, V1
    pop V0
    add V0, V1
    push AX
    pop V1
    mov V1, [$7247]
    jmp _body_19
_end_20:
    mov 0, V0
    ret

