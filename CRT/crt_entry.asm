
%define create_string(NAME, VALUE) NAME db VALUE, 0

section .data
create_string(integer_predicate, "%d")
create_string(float_predicate, "%f")

create_string(overflow_excreption, "Integer overflow! Dump core.")
create_string(platform_terminator_not_defined, "An exception was throwed. But platform terminator is undefined. Program not terminate.")

section .text

; Safe operations

; safe add
%macro sadd 2
    add %1, %2
    jo throw_overflow_exception
%endmacro

; IO   

%define UNDEFINED_TERMINATOR nop

%ifdef WINDOWS

    extern _ExitProcess@4
    
    %macro TERMINATOR 0 
        push 0
        call _ExitProcess@4 
    %endmacro
    
%elifdef LINUX

%else
    %define TERMINATOR UNDEFINED_TERMINATOR
%endif
   
extern _puts ; print operator with string
extern _printf ; print operator with floats | doubles | integers | boolean

; Exporting crt symbols
global print_int8
global print_int16
global print_int32
global print_string

global exit
global throw_exception
global throw_overflow_exception

; Terminating program
exit: 
    TERMINATOR

; eax - ptr to text
throw_exception:

%if %str(TERMINATOR) != %str(UNDEFINED_TERMINATOR)

    push eax
    call _puts
    
    jmp exit

%else

    push platform_terminator_not_defined
    call _puts
    
    xor eax, eax
    ret

%endif

throw_overflow_exception:

    mov eax, overflow_excreption
    jmp throw_exception

; implement print statement

print_int8:
    push ebp
    mov ebp, esp
    sub esp, 8
    
    mov [ebp-1], al
    movsx eax, byte [ebp-1]
    
    jo throw_overflow_exception
    
    push eax
    push integer_predicate
    call _printf
    
    leave
    
    xor eax, eax
    ret

print_int16:
    push ebp
    mov ebp, esp
    sub esp, 8
    
    mov [ebp-2], ax
    movsx eax, word [ebp-2]
    
    push eax
    push integer_predicate
    call _printf
    
    leave
    
    xor eax, eax
    ret

print_int32:
    push ebp
    mov ebp, esp
    
    push eax
    push integer_predicate
    call _printf
    
    leave
    
    xor eax, eax
    ret

; print(float):

; print(double):

print_string:
    push ebp
    mov ebp, esp
    
    push eax
    call _puts
    
    leave
    
    xor eax, eax
    ret