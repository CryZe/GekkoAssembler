# GekkoAssembler

[![Build Status](https://travis-ci.org/CryZe/GekkoAssembler.svg)](https://travis-ci.org/CryZe/GekkoAssembler) [![Build status](https://ci.appveyor.com/api/projects/status/gbs2ur6lfolmipxn?svg=true)](https://ci.appveyor.com/project/CryZe/gekkoassembler)

Assembles Gekko Assembly to an Action Replay Cheat Code.

## Examples

### Usage of CPU Instructions
```asm
0x80491A60:
.u32 32 ; Console X Coordinate
.u32 112 ; Console Y Coordinate

0x80491A80:
.u32 0x00000000 ; Console Background Color
.u8 1 ; Console Visible

0x80491A89:
.str "Stage: " ; Line 1 Text

0x80491AC6:
.u8 0xFF ; Line 2 Active
.str "Room: " ; Line 2 Text

0x80491B04:
.u8 0xFF ; Line 3 Active
.str "Entrance: " ; Line 3 Text

0x80491B42:
.u8 0xFF ; Line 4 Active
.str "Layer: " ; Line 4 Text

0x80491BBE:
.u8 0 ; No Line 5

0x803B7FF4:
.str "%X, %X, %X" ; Coordinate Format
.u16 0x0000 ; Null Terminator

0x803B8002:
.str "%d" ; Integer Format
.u16 0x0000 ; Null Terminator

0x8000645C:
bl 0x80A00000 ; Inject the function call

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

### Usage of Action Replay Instructions
```asm
; R + D-Pad Right to Load Earth Temple
0x803E0D2A:
!u16equal 0x0022 ; R + D-Pad Right
	0x803B8742:
	.u8 0x28 ; Has Medli on Boat
	0x803B8755:
	.u8 0x20 ; Medli in Earth Temple
	0x803B8759:
	.u8 1 ; Animation Set 2
	.u8 0x4 ; Medli in Earth Temple Entrance
	0x803B811B:
	.u8 16 ; Max Magic
	.u8 16 ; Magic
	0x803B814A:
	.u8 0x34 ; Has Deku Leaf
	0x803BD248:
	.str "M_Dai" ; Stage
	.u8 0 ; Null Terminator
	0x803BD250:
	.u16 0 ; Entrance
	.u8 0 ; Room
	.s8 -1 ; Layer Override
	.u8 1 ; Enable Warp
!end
```
