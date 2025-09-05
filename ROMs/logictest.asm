    jmp _main
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

_main:                        ; int main()
    ; Test values
    mov 0, [$7215]            ; int a = 0
    ; 170
    mov 0, [$7217]            ; int b = 0
    ; 204
    mov 0, [$7219]            ; int c = 0
    ; AND
    movb [$7215], [$7219]     ; [$7219] = [$7215]
    push [$7219]              ; left operand
    mov [$7217], V1           ; right operand
    pop V0                    ; get left operand back
    and V0, V1                ; compute and
    ; expect 0b10001000 = 136
    mov $7100, V0             ; move pointer into register for dereference
    mov [V0], V0              ; move pointer into register for dereference
    ; OR
    movb [$7215], [$7219]     ; [$7219] = [$7215]
    push [$7219]              ; left operand
    mov [$7217], V1           ; right operand
    pop V0                    ; get left operand back
    or V0, V1                 ; compute or
    ; expect 0b11101110 = 238
    mov $7101, V0             ; move pointer into register for dereference
    mov [V0], V0              ; move pointer into register for dereference
    ; XOR
    movb [$7215], [$7219]     ; [$7219] = [$7215]
    push [$7219]              ; left operand
    mov [$7217], V1           ; right operand
    pop V0                    ; get left operand back
    xor V0, V1                ; compute exclusive or
    ; expect 0b01100110 = 102
    mov $7102, V0             ; move pointer into register for dereference
    mov [V0], V0              ; move pointer into register for dereference
    ; NOT
    inv [$7215]
    movb DX, [$7219]          ; [$7219] = DX
    ; expect 0b01010101 = 85
    mov $7103, V0             ; move pointer into register for dereference
    mov [V0], V0              ; move pointer into register for dereference
    ; SHL
    movb [$7215], [$7219]     ; [$7219] = [$7215]
    push [$7219]              ; left operand
    mov 2, V1                 ; right operand
    pop V0                    ; get left operand back
    shl V0, V1                ; compute left shift
    ; expect 0b10101000 = 168
    mov $7104, V0             ; move pointer into register for dereference
    mov [V0], V0              ; move pointer into register for dereference
    ; SHR
    movb [$7215], [$7219]     ; [$7219] = [$7215]
    push [$7219]              ; left operand
    mov 2, V1                 ; right operand
    pop V0                    ; get left operand back
    shr V0, V1                ; compute right shift
    ; expect 0b00101010 = 42
    mov $7105, V0             ; move pointer into register for dereference
    mov [V0], V0              ; move pointer into register for dereference
    ; Program ends cleanly
    mov 0, V0                 ; return value
    ret
    mov 0, V0                 ; fallback return 0
    ret

