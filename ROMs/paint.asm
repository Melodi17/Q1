    jmp _main
_hid_getKeyState_int_scanCode:; int hid_getKeyState(int scanCode)
    pop V0                    ; preserve function return address
    pop [$71FF]               ; param int scanCode
    push V0                   ; restore function return address
    mov $6033, [$7201]        ; int sk_table = $6033
    push [$71FF]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    div V0, V1                ; compute division
    mov AX, [$7203]           ; int index = AX
    push [$71FF]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    mod V0, V1                ; compute modulus
    mov AX, [$7205]           ; int bit = AX
    push [$7201]              ; store index obj
    push [$7203]              ; left operand
    mov 1, V1                 ; right operand
    pop V0                    ; get left operand back
    sub V0, V1                ; compute subtraction
    mov AX, V1                ; store indexer
    pop V0                    ; restore index obj
    mul V1, 1                 ; multiply indexer by 1
    add AX, V0                ; add indexer and index obj
    mov AX, V0                ; move to general purpose register
    mov [V0], [$7207]         ; int val = [V0]
    push [$7207]              ; left operand
    push 1                    ; left operand
    mov [$7205], V1           ; right operand
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

_hid_getPressedKey:           ; int hid_getPressedKey()
    mov 0, [$7209]            ; int sk = 0
_body_1:
    push [$7209]              ; left operand
    mov 256, V1               ; right operand
    pop V0                    ; get left operand back
    lt V0, V1                 ; compute less than check
    not LX
    bz
    jmp _end_2                ; if for condition fails, jump to end
    push [$7209]              ; arg int scanCode
    call _hid_getKeyState_int_scanCode; int hid_getKeyState(int scanCode)
    cmp V0, 0                 ; logical and
    not
    bz
    jmp _clause_3
    mov 0, V0                 ; first operand failed, store 0 and skip second
    jmp _end_4
_clause_3:
    push [$7209]              ; left operand
    mov 160, V1               ; right operand
    pop V0                    ; get left operand back
    cmp V0, V1                ; compute inequality check
    not
    cmp LX, 0                 ; compare result from second operand to 0
    not
    mov LX, V0                ; store second operand result
_end_4:
    not V0
    bz                        ; check if condition
    jmp _end_5                ; if succeeds, continue, else jump to end
    mov [$7209], V0           ; return value
    ret
_end_5:
    push [$7209]              ; i++
    inc [$7209]
    pop V0
    jmp _body_1
_end_2:
    mov 0, V0                 ; return value
    ret
    mov 0, V0                 ; fallback return 0
    ret

_hid_waitForPressedKey:       ; int hid_waitForPressedKey()
_body_6:
    not 1
    bz
    jmp _end_7                ; if while condition fails, jump to end
    call _hid_getPressedKey   ; int hid_getPressedKey()
    mov V0, [$720B]           ; int sk = V0
    push [$720B]              ; left operand
    mov 0, V1                 ; right operand
    pop V0                    ; get left operand back
    cmp V0, V1                ; compute inequality check
    not
    not LX
    bz                        ; check if condition
    jmp _end_8                ; if succeeds, continue, else jump to end
    mov [$720B], V0           ; return value
    ret
_end_8:
    jmp _body_6
_end_7:
    mov 0, V0                 ; fallback return 0
    ret

_hid_getMouseIndex:           ; int hid_getMouseIndex()
    mov $602F, V0             ; move pointer into register for dereference
    mov [V0], V0              ; return value
    ret
    mov 0, V0                 ; fallback return 0
    ret

_hid_getMouseState:           ; int hid_getMouseState()
    push $602F                ; left operand
    mov 2, V1                 ; right operand
    pop V0                    ; get left operand back
    add V0, V1                ; compute addition
    mov AX, V0                ; move pointer into register for dereference
    mov [V0], V0              ; return value
    ret
    mov 0, V0                 ; fallback return 0
    ret

_screen_setPixel_int_x__int_y__int_c:; int screen_setPixel(int x, int y, int c)
    pop V0                    ; preserve function return address
    pop [$720D]               ; param int x
    pop [$720F]               ; param int y
    pop [$7211]               ; param int c
    push V0                   ; restore function return address
    mov $402F, [$7213]        ; int screenStart = $402F
    push [$720F]              ; left operand
    mov 128, V1               ; right operand
    pop V0                    ; get left operand back
    mul V0, V1                ; compute multiplication
    push AX                   ; left operand
    mov [$720D], V1           ; right operand
    pop V0                    ; get left operand back
    add V0, V1                ; compute addition
    mov AX, [$7215]           ; int i = AX
    push [$7213]              ; store index obj
    mov [$7215], V1           ; store indexer
    pop V0                    ; restore index obj
    mul V1, 1                 ; multiply indexer by 1
    add AX, V0                ; add indexer and index obj
    mov AX, V0                ; move to general purpose register
    movb [$7211], [V0]        ; [V0] = [$7211]
    mov 0, V0                 ; fallback return 0
    ret

_screen_setPixel_int_i__int_c:; int screen_setPixel(int i, int c)
    pop V0                    ; preserve function return address
    pop [$7217]               ; param int i
    pop [$7219]               ; param int c
    push V0                   ; restore function return address
    mov $402F, [$721B]        ; int screenStart = $402F
    push [$721B]              ; store index obj
    mov [$7217], V1           ; store indexer
    pop V0                    ; restore index obj
    mul V1, 1                 ; multiply indexer by 1
    add AX, V0                ; add indexer and index obj
    mov AX, V0                ; move to general purpose register
    movb [$7219], [V0]        ; [V0] = [$7219]
    mov 0, V0                 ; fallback return 0
    ret

_screen_clear_int_c:          ; int screen_clear(int c)
    pop V0                    ; preserve function return address
    pop [$721D]               ; param int c
    push V0                   ; restore function return address
    mov $402F, [$721F]        ; int screenStart = $402F
    mov 0, [$7221]            ; int i = 0
_body_9:
    push [$7221]              ; left operand
    push 128                  ; left operand
    mov 64, V1                ; right operand
    pop V0                    ; get left operand back
    mul V0, V1                ; compute multiplication
    mov AX, V1                ; right operand
    pop V0                    ; get left operand back
    lt V0, V1                 ; compute less than check
    not LX
    bz
    jmp _end_10               ; if for condition fails, jump to end
    push [$721F]              ; store index obj
    mov [$7221], V1           ; store indexer
    pop V0                    ; restore index obj
    mul V1, 1                 ; multiply indexer by 1
    add AX, V0                ; add indexer and index obj
    mov AX, V0                ; move to general purpose register
    movb [$721D], [V0]        ; [V0] = [$721D]
    push [$7221]              ; i++
    inc [$7221]
    pop V0
    jmp _body_9
_end_10:
    mov 0, V0                 ; fallback return 0
    ret

_main:                        ; int main()
    mov 0, [$7223]            ; int color = 0
_body_11:
    not 1
    bz
    jmp _end_12               ; if while condition fails, jump to end
    call _hid_getMouseIndex   ; int hid_getMouseIndex()
    mov V0, [$7225]           ; int mouseIndex = V0
    call _hid_getMouseState   ; int hid_getMouseState()
    mov V0, [$7227]           ; int mouseState = V0
    push [$7227]              ; left operand
    mov 1, V1                 ; right operand
    pop V0                    ; get left operand back
    and V0, V1                ; compute and
    not DX
    bz                        ; check if condition
    jmp _end_13               ; if succeeds, continue, else jump to end
    push [$7223]              ; left operand
    mov 3, V1                 ; right operand
    pop V0                    ; get left operand back
    mod V0, V1                ; compute modulus
    push AX                   ; left operand
    mov 1, V1                 ; right operand
    pop V0                    ; get left operand back
    add V0, V1                ; compute addition
    push AX                   ; arg int i
    push [$7225]              ; arg int c
    call _screen_setPixel_int_i__int_c; int screen_setPixel(int i, int c)
_end_13:
    push 67                   ; arg int scanCode
    call _hid_getKeyState_int_scanCode; int hid_getKeyState(int scanCode)
    not V0
    bz                        ; check if condition
    jmp _end_14               ; if succeeds, continue, else jump to end
    push 8                    ; arg int c
    call _screen_clear_int_c  ; int screen_clear(int c)
_end_14:
    push 65                   ; arg int scanCode
    call _hid_getKeyState_int_scanCode; int hid_getKeyState(int scanCode)
    not V0
    bz                        ; check if condition
    jmp _end_15               ; if succeeds, continue, else jump to end
    push [$7223]              ; i++
    inc [$7223]
    pop V0
_body_16:
    push 65                   ; arg int scanCode
    call _hid_getKeyState_int_scanCode; int hid_getKeyState(int scanCode)
    not V0
    bz
    jmp _end_17               ; if while condition fails, jump to end
    jmp _body_16
_end_17:
_end_15:
    jmp _body_11
_end_12:
    mov 0, V0                 ; fallback return 0
    ret

