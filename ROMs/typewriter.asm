    jmp _main
    .include "font.bin"
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

_drawChar_int_ch__int_x__int_y__int_color:; int drawChar(int ch, int x, int y, int color)
    pop V0                    ; preserve function return address
    pop [$7209]               ; param int ch
    pop [$720B]               ; param int x
    pop [$720D]               ; param int y
    pop [$720F]               ; param int color
    push V0                   ; restore function return address
    ; adjust for jmp instruction
    push $1FFF                ; left operand
    mov 2, V1                 ; right operand
    pop V0                    ; get left operand back
    add V0, V1                ; compute addition
    mov AX, [$7211]           ; int fontStart = AX
    push [$7209]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    mul V0, V1                ; compute multiplication
    mov AX, [$7213]           ; int charStart = AX
    mov 0, [$7215]            ; int yOff = 0
_body_1:
    push [$7215]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    lt V0, V1                 ; compute less than check
    not LX
    bz
    jmp _end_2                ; if for condition fails, jump to end
    push [$7211]              ; store index obj
    push [$7213]              ; left operand
    mov [$7215], V1           ; right operand
    pop V0                    ; get left operand back
    add V0, V1                ; compute addition
    mov AX, V1                ; store indexer
    pop V0                    ; restore index obj
    mul V1, 1                 ; multiply indexer by 1
    add AX, V0                ; add indexer and index obj
    mov AX, V0                ; move to general purpose register
    mov [V0], [$7217]         ; int row = [V0]
    mov 0, [$7219]            ; int xOff = 0
_body_3:
    push [$7219]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    lt V0, V1                 ; compute less than check
    not LX
    bz
    jmp _end_4                ; if for condition fails, jump to end
    push [$7217]              ; left operand
    push 7                    ; left operand
    mov [$7219], V1           ; right operand
    pop V0                    ; get left operand back
    sub V0, V1                ; compute subtraction
    mov AX, V1                ; right operand
    pop V0                    ; get left operand back
    shr V0, V1                ; compute right shift
    push DX                   ; left operand
    mov 1, V1                 ; right operand
    pop V0                    ; get left operand back
    and V0, V1                ; compute and
    mov DX, [$721B]           ; int bit = DX
    bz [$721B]                ; check ternary condition
    jmp _then_5
    mov [$720F], V0           ; store ternary fail result
    jmp _end_6
_then_5:
    mov 8, V0                 ; store ternary success result
_end_6:
    mov V0, [$721D]           ; int pix = V0
    push [$721D]              ; arg int x
    push [$720D]              ; left operand
    mov [$7215], V1           ; right operand
    pop V0                    ; get left operand back
    add V0, V1                ; compute addition
    push AX                   ; arg int y
    push [$720B]              ; left operand
    mov [$7219], V1           ; right operand
    pop V0                    ; get left operand back
    add V0, V1                ; compute addition
    push AX                   ; arg int c
    call _setPixel_int_x__int_y__int_c; int setPixel(int x, int y, int c)
    push [$7219]              ; i++
    inc [$7219]
    pop V0
    jmp _body_3
_end_4:
    push [$7215]              ; i++
    inc [$7215]
    pop V0
    jmp _body_1
_end_2:
    mov 0, V0                 ; fallback return 0
    ret

_clear_int_c:                 ; int clear(int c)
    pop V0                    ; preserve function return address
    pop [$721F]               ; param int c
    push V0                   ; restore function return address
    mov $402F, [$7221]        ; int screenStart = $402F
    push 64                   ; left operand
    mov 128, V1               ; right operand
    pop V0                    ; get left operand back
    mul V0, V1                ; compute multiplication
    mov AX, [$7223]           ; int screenSize = AX
    mov 0, [$7225]            ; int i = 0
_body_7:
    push [$7225]              ; left operand
    mov [$7223], V1           ; right operand
    pop V0                    ; get left operand back
    lt V0, V1                 ; compute less than check
    not LX
    bz
    jmp _end_8                ; if for condition fails, jump to end
    push [$7221]              ; store index obj
    mov [$7225], V1           ; store indexer
    pop V0                    ; restore index obj
    mul V1, 1                 ; multiply indexer by 1
    add AX, V0                ; add indexer and index obj
    mov AX, V0                ; move to general purpose register
    movb [$721F], [V0]        ; [V0] = [$721F]
    push [$7225]              ; i++
    inc [$7225]
    pop V0
    jmp _body_7
_end_8:
    mov 0, V0                 ; fallback return 0
    ret

_getKeyState_int_scanCode:    ; int getKeyState(int scanCode)
    pop V0                    ; preserve function return address
    pop [$7227]               ; param int scanCode
    push V0                   ; restore function return address
    mov $6033, [$7229]        ; int hid = $6033
    push [$7227]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    div V0, V1                ; compute division
    mov AX, [$722B]           ; int index = AX
    push [$7227]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    mod V0, V1                ; compute modulus
    mov AX, [$722D]           ; int bit = AX
    push [$7229]              ; store index obj
    push [$722B]              ; left operand
    mov 1, V1                 ; right operand
    pop V0                    ; get left operand back
    sub V0, V1                ; compute subtraction
    mov AX, V1                ; store indexer
    pop V0                    ; restore index obj
    mul V1, 1                 ; multiply indexer by 1
    add AX, V0                ; add indexer and index obj
    mov AX, V0                ; move to general purpose register
    mov [V0], [$722F]         ; int val = [V0]
    push [$722F]              ; left operand
    push 1                    ; left operand
    mov [$722D], V1           ; right operand
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

_print_int*_text__int_len__int_x__int_y__int_color:; int print(int* text, int len, int x, int y, int color)
    pop V0                    ; preserve function return address
    pop [$7231]               ; param int* text
    pop [$7233]               ; param int len
    pop [$7235]               ; param int x
    pop [$7237]               ; param int y
    pop [$7239]               ; param int color
    push V0                   ; restore function return address
    mov 0, [$723B]            ; int i = 0
_body_9:
    push [$723B]              ; left operand
    mov [$7233], V1           ; right operand
    pop V0                    ; get left operand back
    lt V0, V1                 ; compute less than check
    not LX
    bz
    jmp _end_10               ; if for condition fails, jump to end
    push [$7231]              ; store index obj
    mov [$723B], V1           ; store indexer
    pop V0                    ; restore index obj
    mul V1, 2                 ; multiply indexer by 2
    add AX, V0                ; add indexer and index obj
    mov AX, V0                ; move to general purpose register
    mov [V0], [$723D]         ; int ch = [V0]
    push [$7239]              ; arg int ch
    push [$7237]              ; arg int x
    push [$7235]              ; left operand
    push [$723B]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    mul V0, V1                ; compute multiplication
    mov AX, V1                ; right operand
    pop V0                    ; get left operand back
    add V0, V1                ; compute addition
    push AX                   ; arg int y
    push [$723D]              ; arg int color
    call _drawChar_int_ch__int_x__int_y__int_color; int drawChar(int ch, int x, int y, int color)
    push [$723B]              ; i++
    inc [$723B]
    pop V0
    jmp _body_9
_end_10:
    mov 0, V0                 ; fallback return 0
    ret

_main:                        ; int main()
    mov 0, [$723F]            ; int x = 0
    mov 0, [$7241]            ; int y = 0
_body_11:
    not 1
    bz
    jmp _end_12               ; if while condition fails, jump to end
    mov 0, [$7243]            ; int sk = 0
_body_13:
    push [$7243]              ; left operand
    mov 256, V1               ; right operand
    pop V0                    ; get left operand back
    lt V0, V1                 ; compute less than check
    not LX
    bz
    jmp _end_14               ; if for condition fails, jump to end
    push [$7243]              ; arg int scanCode
    call _getKeyState_int_scanCode; int getKeyState(int scanCode)
    not V0
    bz                        ; check if condition
    jmp _end_15               ; if succeeds, continue, else jump to end
_body_16:
    push [$7243]              ; arg int scanCode
    call _getKeyState_int_scanCode; int getKeyState(int scanCode)
    not V0
    bz
    jmp _end_17               ; if while condition fails, jump to end
    jmp _body_16
_end_17:
    ; Newline pressed
    push [$7243]              ; left operand
    mov 13, V1                ; right operand
    pop V0                    ; get left operand back
    cmp V0, V1                ; compute equality check
    not LX
    bz                        ; check if condition
    jmp _end_18               ; if succeeds, continue, else jump to end
    push [$7241]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    add V0, V1                ; compute addition
    push AX                   ; computation results
    pop V1                    ; get computation results back
    mov V1, [$7241]           ; store back into left param
    mov 0, [$723F]            ; [$723F] = 0
    jmp _body_13              ; continue
_end_18:
    ; Backspace pressed
    push [$7243]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    cmp V0, V1                ; compute equality check
    not LX
    bz                        ; check if condition
    jmp _end_19               ; if succeeds, continue, else jump to end
    push [$723F]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    sub V0, V1                ; compute subtraction
    push AX                   ; computation results
    pop V1                    ; get computation results back
    mov V1, [$723F]           ; store back into left param
    push [$723F]              ; left operand
    mov 0, V1                 ; right operand
    pop V0                    ; get left operand back
    lt V0, V1                 ; compute less than check
    not LX
    bz                        ; check if condition
    jmp _end_20               ; if succeeds, continue, else jump to end
    mov 0, [$723F]            ; [$723F] = 0
_end_20:
    mov 32, [$7247]
    mov $7247, [$7245]        ; int text = $7247
    push 1                    ; arg int* text
    push [$7241]              ; arg int len
    push [$723F]              ; arg int x
    push 1                    ; arg int y
    push [$7245]              ; arg int color
    call _print_int*_text__int_len__int_x__int_y__int_color; int print(int* text, int len, int x, int y, int color)
    jmp _body_13              ; continue
_end_19:
    mov [$7243], [$724B]
    mov $724B, [$7249]        ; int text = $724B
    push 1                    ; arg int* text
    push [$7241]              ; arg int len
    push [$723F]              ; arg int x
    push 1                    ; arg int y
    push [$7249]              ; arg int color
    call _print_int*_text__int_len__int_x__int_y__int_color; int print(int* text, int len, int x, int y, int color)
    push [$723F]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    add V0, V1                ; compute addition
    push AX                   ; computation results
    pop V1                    ; get computation results back
    mov V1, [$723F]           ; store back into left param
_end_15:
    push [$7243]              ; i++
    inc [$7243]
    pop V0
    jmp _body_13
_end_14:
    push [$723F]              ; left operand
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
    mov 0, [$723F]            ; [$723F] = 0
    push [$7241]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    add V0, V1                ; compute addition
    push AX                   ; computation results
    pop V1                    ; get computation results back
    mov V1, [$7241]           ; store back into left param
_end_22:
    push [$7241]              ; left operand
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
    mov 0, [$723F]            ; [$723F] = 0
    mov 0, [$7241]            ; [$7241] = 0
    push 8                    ; arg int c
    call _clear_int_c         ; int clear(int c)
_end_24:
    jmp _body_11
_end_12:
    mov 0, V0                 ; fallback return 0
    ret

