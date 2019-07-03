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
