# Gekko Assembler Reference

The Gekko Assembler allows you to assemble Gekko Assembly into Action Replay or Gecko Cheat Codes.
This Document details the individual Instructions that are available to you.

There's 4 different Types of Instructions:
 1. Address Specifiers
 *  Gekko CPU Instructions
 *  Data Sections
 *  Cheat Instructions

## Types

This section describes the types that are available in the Gekko Assembler.

The following types are available:

Type | Width | Description
---- | ------ | -----
u8 | 8-bit | Unsigned Integer Byte
u16 | 16-bit | Unsigned Integer Halfword
u32 | 32-bit | Unsigned Integer Word
s8 | 8-bit | Signed Integer Byte
s16 | 16-bit | Signed Integer Halfword
s32 | 32-bit | Signed Integer Word
f32 | 32-bit | IEEE 754 Single
f64 | 64-bit | IEEE 754 Double
str | dynamic | String of characters

Note that Strings are not null terminated. If you want to null terminate them, append a u8 of 0 afterwards.

## Address Specifiers

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
The address is automatically incremented by the width of the type

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

### Mask Checks

```
!<TYPE>mask <VALUE>
```

A *Mask Check* tests whether the bits specified by the value of the Mask Check, are all set in the value located at the current address.
If that's the case, the following Code Block is being executed, otherwise it's being skipped.

### Unequality Checks

```
!<TYPE>unequal <VALUE>
```

An *Unequality Check* tests whether the value located at the current address is not equal to the value specified.
If that's the case, the following Code Block is being executed, otherwise it's being skipped.
