    jmp _main
_setPixel_int_x__int_y__int_c:; int setPixel(int x, int y, int c)
    pop V0                    ; preserve function return address
    pop [$71FF]               ; param int x
    pop [$7201]               ; param int y
    pop [$7203]               ; param int c
    push V0                   ; restore function return address
    mov $402F, [$7205]        ; int screenStart = $402F
    push [$7201]              ; left operand
    mov 128, V1               ; right operand
    pop V0                    ; get left operand back
    mul V0, V1                ; compute multiplication
    push AX                   ; left operand
    mov [$71FF], V1           ; right operand
    pop V0                    ; get left operand back
    add V0, V1                ; compute addition
    mov AX, [$7207]           ; int i = AX
    push [$7205]              ; store index obj
    mov [$7207], V1           ; store indexer
    pop V0                    ; restore index obj
    mul V1, 1                 ; multiply indexer by 1
    add AX, V0                ; add indexer and index obj
    mov AX, V0                ; move to general purpose register
    movb [$7203], [V0]        ; [V0] = [$7203]
    mov 0, V0                 ; fallback return 0
    ret

_setPixelIndex_int_i__int_c:  ; int setPixelIndex(int i, int c)
    pop V0                    ; preserve function return address
    pop [$7209]               ; param int i
    pop [$720B]               ; param int c
    push V0                   ; restore function return address
    mov $402F, [$720D]        ; int screenStart = $402F
    push [$720D]              ; store index obj
    mov [$7209], V1           ; store indexer
    pop V0                    ; restore index obj
    mul V1, 1                 ; multiply indexer by 1
    add AX, V0                ; add indexer and index obj
    mov AX, V0                ; move to general purpose register
    movb [$720B], [V0]        ; [V0] = [$720B]
    mov 0, V0                 ; fallback return 0
    ret

_clear_int_c:                 ; int clear(int c)
    pop V0                    ; preserve function return address
    pop [$720F]               ; param int c
    push V0                   ; restore function return address
    mov $402F, [$7211]        ; int screenStart = $402F
    push 64                   ; left operand
    mov 128, V1               ; right operand
    pop V0                    ; get left operand back
    mul V0, V1                ; compute multiplication
    mov AX, [$7213]           ; int screenSize = AX
    mov 0, [$7215]            ; int i = 0
_body_1:
    push [$7215]              ; left operand
    mov [$7213], V1           ; right operand
    pop V0                    ; get left operand back
    lt V0, V1                 ; compute less than check
    not LX
    bz
    jmp _end_2                ; if for condition fails, jump to end
    push [$7211]              ; store index obj
    mov [$7215], V1           ; store indexer
    pop V0                    ; restore index obj
    mul V1, 1                 ; multiply indexer by 1
    add AX, V0                ; add indexer and index obj
    mov AX, V0                ; move to general purpose register
    movb [$720F], [V0]        ; [V0] = [$720F]
    push [$7215]              ; i++
    inc [$7215]
    pop V0
    jmp _body_1
_end_2:
    mov 0, V0                 ; fallback return 0
    ret

_getKeyState_int_scanCode:    ; int getKeyState(int scanCode)
    pop V0                    ; preserve function return address
    pop [$7217]               ; param int scanCode
    push V0                   ; restore function return address
    mov $6033, [$7219]        ; int hid = $6033
    push [$7217]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    div V0, V1                ; compute division
    mov AX, [$721B]           ; int index = AX
    push [$7217]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    mod V0, V1                ; compute modulus
    mov AX, [$721D]           ; int bit = AX
    push [$7219]              ; store index obj
    push [$721B]              ; left operand
    mov 1, V1                 ; right operand
    pop V0                    ; get left operand back
    sub V0, V1                ; compute subtraction
    mov AX, V1                ; store indexer
    pop V0                    ; restore index obj
    mul V1, 1                 ; multiply indexer by 1
    add AX, V0                ; add indexer and index obj
    mov AX, V0                ; move to general purpose register
    mov [V0], [$721F]         ; int val = [V0]
    push [$721F]              ; left operand
    push 1                    ; left operand
    mov [$721D], V1           ; right operand
    pop V0                    ; get left operand back
    shl V0, V1                ; compute left shift
    mov DX, V1                ; right operand
    pop V0                    ; get left operand back
    and V0, V1                ; compute and
    push DX                   ; left operand
    mov 0, V1                 ; right operand
    pop V0                    ; get left operand back
    cmp V0, V1                ; compute inequality check
    not
    mov LX, V0                ; return value
    ret
    mov 0, V0                 ; fallback return 0
    ret

_main:                        ; int main()
    mov 0, [$7221]            ; int color = 0
_body_3:
    not 1
    bz
    jmp _end_4                ; if while condition fails, jump to end
    mov $602F, [$7223]        ; int hid = $602F
    push [$7223]              ; store index obj
    mov 0, V1                 ; store indexer
    pop V0                    ; restore index obj
    mul V1, 2                 ; multiply indexer by 2
    add AX, V0                ; add indexer and index obj
    mov AX, V0                ; move to general purpose register
    mov [V0], [$7225]         ; int mouseIndex = [V0]
    push [$7223]              ; store index obj
    mov 1, V1                 ; store indexer
    pop V0                    ; restore index obj
    mul V1, 2                 ; multiply indexer by 2
    add AX, V0                ; add indexer and index obj
    mov AX, V0                ; move to general purpose register
    mov [V0], [$7227]         ; int mouseState = [V0]
    push [$7227]              ; left operand
    mov 1, V1                 ; right operand
    pop V0                    ; get left operand back
    and V0, V1                ; compute and
    not DX
    bz                        ; check if condition
    jmp _end_5                ; if succeeds, continue, else jump to end
    push [$7221]              ; left operand
    mov 3, V1                 ; right operand
    pop V0                    ; get left operand back
    mod V0, V1                ; compute modulus
    push AX                   ; left operand
    mov 1, V1                 ; right operand
    pop V0                    ; get left operand back
    add V0, V1                ; compute addition
    push AX                   ; arg int i
    push [$7225]              ; arg int c
    call _setPixelIndex_int_i__int_c; int setPixelIndex(int i, int c)
_end_5:
    push 67                   ; arg int scanCode
    call _getKeyState_int_scanCode; int getKeyState(int scanCode)
    not V0
    bz                        ; check if condition
    jmp _end_6                ; if succeeds, continue, else jump to end
    push 8                    ; arg int c
    call _clear_int_c         ; int clear(int c)
_end_6:
    push 65                   ; arg int scanCode
    call _getKeyState_int_scanCode; int getKeyState(int scanCode)
    not V0
    bz                        ; check if condition
    jmp _end_7                ; if succeeds, continue, else jump to end
    push [$7221]              ; i++
    inc [$7221]
    pop V0
_body_8:
    push 65                   ; arg int scanCode
    call _getKeyState_int_scanCode; int getKeyState(int scanCode)
    not V0
    bz
    jmp _end_9                ; if while condition fails, jump to end
    jmp _body_8
_end_9:
_end_7:
    jmp _body_3
_end_4:
    mov 0, V0                 ; fallback return 0
    ret

