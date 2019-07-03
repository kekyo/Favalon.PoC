//==================================
Lambda 1:
a -> a
(a:? -> a:?):?
1:-------------------
(a:? -> a:?):'1
(a:'2 -> a:?):'1                     : Bind(a:'2)
(a:'2 -> a:'2):'1                    : Lookup(a => '2), Memoize('1 => ('2 -> '2))
2:-------------------
(a:'2 -> a:'2):('2 -> '2)            : Update('1 => ('2 -> '2))
3:-------------------
'2 -> '2

//==================================
Lambda 2:
a -> a:System.Int32
(a:? -> a:System.Int32):?
1:-------------------
(a:? -> a:System.Int32):'1
(a:'2 -> a:System.Int32):'1          : Bind(a:'2)
(a:'2 -> a:System.Int32):'1          : Lookup(a => '2), Memoize('2 => System.Int32), Memoize('1 => ('2 -> System.Int32))
2:-------------------
(a:System.Int32 -> a:System.Int32):'1          : Update('2 => System.Int32)
(a:System.Int32 -> a:System.Int32):('2 -> System.Int32)          : Update('1 => ('2 -> System.Int32))
(a:System.Int32 -> a:System.Int32):(System.Int32 -> System.Int32)          : Update('2 => System.Int32)
3:-------------------
System.Int32 -> System.Int32

//==================================
Lambda 3:
a:System.Int32 -> a
(a:System.Int32 -> a:?):?
1:-------------------
(a:System.Int32 -> a:?):'1
(a:System.Int32 -> a:?):'1                      : Bind(a:System.Int32)
(a:System.Int32 -> a:System.Int32):'1           : Lookup(a => System.Int32), Memoize('1 => (System.Int32 -> System.Int32))
2:-------------------
(a:System.Int32 -> a:System.Int32):(System.Int32 -> System.Int32)           : Update('1 => (System.Int32 -> System.Int32))
3:-------------------
System.Int32 -> System.Int32

//==================================
Lambda 4:
a -> b -> a
(a:? -> (b:? -> a:?):?):?
1:-------------------
(a:? -> (b:? -> a:?):?):'1
(a:'2 -> (b:? -> a:?):?):'1                     : Bind(a:'2)
(a:'2 -> (b:? -> a:?):'3):'1                    : Memoize('1 => ('2 -> '3))
(a:'2 -> (b:'4 -> a:?):'3):'1                   : Bind(b:'4)
(a:'2 -> (b:'4 -> a:'2):'3):'1                  : Lookup(a => '2), Memoize('3 => ('4 -> '2))
2:-------------------
(a:'2 -> (b:'4 -> a:'2):('4 -> '2)):'1          : Update('3 => ('4 -> '2))
(a:'2 -> (b:'4 -> a:'2):('4 -> '2)):('2 -> '3)  : Update('1 => ('2 -> '3))
(a:'2 -> (b:'4 -> a:'2):('4 -> '2)):('2 -> ('4 -> '2))      : Update('3 => ('4 -> '2))
3:-------------------
'2 -> ('4 -> '2)
'2 -> '4 -> '2

//==================================
Lambda 5:
a -> b -> b
(a:? -> (b:? -> b:?):?):?
1:-------------------
(a:? -> (b:? -> b:?):?):'1
(a:'2 -> (b:? -> b:?):?):'1                     : Bind(a:'2)
(a:'2 -> (b:? -> b:?):'3):'1                    : Memoized('1 => ('2 -> '3))
(a:'2 -> (b:'4 -> b:?):'3):'1                   : Bind(b:'4)
(a:'2 -> (b:'4 -> b:'4):'3):'1                  : Lookup(b => '4), Memoized('3 => ('4 -> '4))
2:-------------------
(a:'2 -> (b:'4 -> b:'4):('4 -> '4)):'1          : Update('3 => ('4 -> '4))
(a:'2 -> (b:'4 -> b:'4):('4 -> '4)):('2 -> '3)  : Update('1 => ('2 -> '3))
(a:'2 -> (b:'4 -> b:'4):('4 -> '4)):('2 -> ('4 -> '4))      : Update('3 => ('4 -> '4))
3:-------------------
'2 -> ('4 -> '4)
'2 -> '4 -> '4

//==================================
Lambda 6:
a -> b -> a:System.Int32
(a:? -> (b:? -> a:System.Int32):?):?
1:-------------------
(a:? -> (b:? -> a:System.Int32):?):'1
(a:'2 -> (b:? -> a:System.Int32):?):'1          : Bind(a:'2)
(a:'2 -> (b:? -> a:System.Int32):'3):'1         : Memoized('1 => ('2 -> '3))
(a:'2 -> (b:'4 -> a:System.Int32):'3):'1        : Bind(b:'4)
(a:'2 -> (b:'4 -> a:System.Int32):'3):'1        : Lookup(a => '2), Memoized('2 => System.Int32)
(a:'2 -> (b:'4 -> a:System.Int32):'3):'1        : Memoized('3 => ('4 -> System.Int32))
2:-------------------
(a:'2 -> (b:'4 -> a:System.Int32):('4 -> System.Int32)):'1        : Update('3 => ('4 -> System.Int32))
(a:System.Int32 -> (b:'4 -> a:System.Int32):('4 -> System.Int32)):'1        : Update('2 => System.Int32)
(a:System.Int32 -> (b:'4 -> a:System.Int32):('4 -> System.Int32)):('2 -> '3)        : Update('1 => ('2 -> '3))
(a:System.Int32 -> (b:'4 -> a:System.Int32):('4 -> System.Int32)):(System.Int32 -> '3)        : Update('2 => System.Int32)
(a:System.Int32 -> (b:'4 -> a:System.Int32):('4 -> System.Int32)):(System.Int32 -> ('4 -> System.Int32))        : Update('3 => ('4 -> System.Int32))
3:-------------------
System.Int32 -> ('4 -> System.Int32)
System.Int32 -> '4 -> System.Int32

//==================================
Lambda 7:
a -> b:System.Int32 -> a
(a:? -> (b:System.Int32 -> a:?):?):?
1:-------------------
(a:? -> (b:System.Int32 -> a:?):?):'1
(a:'2 -> (b:System.Int32 -> a:?):?):'1           : Bind(a:'2)
(a:'2 -> (b:System.Int32 -> a:?):'3):'1          : Memoize('1 => ('2 -> '3))
(a:'2 -> (b:System.Int32 -> a:?):'3):'1          : Bind(b:System.Int32)
(a:'2 -> (b:System.Int32 -> a:'2):'3):'1         : Lookup(a => '2), Memoize('3 => (System.Int32 -> '2))
2:-------------------
(a:'2 -> (b:System.Int32 -> a:'2):(System.Int32 -> '2)):'1           : Update('3 => (System.Int32 -> '2))
(a:'2 -> (b:System.Int32 -> a:'2):(System.Int32 -> '2)):('2 -> '3)             : Update('1 => ('2 -> '3))
(a:'2 -> (b:System.Int32 -> a:'2):(System.Int32 -> '2)):('2 -> (System.Int32 -> '2))         : Update('3 => (System.Int32 -> '2))
3:-------------------
'2 -> (System.Int32 -> '2)
'2 -> System.Int32 -> '2

//==================================
Lambda 8:
a:System.Int32 -> b -> a
(a:System.Int32 -> (b:? -> a:?):?):?
1:-------------------
(a:System.Int32 -> (b:? -> a:?):?):'1
(a:System.Int32 -> (b:? -> a:?):?):'1             : Bind(a:System.Int32)
(a:System.Int32 -> (b:? -> a:?):'2):'1            : Memoize('1 => (System.Int32 -> '2))
(a:System.Int32 -> (b:'3 -> a:?):'2):'1           : Bind(b:'3)
(a:System.Int32 -> (b:'3 -> a:System.Int32):'2):'1           : Lookup(a:System.Int32), Memoize('2 => ('3 -> System.Int32))
2:-------------------
(a:System.Int32 -> (b:'3 -> a:System.Int32):('3 -> System.Int32):'1           : Update('2 => ('3 -> System.Int32))
(a:System.Int32 -> (b:'3 -> a:System.Int32):('3 -> System.Int32):(System.Int32 -> '2)           : Update('1 => (System.Int32 -> '2))
(a:System.Int32 -> (b:'3 -> a:System.Int32):('3 -> System.Int32):(System.Int32 -> ('3 -> System.Int32))           : Update('2 => ('3 -> System.Int32))
3:-------------------
System.Int32 -> ('3 -> System.Int32)
System.Int32 -> '3 -> System.Int32
