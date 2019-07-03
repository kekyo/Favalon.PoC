
//==================================
Apply 1:
a b
(a:? b:?):?
1:-------------------
(a:? b:?):'1
(a:? b:'2):'1
(a:('2 -> '1) b:'2):'1
2:-------------------
3:-------------------
'1

//==================================
Apply 2:
a b:System.Int32
(a:? b:System.Int32):?
1:-------------------
(a:? b:System.Int32):'1
(a:(System.Int32 -> '1) b:System.Int32):'1
2:-------------------
3:-------------------
'1

//==================================
Apply 3:
a:(System.Int32 -> ?) b
(a:(System.Int32 -> ?) b:?):?
1:-------------------
(a:(System.Int32 -> ?) b:?):'1               : Hint('1)
(a:(System.Int32 -> ?) b:'2):'1              : Hint('2)
(a:(System.Int32 -> '1) b:'2):'1             : Hint('2 -> '1), Memoize('2 => System.Int32)
2:-------------------
(a:(System.Int32 -> '1) b:System.Int32):'1   : Update('2 => System.Int32)
3:-------------------
'1

//==================================
Apply 4:
a b c
((a:? b:?):? c:?):?
1:-------------------
((a:? b:?):? c:?):'1
((a:? b:?):? c:'2):'1
((a:? b:?):('2 -> '1) c:'2):'1
((a:? b:'3):('2 -> '1) c:'2):'1
((a:('3 -> ('2 -> '1)) b:'3):('2 -> '1) c:'2):'1
2:-------------------
3:-------------------
'1

//==================================
Apply 5:
a b c:System.Int32
((a:? b:?):? c:System.Int32):?
1:-------------------
((a:? b:?):? c:System.Int32):'1
((a:? b:?):(System.Int32 -> '1) c:System.Int32):'1
((a:? b:'2):(System.Int32 -> '1) c:System.Int32):'1
((a:('2 -> (System.Int32 -> '1)) b:'2):(System.Int32 -> '1) c:System.Int32):'1
2:-------------------
3:-------------------
'1

//==================================
Apply 6:
a b:System.Int32 c
((a:? b:System.Int32):? c:?):?
1:-------------------
((a:? b:System.Int32):? c:?):'1
((a:? b:System.Int32):? c:'2):'1
((a:? b:System.Int32):('2 -> '1) c:'2):'1
((a:(System.Int32 -> ('2 -> '1)) b:System.Int32):('2 -> '1) c:'2):'1
2:-------------------
3:-------------------
'1

//==================================
Apply 7:
a:(System.Int32 -> ?) b c
((a:(System.Int32 -> ?) b:?):? c:?):?
1:-------------------
((a:(System.Int32 -> ?) b:?):? c:?):'1                                 : Hint('1)
((a:(System.Int32 -> ?) b:?):? c:'2):'1                                : Hint('2)
((a:(System.Int32 -> ?) b:?):('2 -> '1) c:'2):'1                       : Hint(('2 -> '1))
((a:(System.Int32 -> ?) b:'3):('2 -> '1) c:'2):'1                      : Hint('3)
((a:(System.Int32 -> ('2 -> '1)) b:'3):('2 -> '1) c:'2):'1             : Hint('3 -> ('2 -> '1)), Memoize('3 => System.Int32)
2:-------------------
((a:(System.Int32 -> ('2 -> '1)) b:System.Int32):('2 -> '1) c:'2):'1   : Update('3 => System.Int32)
3:-------------------
'1

//==================================
Apply 8:
a (b c)
(a:? (b:? c:?):?):?
1:-------------------
(a:? (b:? c:?):?):'1
(a:? (b:? c:?):'2):'1
(a:('2 -> '1) (b:? c:?):'2):'1
(a:('2 -> '1) (b:? c:'3):'2):'1
(a:('2 -> '1) (b:('3 -> '2) c:'3):'2):'1
2:-------------------
3:-------------------
'1

//==================================
Apply 9:
(a (b c)):System.Int32
(a:? (b:? c:?):?):System.Int32
1:-------------------
(a:? (b:? c:?):'1):System.Int32
(a:('1 -> System.Int32) (b:? c:?):'1):System.Int32
(a:('1 -> System.Int32) (b:? c:'2):'1):System.Int32
(a:('1 -> System.Int32) (b:('2 -> '1) c:'2):'1):System.Int32
2:-------------------
3:-------------------
System.Int32

//==================================
Apply 10:
a (b c):System.Int32
(a:? (b:? c:?):System.Int32):?
1:-------------------
(a:? (b:? c:?):System.Int32):'1
(a:(System.Int32 -> '1) (b:? c:?):System.Int32):'1
(a:(System.Int32 -> '1) (b:? c:'2):System.Int32):'1
(a:(System.Int32 -> '1) (b:('2 -> System.Int32) c:'2):System.Int32):'1
2:-------------------
3:-------------------
'1

//==================================
Apply 11:
a (b c:System.Int32)
(a:? (b:? c:System.Int32):?):?
1:-------------------
(a:? (b:? c:System.Int32):?):'1
(a:? (b:? c:System.Int32):'2):'1
(a:('2 -> '1) (b:? c:System.Int32):'2):'1
(a:('2 -> '1) (b:(System.Int32 -> '2) c:System.Int32):'2):'1
2:-------------------
3:-------------------
'1

//==================================
Apply 12:
a (b:(System.Int32 -> ?) c)
(a:? (b:(System.Int32 -> ?) c:?):?):?
1:-------------------
(a:? (b:(System.Int32 -> ?) c:?):?):'1
(a:? (b:(System.Int32 -> ?) c:?):'2):'1
(a:('2 -> '1) (b:(System.Int32 -> ?) c:?):'2):'1
(a:('2 -> '1) (b:(System.Int32 -> ?) c:'3):'2):'1
(a:('2 -> '1) (b:(System.Int32 -> '2) c:'3):'2):'1                  : Memoize('3 => System.Int32)
2:-------------------
(a:('2 -> '1) (b:(System.Int32 -> '2) c:System.Int32):'2):'1        : Update('3 => System.Int32)
3:-------------------
'1

//==================================
Apply 13:
a:(System.Int32 -> ? -> ?) (b c)
(a:((System.Int32 -> ?) -> ?) (b:? c:?):?):?
1:-------------------
(a:((System.Int32 -> ?) -> ?) (b:? c:?):?):'1
(a:((System.Int32 -> ?) -> ?) (b:? c:?):'2):'1
(a:((System.Int32 -> '3) -> '1) (b:? c:?):'2):'1                    : Memoize('2 => (System.Int32 -> '3))
(a:((System.Int32 -> '3) -> '1) (b:? c:'4):'2):'1
(a:((System.Int32 -> '3) -> '1) (b:('4 -> '2) c:'4):'2):'1
2:-------------------
(a:((System.Int32 -> '3) -> '1) (b:('4 -> (System.Int32 -> '3)) c:'4):'2):'1                     : Update('2 => (System.Int32 -> '3))
(a:((System.Int32 -> '3) -> '1) (b:('4 -> (System.Int32 -> '3)) c:'4):(System.Int32 -> '3)):'1   : Update('2 => (System.Int32 -> '3))
3:-------------------
'1
