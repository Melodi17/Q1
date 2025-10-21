    jmp _main
    .include "font.bin"
_drawChar_int_ch__int_x__int_y__int_color:; int drawChar(int ch, int x, int y, int color)
    pop V0                    ; preserve function return address
    pop [$71FF]               ; param int ch
    pop [$7201]               ; param int x
    pop [$7203]               ; param int y
    pop [$7205]               ; param int color
    push V0                   ; restore function return address
    ; adjust for jmp instruction
    push $1FFF                ; left operand
    mov 2, V1                 ; right operand
    pop V0                    ; get left operand back
    add V0, V1                ; compute addition
    mov AX, [$7207]           ; int fontStart = AX
    push [$71FF]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    mul V0, V1                ; compute multiplication
    mov AX, [$7209]           ; int charStart = AX
    mov 0, [$720B]            ; int yOff = 0
_body_1:
    push [$720B]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    lt V0, V1                 ; compute less than check
    not LX
    bz
    jmp _end_2                ; if for condition fails, jump to end
    push [$7207]              ; store index obj
    push [$7209]              ; left operand
    mov [$720B], V1           ; right operand
    pop V0                    ; get left operand back
    add V0, V1                ; compute addition
    mov AX, V1                ; store indexer
    pop V0                    ; restore index obj
    mul V1, 1                 ; multiply indexer by 1
    add AX, V0                ; add indexer and index obj
    mov AX, V0                ; move to general purpose register
    mov [V0], [$720D]         ; int row = [V0]
    mov 0, [$720F]            ; int xOff = 0
_body_3:
    push [$720F]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    lt V0, V1                 ; compute less than check
    not LX
    bz
    jmp _end_4                ; if for condition fails, jump to end
    push [$720D]              ; left operand
    push 7                    ; left operand
    mov [$720F], V1           ; right operand
    pop V0                    ; get left operand back
    sub V0, V1                ; compute subtraction
    mov AX, V1                ; right operand
    pop V0                    ; get left operand back
    shr V0, V1                ; compute right shift
    push DX                   ; left operand
    mov 1, V1                 ; right operand
    pop V0                    ; get left operand back
    and V0, V1                ; compute and
    mov DX, [$7211]           ; int bit = DX
    bz [$7211]                ; check ternary condition
    jmp _then_5
    mov [$7205], V0           ; store ternary fail result
    jmp _end_6
_then_5:
    mov 0, V0                 ; store ternary success result
_end_6:
    mov V0, [$7213]           ; int pix = V0
    push [$7213]              ; arg int c
    push [$7203]              ; left operand
    mov [$720B], V1           ; right operand
    pop V0                    ; get left operand back
    add V0, V1                ; compute addition
    push AX                   ; arg int y
    push [$7201]              ; left operand
    mov [$720F], V1           ; right operand
    pop V0                    ; get left operand back
    add V0, V1                ; compute addition
    push AX                   ; arg int x
    call _setPixel_int_x__int_y__int_c; int setPixel(int x, int y, int c)
    push [$720F]              ; i++
    inc [$720F]
    pop V0
    jmp _body_3
_end_4:
    push [$720B]              ; i++
    inc [$720B]
    pop V0
    jmp _body_1
_end_2:
    mov 0, V0                 ; fallback return 0
    ret

_setPixel_int_x__int_y__int_c:; int setPixel(int x, int y, int c)
    pop V0                    ; preserve function return address
    pop [$7215]               ; param int x
    pop [$7217]               ; param int y
    pop [$7219]               ; param int c
    push V0                   ; restore function return address
    mov $402F, [$721B]        ; int screenStart = $402F
    push [$7217]              ; left operand
    mov 128, V1               ; right operand
    pop V0                    ; get left operand back
    mul V0, V1                ; compute multiplication
    push AX                   ; left operand
    mov [$7215], V1           ; right operand
    pop V0                    ; get left operand back
    add V0, V1                ; compute addition
    mov AX, [$721D]           ; int i = AX
    push [$721B]              ; store index obj
    mov [$721D], V1           ; store indexer
    pop V0                    ; restore index obj
    mul V1, 1                 ; multiply indexer by 1
    add AX, V0                ; add indexer and index obj
    mov AX, V0                ; move to general purpose register
    movb [$7219], [V0]        ; [V0] = [$7219]
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

_main:                        ; int main()
    mov 1, [$7227]            ; int color = 1
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
    mov $722B, [$7229]        ; int text = $722B
    mov 12, [$7243]           ; int len = 12
    push 0                    ; arg int c
    call _clear_int_c         ; int clear(int c)
_body_9:
    not 1
    bz
    jmp _end_10               ; if while condition fails, jump to end
    mov 0, [$7245]            ; int i = 0
_body_11:
    push [$7245]              ; left operand
    mov [$7243], V1           ; right operand
    pop V0                    ; get left operand back
    lt V0, V1                 ; compute less than check
    not LX
    bz
    jmp _end_12               ; if for condition fails, jump to end
    push [$7229]              ; store index obj
    mov [$7245], V1           ; store indexer
    pop V0                    ; restore index obj
    mul V1, 2                 ; multiply indexer by 2
    add AX, V0                ; add indexer and index obj
    mov AX, V0                ; move to general purpose register
    mov [V0], [$7247]         ; int ch = [V0]
    push [$7227]              ; arg int color
    push 0                    ; arg int y
    push [$7245]              ; left operand
    mov 8, V1                 ; right operand
    pop V0                    ; get left operand back
    mul V0, V1                ; compute multiplication
    push AX                   ; arg int x
    push [$7247]              ; arg int ch
    call _drawChar_int_ch__int_x__int_y__int_color; int drawChar(int ch, int x, int y, int color)
    push [$7245]              ; i++
    inc [$7245]
    pop V0
    jmp _body_11
_end_12:
    push [$7227]              ; i++
    inc [$7227]
    pop V0
    jmp _body_9
_end_10:
    mov 0, V0                 ; fallback return 0
    ret

