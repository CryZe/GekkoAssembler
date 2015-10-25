# GekkoAssembler

[![Build Status](https://travis-ci.org/CryZe/GekkoAssembler.svg)](https://travis-ci.org/CryZe/GekkoAssembler)

Assembles Gekko Assembly to an Action Replay Cheat Code.

## Example

```asm
0x80491A60:
; Header Stuff
.u32 0x00000020
.u32 0x00000070

0x80491A80:
; More Header Stuff
.u32 0x00000000
.u8 1

0x80491A89:
; Line 1: Stage
.str "Stage: "

0x80491AC6:
; Line 2: Room
.u8 0xFF
.str "Room: "

0x80491B04:
; Line 3: Entrance
.u8 0xFF
.str "Entrance: "

0x80491B42:
; Line 4: Layer
.u8 0xFF
.str "Layer: "

0x80491BBE:
; No Line 5
.u8 0

0x803B7FF4:
; Coordinate Format
.str "%X, %X, %X"
.u16 0x0000

0x803B8002:
; Integer Format
.str "%d"
.u16 0x0000

0x8000645C:
; Inject the function call
bl 0x80A00000

0x80A00000:
stwu sp, -0x10 (sp)
mflr r0
stw r0, 0x14 (sp)

; String copy from 803BD23C to 80491A90
lis r3, 0x8049
addi r3, r3, 0x1A90
lis r4, 0x803B
addi r4, r4, 0x691E
addi r4, r4, 0x691E
bl 0x8032B6E0

; sprintf(80491ACD, "%d" /*803B8002*/, 803E9F48)
lis r5, 0x803E
addi r5, r5, 0x4FA4
addi r5, r5, 0x4FA4
lbz r5, 0x0 (r5)
lis r4, 0x803B
addi r4, r4, 0x4001
addi r4, r4, 0x4001
lis r3, 0x8049
addi r3, r3, 0x1ACD
crclr crb6
bl 0x80329308

; sprintf(80491B0F, "%d", 803B9244)
lis r5, 0x803B
addi r5, r5, 0x4922
addi r5, r5, 0x4922
lhz r5, 0x0 (r5)
lis r4, 0x803B
addi r4, r4, 0x4001
addi r4, r4, 0x4001
lis r3, 0x8049
addi r3, r3, 0x1B0F
crclr crb6
bl 0x80329308

; sprintf(80491B4A, "%d", 803B8000)
lis r5, 0x803B
addi r5, r5, 0x4000
addi r5, r5, 0x4000
lbz r5, 0x0 (r5)
lis r4, 0x803B
addi r4, r4, 0x4001
addi r4, r4, 0x4001
lis r3, 0x8049
addi r3, r3, 0x1B4A
crclr crb6
bl 0x80329308

; sprintf(80491B81, "%X, %X, %X", 803D78FC, 803D7900, 803D7904)
lis r7, 0x803D
addi r7, r7, 0x78FC
lwz r5, 0x0 (r7)
addi r7, r7, 0x0004
lwz r6, 0x0 (r7)
addi r7, r7, 0x0004
lwz r7, 0x0 (r7)
lis r4, 0x803B
addi r4, r4, 0x7FF4
lis r3, 0x8049
addi r3, r3, 0x1B81
crclr crb6
bl 0x80329308

lwz r0, 0x14 (sp)
mtlr r0
addi sp, sp, 16
blr
```
