    jmp _main
    .include "font.bin"
_screen_setPixel_int_x__int_y__int_c:; int screen_setPixel(int x, int y, int c)
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

_screen_setPixel_int_i__int_c:; int screen_setPixel(int i, int c)
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

_screen_clear_int_c:          ; int screen_clear(int c)
    pop V0                    ; preserve function return address
    pop [$720F]               ; param int c
    push V0                   ; restore function return address
    mov $402F, [$7211]        ; int screenStart = $402F
    mov 0, [$7213]            ; int i = 0
_body_1:
    push [$7213]              ; left operand
    push 128                  ; left operand
    mov 64, V1                ; right operand
    pop V0                    ; get left operand back
    mul V0, V1                ; compute multiplication
    mov AX, V1                ; right operand
    pop V0                    ; get left operand back
    lt V0, V1                 ; compute less than check
    not LX
    bz
    jmp _end_2                ; if for condition fails, jump to end
    push [$7211]              ; store index obj
    mov [$7213], V1           ; store indexer
    pop V0                    ; restore index obj
    mul V1, 1                 ; multiply indexer by 1
    add AX, V0                ; add indexer and index obj
    mov AX, V0                ; move to general purpose register
    movb [$720F], [V0]        ; [V0] = [$720F]
    push [$7213]              ; i++
    inc [$7213]
    pop V0
    jmp _body_1
_end_2:
    mov 0, V0                 ; fallback return 0
    ret

_hid_getKeyState_int_scanCode:; int hid_getKeyState(int scanCode)
    pop V0                    ; preserve function return address
    pop [$7215]               ; param int scanCode
    push V0                   ; restore function return address
    mov $6033, [$7217]        ; int sk_table = $6033
    push [$7215]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    div V0, V1                ; compute division
    mov AX, [$7219]           ; int index = AX
    push [$7215]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    mod V0, V1                ; compute modulus
    mov AX, [$721B]           ; int bit = AX
    push [$7217]              ; store index obj
    push [$7219]              ; left operand
    mov 1, V1                 ; right operand
    pop V0                    ; get left operand back
    sub V0, V1                ; compute subtraction
    mov AX, V1                ; store indexer
    pop V0                    ; restore index obj
    mul V1, 1                 ; multiply indexer by 1
    add AX, V0                ; add indexer and index obj
    mov AX, V0                ; move to general purpose register
    mov [V0], [$721D]         ; int val = [V0]
    push [$721D]              ; left operand
    push 1                    ; left operand
    mov [$721B], V1           ; right operand
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
    mov 0, [$721F]            ; int sk = 0
_body_3:
    push [$721F]              ; left operand
    mov 256, V1               ; right operand
    pop V0                    ; get left operand back
    lt V0, V1                 ; compute less than check
    not LX
    bz
    jmp _end_4                ; if for condition fails, jump to end
    push [$721F]              ; arg int scanCode
    call _hid_getKeyState_int_scanCode; int hid_getKeyState(int scanCode)
    cmp V0, 0                 ; logical and
    not
    bz
    jmp _clause_5
    mov 0, V0                 ; first operand failed, store 0 and skip second
    jmp _end_6
_clause_5:
    push [$721F]              ; left operand
    mov 160, V1               ; right operand
    pop V0                    ; get left operand back
    cmp V0, V1                ; compute inequality check
    not
    cmp LX, 0                 ; compare result from second operand to 0
    not
    mov LX, V0                ; store second operand result
_end_6:
    not V0
    bz                        ; check if condition
    jmp _end_7                ; if succeeds, continue, else jump to end
    mov [$721F], V0           ; return value
    ret
_end_7:
    push [$721F]              ; i++
    inc [$721F]
    pop V0
    jmp _body_3
_end_4:
    mov 0, V0                 ; return value
    ret
    mov 0, V0                 ; fallback return 0
    ret

_hid_waitForPressedKey:       ; int hid_waitForPressedKey()
_body_8:
    not 1
    bz
    jmp _end_9                ; if while condition fails, jump to end
    call _hid_getPressedKey   ; int hid_getPressedKey()
    mov V0, [$7221]           ; int sk = V0
    push [$7221]              ; left operand
    mov 0, V1                 ; right operand
    pop V0                    ; get left operand back
    cmp V0, V1                ; compute inequality check
    not
    not LX
    bz                        ; check if condition
    jmp _end_10               ; if succeeds, continue, else jump to end
    mov [$7221], V0           ; return value
    ret
_end_10:
    jmp _body_8
_end_9:
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

_drawChar_int_ch__int_x__int_y__int_color:; int drawChar(int ch, int x, int y, int color)
    pop V0                    ; preserve function return address
    pop [$7223]               ; param int ch
    pop [$7225]               ; param int x
    pop [$7227]               ; param int y
    pop [$7229]               ; param int color
    push V0                   ; restore function return address
    ; adjust for jmp instruction
    push $1FFF                ; left operand
    mov 2, V1                 ; right operand
    pop V0                    ; get left operand back
    add V0, V1                ; compute addition
    mov AX, [$722B]           ; int fontStart = AX
    push [$7223]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    mul V0, V1                ; compute multiplication
    mov AX, [$722D]           ; int charStart = AX
    mov 0, [$722F]            ; int yOff = 0
_body_11:
    push [$722F]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    lt V0, V1                 ; compute less than check
    not LX
    bz
    jmp _end_12               ; if for condition fails, jump to end
    push [$722B]              ; store index obj
    push [$722D]              ; left operand
    mov [$722F], V1           ; right operand
    pop V0                    ; get left operand back
    add V0, V1                ; compute addition
    mov AX, V1                ; store indexer
    pop V0                    ; restore index obj
    mul V1, 1                 ; multiply indexer by 1
    add AX, V0                ; add indexer and index obj
    mov AX, V0                ; move to general purpose register
    mov [V0], [$7231]         ; int row = [V0]
    mov 0, [$7233]            ; int xOff = 0
_body_13:
    push [$7233]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    lt V0, V1                 ; compute less than check
    not LX
    bz
    jmp _end_14               ; if for condition fails, jump to end
    push [$7231]              ; left operand
    push 7                    ; left operand
    mov [$7233], V1           ; right operand
    pop V0                    ; get left operand back
    sub V0, V1                ; compute subtraction
    mov AX, V1                ; right operand
    pop V0                    ; get left operand back
    shr V0, V1                ; compute right shift
    push DX                   ; left operand
    mov 1, V1                 ; right operand
    pop V0                    ; get left operand back
    and V0, V1                ; compute and
    mov DX, [$7235]           ; int bit = DX
    bz [$7235]                ; check ternary condition
    jmp _then_15
    mov [$7229], V0           ; store ternary fail result
    jmp _end_16
_then_15:
    mov 8, V0                 ; store ternary success result
_end_16:
    mov V0, [$7237]           ; int pix = V0
    push [$7237]              ; arg int x
    push [$7227]              ; left operand
    mov [$722F], V1           ; right operand
    pop V0                    ; get left operand back
    add V0, V1                ; compute addition
    push AX                   ; arg int y
    push [$7225]              ; left operand
    mov [$7233], V1           ; right operand
    pop V0                    ; get left operand back
    add V0, V1                ; compute addition
    push AX                   ; arg int c
    call _screen_setPixel_int_x__int_y__int_c; int screen_setPixel(int x, int y, int c)
    push [$7233]              ; i++
    inc [$7233]
    pop V0
    jmp _body_13
_end_14:
    push [$722F]              ; i++
    inc [$722F]
    pop V0
    jmp _body_11
_end_12:
    mov 0, V0                 ; fallback return 0
    ret

_print_int*_text__int_len__int_x__int_y__int_color:; int print(int* text, int len, int x, int y, int color)
    pop V0                    ; preserve function return address
    pop [$7239]               ; param int* text
    pop [$723B]               ; param int len
    pop [$723D]               ; param int x
    pop [$723F]               ; param int y
    pop [$7241]               ; param int color
    push V0                   ; restore function return address
    mov 0, [$7243]            ; int i = 0
_body_17:
    push [$7243]              ; left operand
    mov [$723B], V1           ; right operand
    pop V0                    ; get left operand back
    lt V0, V1                 ; compute less than check
    not LX
    bz
    jmp _end_18               ; if for condition fails, jump to end
    push [$7239]              ; store index obj
    mov [$7243], V1           ; store indexer
    pop V0                    ; restore index obj
    mul V1, 2                 ; multiply indexer by 2
    add AX, V0                ; add indexer and index obj
    mov AX, V0                ; move to general purpose register
    mov [V0], [$7245]         ; int ch = [V0]
    push [$7241]              ; arg int ch
    push [$723F]              ; arg int x
    push [$723D]              ; left operand
    push [$7243]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    mul V0, V1                ; compute multiplication
    mov AX, V1                ; right operand
    pop V0                    ; get left operand back
    add V0, V1                ; compute addition
    push AX                   ; arg int y
    push [$7245]              ; arg int color
    call _drawChar_int_ch__int_x__int_y__int_color; int drawChar(int ch, int x, int y, int color)
    push [$7243]              ; i++
    inc [$7243]
    pop V0
    jmp _body_17
_end_18:
    mov 0, V0                 ; fallback return 0
    ret

_main:                        ; int main()
    mov 0, [$7247]            ; int x = 0
    mov 0, [$7249]            ; int y = 0
_body_19:
    not 1
    bz
    jmp _end_20               ; if while condition fails, jump to end
    push [$7247]              ; left operand
    mov 128, V1               ; right operand
    pop V0                    ; get left operand back
    cmp V0, V1                ; compute equality check
    bz
    jmp _end_21
    gt V0, V1
_end_21:
    not LX
    bz                        ; check if condition
    jmp _end_22               ; if succeeds, continue, else jump to end
    mov 0, [$7247]            ; [$7247] = 0
    push [$7249]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    add V0, V1                ; compute addition
    push AX                   ; computation results
    pop V1                    ; get computation results back
    mov V1, [$7249]           ; store back into left param
_end_22:
    push [$7249]              ; left operand
    mov 64, V1                ; right operand
    pop V0                    ; get left operand back
    cmp V0, V1                ; compute equality check
    bz
    jmp _end_23
    gt V0, V1
_end_23:
    not LX
    bz                        ; check if condition
    jmp _end_24               ; if succeeds, continue, else jump to end
    mov 0, [$7247]            ; [$7247] = 0
    mov 0, [$7249]            ; [$7249] = 0
    push 8                    ; arg int c
    call _screen_clear_int_c  ; int screen_clear(int c)
_end_24:
    call _hid_waitForPressedKey; int hid_waitForPressedKey()
    mov V0, [$724B]           ; int sk = V0
_body_25:
    push [$724B]              ; arg int scanCode
    call _hid_getKeyState_int_scanCode; int hid_getKeyState(int scanCode)
    not V0
    bz
    jmp _end_26               ; if while condition fails, jump to end
    jmp _body_25
_end_26:
    push [$724B]              ; left operand
    mov 13, V1                ; right operand
    pop V0                    ; get left operand back
    cmp V0, V1                ; compute equality check
    not LX
    bz                        ; check if condition
    jmp _end_27               ; if succeeds, continue, else jump to end
    push [$7249]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    add V0, V1                ; compute addition
    push AX                   ; computation results
    pop V1                    ; get computation results back
    mov V1, [$7249]           ; store back into left param
    mov 0, [$7247]            ; [$7247] = 0
    jmp _body_19              ; continue
_end_27:
    push [$724B]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    cmp V0, V1                ; compute equality check
    not LX
    bz                        ; check if condition
    jmp _end_28               ; if succeeds, continue, else jump to end
    push [$7247]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    sub V0, V1                ; compute subtraction
    push AX                   ; computation results
    pop V1                    ; get computation results back
    mov V1, [$7247]           ; store back into left param
    push [$7247]              ; left operand
    mov 0, V1                 ; right operand
    pop V0                    ; get left operand back
    lt V0, V1                 ; compute less than check
    not LX
    bz                        ; check if condition
    jmp _end_29               ; if succeeds, continue, else jump to end
    mov 0, [$7247]            ; [$7247] = 0
_end_29:
    push 1                    ; arg int ch
    push [$7249]              ; arg int x
    push [$7247]              ; arg int y
    push 32                   ; arg int color
    call _drawChar_int_ch__int_x__int_y__int_color; int drawChar(int ch, int x, int y, int color)
    jmp _body_19              ; continue
_end_28:
    push 160                  ; arg int scanCode
    call _hid_getKeyState_int_scanCode; int hid_getKeyState(int scanCode)
    not V0
    bz                        ; check if condition
    jmp _end_30               ; if succeeds, continue, else jump to end
    push [$724B]              ; left operand
    push 97                   ; left operand
    mov 65, V1                ; right operand
    pop V0                    ; get left operand back
    sub V0, V1                ; compute subtraction
    mov AX, V1                ; right operand
    pop V0                    ; get left operand back
    add V0, V1                ; compute addition
    push AX                   ; computation results
    pop V1                    ; get computation results back
    mov V1, [$724B]           ; store back into left param
_end_30:
    push 1                    ; arg int ch
    push [$7249]              ; arg int x
    push [$7247]              ; arg int y
    push [$724B]              ; arg int color
    call _drawChar_int_ch__int_x__int_y__int_color; int drawChar(int ch, int x, int y, int color)
    push [$7247]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    add V0, V1                ; compute addition
    push AX                   ; computation results
    pop V1                    ; get computation results back
    mov V1, [$7247]           ; store back into left param
    jmp _body_19
_end_20:
    mov 0, V0                 ; fallback return 0
    ret

