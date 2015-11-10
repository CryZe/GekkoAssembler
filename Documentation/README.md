# Gekko Assembler Documentation

<!-- TOC depth:6 withLinks:1 updateOnSave:1 orderedList:0 -->

- [Literals](#literals)
	- [Integer Literals](#integer-literals)
	- [Floating Point Literals](#floating-point-literals)
	- [String Literals](#string-literals)
- [Types](#types)
	- [Address Specifiers](#address-specifiers)
- [Gekko CPU Instructions](#gekko-cpu-instructions)
- [Data Sections](#data-sections)
- [Cheat Instructions](#cheat-instructions)
	- [Code Blocks and the End Tag](#code-blocks-and-the-end-tag)
	- [Equality Checks](#equality-checks)
	- [Mask Checks](#mask-checks)
	- [Inequality Checks](#inequality-checks)
	- [Less Than Checks](#less-than-checks)
	- [Greater Than Checks](#greater-than-checks)
	- [Setting Bits](#setting-bits)
	- [Unsetting Bits](#unsetting-bits)
	- [Adding Values](#adding-values)
	- [Repeating a Code Block](#repeating-a-code-block)

<!-- /TOC -->

The Gekko Assembler allows you to assemble Gekko Assembly into Action Replay or Gecko Cheat Codes.
This Document details the individual Instructions that are available to you.

There's 4 different Types of Instructions:
 1. Address Specifiers
 *  Gekko CPU Instructions
 *  Data Sections
 *  Cheat Instructions

## Literals

### Integer Literals

Integer Literals are either decimal integer numbers of the following form:
```
..., -11, -10, -9, ..., -3, -2, -1, 0, 1, 2, 3, ..., 9, 10, 11, ...
```
or they can be represented by hexadecimal numbers of the following form:
```
..., -0x11, -0x10, -0xF, ..., -0x3, -0x2, -0x1, 0x0, 0x1, 0x2, 0x3, ..., 0xF, 0x10, 0x11, ...
```

### Floating Point Literals

Floating Point Literals are real numbers of the following form:
```
<INTEGER>.<INTEGER>
```
They can't be hexadecimal numbers though.
Also, the fractional part can't be a negative number.

### String Literals

String Literals are enclosed by quotation marks and can be any string of characters.

## Types

This section describes the types that are available in the Gekko Assembler.

The following types are available:

Type | Width | Literals | Description
---- | ------ | ----- |-----
u8 | 8-bit | Integer | Unsigned Integer Byte
u16 | 16-bit | Integer | Unsigned Integer Halfword
u32 | 32-bit | Integer | Unsigned Integer Word
s8 | 8-bit | Integer | Signed Integer Byte
s16 | 16-bit | Integer | Signed Integer Halfword
s32 | 32-bit | Integer | Signed Integer Word
f32 | 32-bit | Floating Point | IEEE 754 Single
f64 | 64-bit | Floating Point | IEEE 754 Double
str | dynamic | String | String of characters

Note that Strings are not null terminated. If you want to null terminate them, append a u8 of 0 afterwards.

### Address Specifiers

```
<INTEGER>:
```

An *Address Specifier* sets the current Address to the integer specified.
Every line is being executed in the context of the current address.
Most Instructions increment the address automatically, so that the following instruction is being executed on the next address.

## Gekko CPU Instructions

The *Gekko CPU Instructions* are explained in the [IBM Gekko RISC Microprocessor User’s Manual](http://datasheets.chipdb.org/IBM/PowerPC/Gekko/gekko_user_manual.pdf).
Not every Instruction is supported yet, but the most common ones are.
CPU Instructions are always automatically aligned to 32-Bit.
Each CPU Instruction increments the current address by 32-Bit.

## Data Sections

```
.<TYPE> <VALUE>
```

A *Data Section* describes a constant being written to the current address.
The value given has to be of the type specified.
The address is automatically incremented by the width of the type.

## Cheat Instructions

This sections explains the individual *Cheat Instructions* available.
These Cheat Instructions are available through Action Replay or Gecko and often get executed immediately, while CPU Instructions only get executed if the Instruction Pointer of the CPU points to the address of the instruction.
Therefore it's often simpler to use Cheat Instructions instead.
They are less flexible though. Not all Cheat Instructions are perfectly supported by either Action Replay or Gecko, as their Instructions differ.
You'll get a warning if that's the case.

### Code Blocks and the End Tag

```
!end
```

If there's a *Code Block*, consisting of multiple lines, the end of the Code Block is designated by a line consisting of an *End Tag*.
The End Tag is optional if the end of the code is reached.

### Equality Checks

```
!<TYPE>equal <VALUE>
```

An *Equality Check* tests whether the value located at the current address equals the value specified.
If that's the case, the following Code Block is being executed, otherwise it's being skipped.

The following types are supported: ``u8, u16, u32, s8, s16, s32, f32``

### Mask Checks

```
!<TYPE>mask <VALUE>
```

A *Mask Check* tests whether the bits specified by the value of the Mask Check, are all set in the value located at the current address.
If that's the case, the following Code Block is being executed, otherwise it's being skipped.

The following types are supported: ``u8, u16``

### Inequality Checks

```
!<TYPE>unequal <VALUE>
```

An *Inequality Check* tests whether the value located at the current address is not equal to the value specified.
If that's the case, the following Code Block is being executed, otherwise it's being skipped.

The following types are supported: ``u8, u16, u32, s8, s16, s32, f32``

### Less Than Checks

```
!<TYPE>lessthan <VALUE>
```

A *Less Than Check* tests whether the value located at the current address is less than the value specified.
If that's the case, the following Code Block is being executed, otherwise it's being skipped.

The following types are supported: ``u8, u16, u32, s8, s16, s32, f32``

### Greater Than Checks

```
!<TYPE>greaterthan <VALUE>
```

A *Greater Than Check* tests whether the value located at the current address is greater than the value specified.
If that's the case, the following Code Block is being executed, otherwise it's being skipped.

The following types are supported: ``u8, u16, u32, s8, s16, s32, f32``

### Setting Bits

```
!<TYPE>bitset <VALUE>
```

You can set the Bits specified by the instruction's value at the current address to 1.

The following types are supported: ``u8, u16, u32``

### Unsetting Bits

```
!<TYPE>bitunset <VALUE>
```

You can set the Bits specified by the instruction's value at the current address to 0.

The following types are supported: ``u8, u16, u32``

### Adding Values

```
!<TYPE>add <VALUE>
```

The value specified by the instruction is being added to the value located at the current address.

The following types are supported: ``u8, u16, u32, s8, s16, s32, f32``

### Repeating a Code Block

```
!repeat <INTEGER>
```

This instruction repeats the following Code Block as often as the value of the instruction indicates.
This has the same effect as copying the Code Block that many times.
Therefore changes in the address are also kept between the individual iterations.
