
LINKER = gcc
LINKERFLAGS = -m32 $(OUTPUT) bin/main.o -o bin\main.exe  # Only for fast tests

FORMAT = win32
FORMAT64 = win64

SOURCE = crt_entry.asm
SOURCE64 = crt_entry_x86_64.asm

OUTPUT = bin/crt32.o
OUTPUT64 = bin/crt64.o

FLAGS = -f $(FORMAT) $(SOURCE) -o $(OUTPUT)
FLAGS64 = -f $(FORMAT64) $(SOURCE64) -o $(OUTPUT64)

WIN32FLAGS = -DWINDOWS -f win32 $(SOURCE) -o bin/crt32.o
WIN64FLAGS = -f win64 $(SOURCE64) -o bin/crt64.o

all: crt32

wincrt: $(SOURCE)
	nasm $(WIN32FLAGS)

crt32: $(SOURCE)
	nasm $(FLAGS)

crt64: $(SOURCE64)
	nasm $(FLAGS64)

link: main.asm
	nasm -f $(FORMAT) main.asm -o bin/main.o
	$(LINKER) $(LINKERFLAGS)