    jmp _main
    .include "ROMs/font.bin"
_drawChar_int_ch__int_x__int_y__int_color:
    pop V0
    pop [$71FF]
    pop [$7201]
    pop [$7203]
    pop [$7205]
    push V0
    ; adjust for jmp instruction
    push $1FFF
    mov 2, V1
    pop V0
    add V0, V1
    mov AX, [$7207]
    push [$71FF]
    mov 8, V1
    pop V0
    mul V0, V1
    mov AX, [$7209]
    mov 0, [$720B]
_body_1:
    push [$720B]
    mov 8, V1
    pop V0
    lt V0, V1
    not LX
    bz
    jmp _end_2
    push [$7207]
    push [$7209]
    mov [$720B], V1
    pop V0
    add V0, V1
    mov AX, V1
    pop V0
    mul V1, 1
    add AX, V0
    mov AX, V0
    mov [V0], [$720D]
    mov 0, [$720F]
_body_3:
    push [$720F]
    mov 8, V1
    pop V0
    lt V0, V1
    not LX
    bz
    jmp _end_4
    push [$720D]
    push 7
    mov [$720F], V1
    pop V0
    sub V0, V1
    mov AX, V1
    pop V0
    shr V0, V1
    push DX
    mov 1, V1
    pop V0
    and V0, V1
    mov DX, [$7211]
    bz [$7211]
    jmp _then_5
    mov [$7205], V0
    jmp _end_6
_then_5:
    mov 0, V0
_end_6:
    mov V0, [$7213]
    push [$7213]
    push [$7203]
    mov [$720B], V1
    pop V0
    add V0, V1
    push AX
    push [$7201]
    mov [$720F], V1
    pop V0
    add V0, V1
    push AX
    call _setPixel_int_x__int_y__int_c
    push [$720F]
    inc [$720F]
    pop V0
    jmp _body_3
_end_4:
    push [$720B]
    inc [$720B]
    pop V0
    jmp _body_1
_end_2:
    mov 0, V0
    ret

_setPixel_int_x__int_y__int_c:
    pop V0
    pop [$7215]
    pop [$7217]
    pop [$7219]
    push V0
    mov $402F, [$721B]
    push [$7217]
    mov 128, V1
    pop V0
    mul V0, V1
    push AX
    mov [$7215], V1
    pop V0
    add V0, V1
    mov AX, [$721D]
    push [$721B]
    mov [$721D], V1
    pop V0
    mul V1, 1
    add AX, V0
    mov AX, V0
    movb [$7219], [V0]
    mov 0, V0
    ret

_clear_int_c:
    pop V0
    pop [$721F]
    push V0
    mov $402F, [$7221]
    push 64
    mov 128, V1
    pop V0
    mul V0, V1
    mov AX, [$7223]
    mov 0, [$7225]
_body_7:
    push [$7225]
    mov [$7223], V1
    pop V0
    lt V0, V1
    not LX
    bz
    jmp _end_8
    push [$7221]
    mov [$7225], V1
    pop V0
    mul V1, 1
    add AX, V0
    mov AX, V0
    movb [$721F], [V0]
    push [$7225]
    inc [$7225]
    pop V0
    jmp _body_7
_end_8:
    mov 0, V0
    ret

_main:
    mov 1, [$7227]
    mov 72, [$722B]
    mov 69, [$722D]
    mov 76, [$722F]
    mov 76, [$7231]
    mov 79, [$7233]
    mov 32, [$7235]
    mov 87, [$7237]
    mov 79, [$7239]
    mov 82, [$723B]
    mov 76, [$723D]
    mov 68, [$723F]
    mov 33, [$7241]
    mov $722B, [$7229]
    mov 12, [$7243]
    push 0
    call _clear_int_c
_body_9:
    not 1
    bz
    jmp _end_10
    mov 0, [$7245]
_body_11:
    push [$7245]
    mov [$7243], V1
    pop V0
    lt V0, V1
    not LX
    bz
    jmp _end_12
    push [$7229]
    mov [$7245], V1
    pop V0
    mul V1, 2
    add AX, V0
    mov AX, V0
    mov [V0], [$7247]
    push [$7227]
    push 0
    push [$7245]
    mov 8, V1
    pop V0
    mul V0, V1
    push AX
    push [$7247]
    call _drawChar_int_ch__int_x__int_y__int_color
    push [$7245]
    inc [$7245]
    pop V0
    jmp _body_11
_end_12:
    push [$7227]
    inc [$7227]
    pop V0
    jmp _body_9
_end_10:
    mov 0, V0
    ret

