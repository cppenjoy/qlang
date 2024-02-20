
%define create_string(NAME, VALUE) NAME db VALUE, 0

section .data
create_string(integer_predicate, "%d")
create_string(float_predicate, "%f")

section .text
   
; IO
extern puts ; print operator with string
extern printf ; print operator with floats | doubles | integers | boolean

; Exporting crt symbols
global print_int8
global print_int16
global print_int32
global print_string

; implement print statement

print_int8:
    push rbp
    mov rbp, rsp
    sub esp, 1
    
    mov [ebp-1], al
    movsx rax, byte [ebp-1]
    
    push rax
    push integer_predicate
    call printf
    
    leave
    
    xor eax, eax
    ret

print_int16:
    push rbp
    mov rbp, rsp
    
    push ax
    push integer_predicate
    call printf
    
    leave
    
    xor eax, eax
    ret

print_int32:
    push rbp
    mov rbp, rsp
    sub esp, 4
    
    mov [ebp-4], rax
    movsx rax, [ebp-4]
    
    push rax
    push integer_predicate
    call printf
    
    leave
    
    xor eax, eax
    ret

; print(float):

; print(double):

print_string:
    push rbp
    mov rbp, rsp
    
    mov edi, eax
    call puts
    
    mov rsp, rbp
    
    xor eax, eax
    pop rsp
    ret