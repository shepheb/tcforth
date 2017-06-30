\ 0 - Intro
\ TC-FORTH Testing Disk
\ This is a collection of tests
\ intended to run on TC-FORTH
\ and verify it is working
\ correctly.
\ Load the main Forth system
\ first, then switch to this
\ disk and 1 LOAD.

\ 1 - Main loader
2  LOAD \ Test core
6  LOAD \ Core tests
37 LOAD \ Memory tests
41 LOAD \ Characters and Strings
45 LOAD \ Control structures

S" Done!" type

\ 2 - Test core
VARIABLE actual-depth
CREATE actual-results 32 allot
VARIABLE start-depth
VARIABLE xcursor
VARIABLE error-xt

3 LOAD
4 LOAD
5 LOAD

\ 3 - Error handlers
: ERROR error-xt @ EXECUTE ;
: EMPTY-STACK
  depth start-depth @ < IF
    depth start-depth @ swap
      DO 0 LOOP THEN
  depth start-depth @ > IF
    depth start-depth @
  DO drop LOOP THEN ;

: ERROR1 type source type cr
  debug empty-stack ;
' error1 error-xt !

\ 4 - Test words
: T{ depth start-depth !
  0 xcursor ! ;

: -> depth dup actual-depth !
  start-depth @ > IF
    depth start-depth @ - 0 DO
      actual-results i + ! LOOP
  THEN ;

\ 5 - Test words 2
: }T
  depth actual-depth @ = IF
    depth start-depth @ > IF
      depth start-depth @ - 0 DO
      actual-results i + @ <> IF
        S" INCORRECT RESULT:
        error LEAVE THEN LOOP
      THEN ELSE
  S" WRONG NUMBER OF RESULTS: "
  ERROR THEN ;

\ 6 - Core tests
7 LOAD \ Assumptions: numbers
HEX
8 LOAD \ 47 Boolean AND
9 LOAD \ 48 INVERT XOR OR
A LOAD \ 49 Small shifts
B LOAD \ 50 Big shifts
DECIMAL
12 LOAD \ Comparison
25 LOAD \ Stack ops
29 LOAD \ Arithmetic


\ 7 - Assumptions: numbers
T{ -> }T
T{ : bitsset? IF 0 0 ELSE 0
  THEN ; -> }T
T{  0 bitsset? -> 0 }T
T{  1 bitsset? -> 0 0 }T
T{ -1 bitsset? -> 0 0 }T

\ Helpers
 0 CONSTANT 0S
-1 CONSTANT 1S

\ 8 - Boolean AND
T{ 0 0 and -> 0 }T
T{ 0 1 and -> 0 }T
T{ 1 0 and -> 0 }T
T{ 1 1 and -> 1 }T

T{ 0 invert 1 and -> 1 }T
T{ 1 invert 1 and -> 0 }T

T{ 0S 0S AND -> 0S }T
T{ 0S 1S AND -> 0S }T
T{ 1S 0S AND -> 0S }T
T{ 1S 1S AND -> 1S }T


\ 9 - Booleans: INVERT OR XOR
T{ 0S invert -> 1S }T
T{ 1S invert -> 0S }T

T{ 0S 0S or -> 0S }T
T{ 0S 1S or -> 1S }T
T{ 1S 0S or -> 1S }T
T{ 1S 1S or -> 1S }T

T{ 0S 0S xor -> 0S }T
T{ 0S 1S xor -> 1S }T
T{ 1S 0S xor -> 1S }T
T{ 1S 1S xor -> 0S }T

\ 10 - Small shifts
1S 1 rshift invert CONSTANT MSB
T{ msb bitsset? -> 0 0 }T
\ 2*
T{   0S 2*       ->   0S }T
T{    1 2*       ->    2 }T
T{ 4000 2*       -> 8000 }T
T{   1S 2* 1 xor ->   1S }T
T{  MSB 2*       ->   0S }T
\ 2/
T{          0S 2/ ->   0S }T
T{           1 2/ ->    0 }T
T{        4000 2/ -> 2000 }T
T{          1S 2/ ->   1S }T
T{    1S 1 XOR 2/ ->   1S }T
T{ MSB 2/ MSB AND ->  MSB }T

\ 11 - Big shifts
\ RSHIFT
T{    1 0 rshift -> 1 }T
T{    1 1 rshift -> 0 }T
T{    2 1 rshift -> 1 }T
T{    4 2 rshift -> 1 }T
T{ 8000 F rshift -> 1 }T
T{  MSB 1 rshift MSB and -> 0 }T
T{  MSB 1 rshift   2*  -> MSB }T
\ LSHIFT
T{    1 0 lshift ->    1 }T
T{    1 1 lshift ->    2 }T
T{    1 2 lshift ->    4 }T
T{    1 F lshift -> 8000 }T
T{   1S 1 lshift 1 xor -> 1S }T
T{  MSB 1 lshift ->    0 }T

\ 12 - Comparison
13 LOAD \ Comparison helpers
14 LOAD \ 0= =
15 LOAD \ 0<  < part 1
16 LOAD \ < part 2
17 LOAD \ > part 1
18 LOAD \ > part 2
19 LOAD \ U<
20 LOAD \ MIN MAX

\ 13 - Comparison helpers
0 invert CONSTANT MAX-UINT
0 invert 1 rshift
  CONSTANT MAX-INT
0 invert 1 rshift invert
  CONSTANT MIN-INT
0 invert 1 rshift
  CONSTANT MID-UINT
0 invert 1 rshift invert
  CONSTANT MID-UINT+1

0S CONSTANT <FALSE>
1S CONSTANT <TRUE>

\ 14 - Comparisons: 0= =
T{  0       0= -> <TRUE>  }T
T{  1       0= -> <FALSE> }T
T{  2       0= -> <FALSE> }T
T{ -1       0= -> <FALSE> }T
T{ MAX-UINT 0= -> <FALSE> }T
T{ MIN-INT  0= -> <FALSE> }T
T{ MAX-INT  0= -> <FALSE> }T
T{  0  0 = -> <TRUE>  }T
T{  1  1 = -> <TRUE>  }T
T{ -1 -1 = -> <TRUE>  }T
T{  1  0 = -> <FALSE> }T
T{ -1  0 = -> <FALSE> }T
T{  0  1 = -> <FALSE> }T
T{  0 -1 = -> <FALSE> }T

\ 15 - Comparsion 0<, < part 1
T{       0 0< -> <FALSE> }T
T{      -1 0< -> <TRUE>  }T
T{ MIN-INT 0< -> <TRUE>  }T
T{       1 0< -> <FALSE> }T
T{ MAX-INT 0< -> <FALSE> }T

T{   0       1 < -> <TRUE>  }T
T{   1       2 < -> <TRUE>  }T
T{  -1       0 < -> <TRUE>  }T
T{  -1       1 < -> <TRUE>  }T
T{ MIN-INT     0 < -> <TRUE>  }T
T{ MIN-INT MAX-INT < ->
  <TRUE>  }T
T{     0 MAX-INT < -> <TRUE>  }T


\ 16 - Comparsion < part 2
T{     0       0 < -> <FALSE> }T
T{     1       1 < -> <FALSE> }T
T{     1       0 < -> <FALSE> }T
T{     2       1 < -> <FALSE> }T
T{     0      -1 < -> <FALSE> }T
T{     1      -1 < -> <FALSE> }T
T{     0 MIN-INT < -> <FALSE> }T
T{ MAX-INT     0 < -> <FALSE> }T
T{ MAX-INT MIN-INT < ->
  <FALSE> }T

\ 17 - Comparsion >  part 1
T{     0       1 > -> <FALSE> }T
T{     1       2 > -> <FALSE> }T
T{    -1       0 > -> <FALSE> }T
T{    -1       1 > -> <FALSE> }T
T{ MIN-INT     0 > -> <FALSE> }T
T{ MIN-INT MAX-INT > ->
  <FALSE> }T
T{     0 MAX-INT > -> <FALSE> }T

\ 18 - Comparsion >  part 2
T{     0       0 > -> <FALSE> }T
T{     1       1 > -> <FALSE> }T
T{     1       0 > -> <TRUE>  }T
T{     2       1 > -> <TRUE>  }T
T{     0      -1 > -> <TRUE>  }T
T{     1      -1 > -> <TRUE>  }T
T{     0 MIN-INT > -> <TRUE>  }T
T{ MAX-INT     0 > -> <TRUE>  }T
T{ MAX-INT MIN-INT > ->
  <TRUE>  }T


\ 19 - Comparsion U<
T{   0      1 U< -> <TRUE>  }T
T{   1      2 U< -> <TRUE>  }T
T{   0 MID-UINT U< -> <TRUE> }T
T{   0 MAX-UINT U< -> <TRUE> }T
T{ MID-UINT MAX-UINT U< ->
    <TRUE>  }T
T{   0      0 U< -> <FALSE> }T
T{   1      1 U< -> <FALSE> }T
T{   1      0 U< -> <FALSE> }T
T{   2      1 U< -> <FALSE> }T
T{ MID-UINT 0 U< -> <FALSE> }T
T{ MAX-UINT 0 U< -> <FALSE> }T
T{ MAX-UINT MID-UINT U< ->
    <FALSE> }T

\ 20 - MIN and MAX
21 LOAD \ MIN part 1
22 LOAD \ MIN part 2
23 LOAD \ MAX part 1
24 LOAD \ MAX part 2


\ 21 - MIN part 1
T{  0   1 MIN ->       0 }T
T{  1   2 MIN ->       1 }T
T{ -1   0 MIN ->      -1 }T
T{ -1   1 MIN ->      -1 }T
T{ MIN-INT  0 MIN -> MIN-INT }T
T{ MIN-INT MAX-INT MIN ->
    MIN-INT }T

\ 22 - MIN part 2
T{ 0 MAX-INT MIN ->       0 }T
T{ 0       0 MIN ->       0 }T
T{ 1       1 MIN ->       1 }T
T{ 1       0 MIN ->       0 }T
T{ 2       1 MIN ->       1 }T
T{ 0      -1 MIN ->      -1 }T
T{ 1      -1 MIN ->      -1 }T
T{ 0 MIN-INT MIN -> MIN-INT }T
T{ MAX-INT 0 MIN ->    0 }T
T{ MAX-INT MIN-INT MIN ->
    MIN-INT }T

\ 23 - MAX part 1
T{  0    1 MAX ->       1 }T
T{  1    2 MAX ->       2 }T
T{ -1    0 MAX ->       0 }T
T{ -1    1 MAX ->       1 }T
T{ MIN-INT  0 MAX ->       0 }T
T{  0 MAX-INT MAX -> MAX-INT }T
T{ MIN-INT MAX-INT MAX ->
  MAX-INT }T

\ 24 - MAX part 2
T{  0       0 MAX ->       0 }T
T{  1       1 MAX ->       1 }T
T{  1       0 MAX ->       1 }T
T{  2       1 MAX ->       2 }T
T{  0      -1 MAX ->       0 }T
T{  1      -1 MAX ->       1 }T
T{  0 MIN-INT MAX ->       0 }T
T{ MAX-INT  0 MAX -> MAX-INT }T
T{ MAX-INT MIN-INT MAX ->
  MAX-INT }T


\ 25 - Stack operations
26 LOAD \ Stack ops 1
27 LOAD \ Stack ops 2
28 LOAD \ Return stack

\ 26 - Stack ops 1
T{ 1 2 drop -> 1 }T
T{ 0   drop ->   }T
T{ 1 dup -> 1 1 }T
T{ 1 2 over -> 1 2 1 }T
T{ 1 2 3  rot -> 2 3 1 }T
T{ 1 2 3 -rot -> 3 1 2 }T
T{ 1 2 swap -> 2 1 }T
T{ 1 2 2drop -> }T
T{ 1 2 2dup -> 1 2 1 2 }T
T{ 1 2 3 4 2over ->
  1 2 3 4 1 2 }T
T{ 1 2 3 4 2swap -> 3 4 1 2 }T

\ 27 - Stack ops 2
T{ -1 ?dup -> -1 -1 }T
T{ 0  ?dup ->  0    }T
T{ 1  ?dup ->  1  1 }T

T{ 0 1 depth -> 0 1 2 }T
T{   0 depth -> 0 1   }T
T{     depth -> 0     }T

\ 28 - Return stack
T{ : GR1 >R R> ; -> }T
T{ : GR2 >R R@ R> DROP ; -> }T
T{ 123 GR1 -> 123 }T
T{ 123 GR2 -> 123 }T
T{  1S GR1 ->  1S }T


\ 29 - Arithmetic
30 LOAD \ +
31 LOAD \ -
32 LOAD \ 1+ 1-
33 LOAD \ ABS NEGATE
34 LOAD \ Multiplication 1
35 LOAD \ Multiplication 2
36 LOAD \ Division 1


\ 30 - +
T{     0  5 + ->      5 }T
T{     5  0 + ->      5 }T
T{     0 -5 + ->     -5 }T
T{    -5  0 + ->     -5 }T
T{     1  2 + ->      3 }T
T{     1 -2 + ->     -1 }T
T{    -1  2 + ->      1 }T
T{    -1 -2 + ->     -3 }T
T{    -1  1 + ->      0 }T
T{ MID-UINT 1 + -> MID-UINT+1 }T


\ 31 - -
T{      0  5 - ->   -5 }T
T{      5  0 - ->    5 }T
T{      0 -5 - ->    5 }T
T{     -5  0 - ->   -5 }T
T{      1  2 - ->   -1 }T
T{      1 -2 - ->    3 }T
T{     -1  2 - ->   -3 }T
T{     -1 -2 - ->    1 }T
T{      0  1 - ->   -1 }T
T{ MID-UINT+1 1 - -> MID-UINT }T


\ 32 - 1+ 1-
\ 1+
T{        0 1+ ->          1 }T
T{       -1 1+ ->          0 }T
T{        1 1+ ->          2 }T
T{ MID-UINT 1+ -> MID-UINT+1 }T
\ 1-
T{          2 1- ->        1 }T
T{          1 1- ->        0 }T
T{          0 1- ->       -1 }T
T{ MID-UINT+1 1- -> MID-UINT }T


\ 33 - ABS NEGATE
\ ABS
T{       0 ABS ->          0 }T
T{       1 ABS ->          1 }T
T{      -1 ABS ->          1 }T
T{ MIN-INT ABS -> MID-UINT+1 }T
\ NEGATE
T{  0 NEGATE ->  0 }T
T{  1 NEGATE -> -1 }T
T{ -1 NEGATE ->  1 }T
T{  2 NEGATE -> -2 }T
T{ -2 NEGATE ->  2 }T


\ 34 - Multiplication
T{  0  0 * ->  0 }T
T{  0  1 * ->  0 }T
T{  1  0 * ->  0 }T
T{  1  2 * ->  2 }T
T{  2  1 * ->  2 }T
T{  3  3 * ->  9 }T
T{ -3  3 * -> -9 }T
T{  3 -3 * -> -9 }T
T{ -3 -3 * ->  9 }T

\ 35 - Multiplication 2
T{ MID-UINT+1 1 RSHIFT 2 *
  -> MID-UINT+1 }T
T{ MID-UINT+1 2 RSHIFT 4 *
  -> MID-UINT+1 }T
T{ MID-UINT+1 1 RSHIFT
  MID-UINT+1 OR 2 * ->
  MID-UINT+1 }T

\ 36 - Division
T{   0   1  /MOD ->   0   0 }T
T{   1   1  /MOD ->   0   1 }T
T{   2   1  /MOD ->   0   2 }T
T{  -1   1  /MOD ->   0  -1 }T
T{  -2   1  /MOD ->   0  -2 }T
T{   0  -1  /MOD ->   0   0 }T
T{   7   3  /MOD ->   1   2 }T
T{  -7   3  /MOD ->  -1  -2 }T
T{   7  -3  /MOD ->   1  -2 }T
T{  -7  -3  /MOD ->  -1   2 }T




\ 37 - Memory
38 LOAD \ Basics
39 LOAD \ +! BITS
40 LOAD \ ALLOT


\ 38 - Memory basics
HERE 1 , HERE 2 ,
CONSTANT 2ND CONSTANT 1ST
T{       1ST 2ND U< -> <TRUE> }T
T{       1ST 1+  -> 2ND }T
T{   1ST 1 +  -> 2ND }T
T{     1ST @ 2ND @  -> 1 2 }T
T{         5 1ST !  ->     }T
T{     1ST @ 2ND @  -> 5 2 }T
T{         6 2ND !  ->     }T
T{     1ST @ 2ND @  -> 5 6 }T
T{           1ST 2@ -> 6 5 }T
T{       2 1 1ST 2! ->     }T
T{           1ST 2@ -> 2 1 }T
T{ 1S 1ST !  1ST @  -> 1S  }T


\ 39 - +!  BITS
T{ 0  1ST  !        ->   }T
T{ 1  1ST +!        ->   }T
T{    1ST  @        -> 1 }T
T{ -1 1ST +! 1ST @  -> 0 }T
: BITS ( X -- U )
   0 SWAP BEGIN DUP WHILE
     DUP MSB AND IF >R 1+ R>
     THEN 2* REPEAT DROP ;
T{ 1 1 <         -> <FALSE> }T
T{ 1 1 MOD ->    0    }T
T{ 1S BITS 10 <   -> <FALSE> }T

\ 40 - ALLOT
HERE 1 ALLOT
HERE
CONSTANT 2NDA
CONSTANT 1STA
T{ 1STA 2NDA U< -> <TRUE> }T
T{      1STA 1+ ->   2NDA }T


\ 41 - Characters and Strings
42 LOAD \ BL CHAR [CHAR]  [ ]
43 LOAD \ S"



\ 42 - BL CHAR [CHAR]  [ ]
T{ BL -> 32 }T
T{ CHAR X -> 88 }T
T{ CHAR HELLO -> 72 }T
T{ : GC1 [CHAR] X   ; -> }T
T{ : GC2 [CHAR] HELLO ; -> }T
T{ GC1 -> 88 }T
T{ GC2 -> 72 }T

\ 43 - S"
T{ : GC4 S" XY" ; ->   }T
T{ GC4 SWAP DROP  -> 2 }T
T{ GC4 DROP DUP @ SWAP 1+ @ ->
  88 89 }T


\ 45 - Control Flow
46 LOAD \ IF ELSE THEN
47 LOAD \ BEGIN WHILE REPEAT
48 LOAD \ BEGIN UNTIL
49 LOAD \ RECURSE 1
50 LOAD \ RECURSE 2



\ 46 - IF ELSE THEN
T{ : GI1 IF 123 THEN ; -> }T
T{ : GI1 IF 123 THEN ; -> }T
T{ : GI2 IF 123 ELSE 234 THEN ;
  -> }T
T{  0 GI1 ->     }T
T{  1 GI1 -> 123 }T
T{ -1 GI1 -> 123 }T
T{  0 GI2 -> 234 }T
T{  1 GI2 -> 123 }T
T{ -1 GI1 -> 123 }T
\ Multiple ELSEs in an IF
: melse IF 1 ELSE 2 ELSE 3 ELSE
  4 ELSE 5 THEN ;
T{ <FALSE> melse -> 2 4 }T
T{ <TRUE>  melse -> 1 3 5 }T

\ 47 - BEGIN WHILE REPEAT 1
T{ : GI3 BEGIN DUP 5 < WHILE
  DUP 1+ REPEAT ; -> }T
T{ 0 GI3 -> 0 1 2 3 4 5 }T
T{ 4 GI3 -> 4 5 }T
T{ 5 GI3 -> 5 }T
T{ 6 GI3 -> 6 }T
T{ : GI5 BEGIN DUP 2 > WHILE
    DUP 5 < WHILE DUP 1+ REPEAT
    123 ELSE 345 THEN ; -> }T
T{ 1 GI5 -> 1 345 }T
T{ 2 GI5 -> 2 345 }T
T{ 3 GI5 -> 3 4 5 123 }T
T{ 4 GI5 -> 4 5 123 }T
T{ 5 GI5 -> 5 123 }T

\ 48 - BEGIN UNTIL
T{ : GI4 BEGIN DUP 1+ DUP 5 >
  UNTIL ; -> }T
T{ 3 GI4 -> 3 4 5 6 }T
T{ 5 GI4 -> 5 6 }T
T{ 6 GI4 -> 6 7 }T

\ 49 - RECURSE part 1
T{ : GI6 ( N -- 0,1,..N )
     DUP IF DUP >R 1- RECURSE R>
     THEN ; -> }T
T{ 0 GI6 -> 0 }T
T{ 1 GI6 -> 0 1 }T
T{ 2 GI6 -> 0 1 2 }T
T{ 3 GI6 -> 0 1 2 3 }T
T{ 4 GI6 -> 0 1 2 3 4 }T
T{ :NONAME ( n -- 0, 1, .., n )
     DUP IF DUP >R 1- RECURSE R>
     THEN ; CONSTANT rn1 -> }T
T{ 0 rn1 EXECUTE -> 0 }T
T{ 4 rn1 EXECUTE -> 0 1 2 3 4 }T

