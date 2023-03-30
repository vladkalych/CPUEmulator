# CPUEmulator 💻

The project was created to understand how the processor works. Some details were missed 

## Instruction MOV

Instruction MOV for each bit of processor

| Description  |  Opcode |  Opcode   |  Opcode   | 
|--------------|---------|-----------|-----------| 
|		           |  8 bits |  16 bits  |  32 bits  |
| MOV reg reg  |  0x10   |   0x25	   |   0x40	   |
| MOV reg mem  |  0x11   |   0x26	   |   0x41	   |
| MOV mem reg  |  0x12   |   0x27	   |   0x42	   |
| MOV reg imm  |  0x13   |   0x28	   |   0x43	   |
| ADD reg reg  |  0x15   |   0x30	   |   0x45	   |
| ADD reg mem  |  0x16   |   0x31	   |   0x46	   |
| ADD reg imm  |  0x17   |   0x32	   |   0x47	   |
| SUB reg reg	 |  0x20   |   0x35	   |   0x50	   |
| SUB reg mem	 |  0x21   |   0x36	   |   0x51	   |
| SUB reg imm	 |  0x22   |   0x37	   |   0x52	   |

## Instructions

Other instructions

| Description | Opcode |
| -------- | ------ |
|Output - read text from	memory:	      |  0xF0  |	     	        
|Input - write text to memory:      |  0xF1  |
|Convert to ASCII from memory: |  0xF5  |
|Convert from ASCII from memory |  0xF6  |

## Registries

8, 16, 32-bits registries of CPU

| Registry | Address |
|----------|---------| 
|    EAX   |   0x20  |  
|    EBX   |   0x21  |  
|    ECX   |   0x22  |  
|    EDX   |   0x23  |  
|    AX    |   0x10  |
|    BX    |   0x11  |
|    CX    |   0x12  |
|    DX    |   0x13  |
|    AL    |   0x01  |
|    CL    |   0x02  |
|    DL    |   0x03  |
|    BL    |   0x04  |
|    AH    |   0x05  |   
|    CH    |   0x06  |
|    DH    |   0x07  |
|    BH    |   0x08  |

## How to use?

In main create CPU

```C#
CPUem cpu = new CPUem(new RAMem(1024)); // 1024 byte
```

And create array with instructions

```C#
byte[] instruction =
{ 
  0x43, 0x20, 0x00, 0x00, 0x00, 0x00,	// EAX <- 0x00 (address)
  0x43, 0x21, 0x02, 0x00, 0x00, 0x00,	// EBX <- 0x02 (length)
  0xF6,                                 // CALL 0xF5 (Convert from ASCII)
  0x43, 0x20, 0x02, 0x00, 0x00, 0x00,	// EAX <- 0x03 (address)
  0x43, 0x21, 0x02, 0x00, 0x00, 0x00,	// EBX <- 0x02 (length)
  0xF6,                                 // CALL 0xF5 (Convert from ASCII)
  0x11, 0x01, 0x00, 0x00, 0x00, 0x00,   // AL <- Memory[0x00000000]
  0x11, 0x02, 0x02, 0x00, 0x00, 0x00,	// CL <- Memory[0x00000002]
  0x15, 0x01, 0x02,                     // AL = AL + CL
  0x12, 0x00, 0x00, 0x00, 0x00, 0x01,   // Memory[0x00000000] <- AL
  0x43, 0x20, 0x00, 0x00, 0x00, 0x00,   // EAX <- 0x00 (address)
  0x43, 0x21, 0x01, 0x00, 0x00, 0x00,   // EBX <- 0x01 (length)
  0xF5,                                 // CALL 0xF5 (Convert to ASCII)
  0xF0                                  // CALL 0xF0 (Print to console)
};
```

You can write to RAM in advance

```C#
cpu.ram.Write(0x00000000, new byte[] { 0x31, 0x00, 0x32, 0x00 }); // ASCII: 1, EOL, 2, EOL
```
