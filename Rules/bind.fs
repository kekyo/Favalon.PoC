//==================================
Bind 1:
a = 123
(a:? = 123:?):?
1:-------------------
(a:? = 123:?):'1
(a:? = 123:Numeric):'1                       : Memoize('1 => Numeric)
(a:Numeric = 123:Numeric):'1
2:-------------------
(a:Numeric = 123:Numeric):Numeric            : Update('1 => Numeric)
3:-------------------
Numeric

//==================================
Bind 2:
a = b -> b
(a:? = (b:? -> b:?):?):?
1:-------------------
(a:? = (b:? -> b:?):?):'1
(a:? = (b:? -> b:?):'1):'1
(a:? = (b:'2 -> b:?):'1):'1                  : Bind(b:'2)
(a:? = (b:'2 -> b:'2):'1):'1                 : Lookup(b => 2), Memoize('1 => ('2 -> '2))
(a:'1 = (b:'2 -> b:'2):'1):'1
2:-------------------
(a:'1 = (b:'2 -> b:'2):('2 -> '2)):'1                        : Update('1 => ('2 -> '2))
(a:('2 -> '2) = (b:'2 -> b:'2):('2 -> '2)):'1                : Update('1 => ('2 -> '2))
(a:('2 -> '2) = (b:'2 -> b:'2):('2 -> '2)):('2 -> '2)        : Update('1 => ('2 -> '2))
3:-------------------
'2 -> '2

//==================================
Bind 3:
a = b -> b:System.Int32
(a:? = (b:? -> b:System.Int32):?):?
1:-------------------
(a:? = (b:? -> b:System.Int32):?):'1
(a:'1 = (b:? -> b:System.Int32):?):'1
(a:'1 = (b:? -> b:System.Int32):'1):'1
(a:'1 = (b:'2 -> b:System.Int32):'1):'1      : Bind(b:'2)
(a:'1 = (b:'2 -> b:System.Int32):'1):'1      : Lookup(b => '2), Memoize('2 => System.Int32)
2:-------------------
(a:'1 = (b:System.Int32 -> b:System.Int32):'1):'1      : Update('2 => System.Int32), Memoize('1 => (System.Int32 -> System.Int32))
(a:'1 = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):'1      : Update('1 => (System.Int32 -> System.Int32))
(a:(System.Int32 -> System.Int32) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):'1      : Update('1 => (System.Int32 -> System.Int32))
(a:(System.Int32 -> System.Int32) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):(System.Int32 -> System.Int32)      : Update('1 => (System.Int32 -> System.Int32))
3:-------------------
System.Int32 -> System.Int32

//==================================
Bind 4:
a = b:System.Int32 -> b
(a:? = (b:System.Int32 -> b:?):?):?
1:-------------------
(a:? = (b:System.Int32 -> b:?):?):'1
(a:'1 = (b:System.Int32 -> b:?):?):'1
(a:'1 = (b:System.Int32 -> b:?):'1):'1
(a:'1 = (b:System.Int32 -> b:?):'1):'1                   : Bind(b:System.Int32)
(a:'1 = (b:System.Int32 -> b:System.Int32):'1):'1        : Lookup(b => System.Int32), Memoize('1 => (System.Int32 -> System.Int32))
2:-------------------
(a:'1 = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):'1        : Update('1 => (System.Int32 -> System.Int32))
(a:(System.Int32 -> System.Int32) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):'1        : Update('1 => (System.Int32 -> System.Int32))
(a:(System.Int32 -> System.Int32) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):(System.Int32 -> System.Int32)        : Update('1 => (System.Int32 -> System.Int32))
3:-------------------
System.Int32 -> System.Int32

//==================================
Bind 5:
a:System.Int32 = b -> b
(a:System.Int32 = (b:? -> b:?):?):?
1:-------------------
(a:System.Int32 = (b:? -> b:?):?):'1
(a:System.Int32 = (b:? -> b:?):?):'1                : Memoize('1 => System.Int32)
(a:System.Int32 = (b:? -> b:?):System.Int32):'1
(a:System.Int32 = (b:'2 -> b:?):System.Int32):'1              : Bind(b:'2)
(a:System.Int32 = (b:'2 -> b:'2):System.Int32):'1             : Lookup(b => '2), // Unification problem (('2 -> '2) => System.Int32)

//==================================
Bind 6:
a:(System.Int32 -> ?) = b -> b
(a:(System.Int32 -> ?) = (b:? -> b:?):?):?
1:-------------------
(a:(System.Int32 -> ?) = (b:? -> b:?):?):'1
(a:(System.Int32 -> '2) = (b:? -> b:?):?):'1        : Memoize('1 => (System.Int32 -> '2))
(a:(System.Int32 -> '2) = (b:? -> b:?):(System.Int32 -> '2)):'1
(a:(System.Int32 -> '2) = (b:System.Int32 -> b:?):(System.Int32 -> '2)):'1      : Bind(b:System.Int32)
(a:(System.Int32 -> '2) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> '2)):'1     : Lookup(b => System.Int32), Memoize((System.Int32 -> '2) => (System.Int32 -> System.Int32))
2:-------------------
(a:(System.Int32 -> '2) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):'1     : Update((System.Int32 -> '2) => (System.Int32 -> System.Int32))
(a:(System.Int32 -> '2) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):(System.Int32 -> '2)       : Update('1 => (System.Int32 -> '2))
(a:(System.Int32 -> '2) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):(System.Int32 -> System.Int32)       : Update((System.Int32 -> '2) => (System.Int32 -> System.Int32))
3:-------------------
System.Int32 -> System.Int32

//==================================
RecBind 1:
rec a = b -> a
(a:? = (b:? -> a:?):?):?
1:-------------------
(a:? = (b:? -> a:?):?):'1
(a:'1 = (b:? -> a:?):?):'1                  : RecBind(a:'1)
(a:'1 = (b:? -> a:?):'1):'1
(a:'1 = (b:'2 -> a:?):'1):'1                : Bind(b:'2)
(a:'1 = (b:'2 -> a:'1):'1):'1               : Lookup(a => '1), Memoize('1 => ('2 -> '1))
2:-------------------
(a:'1 = (b:'2 -> a:'1):('2 -> '1)):'1       : Update('1 => ('2 -> '1))
(a:('2 -> '1) = (b:'2 -> a:'1):('2 -> '1)):'1       : Update('1 => ('2 -> '1))     // Recursive unification problem ('1 => ('2 -> '1))
(a:('2 -> '1) = (b:'2 -> a:'1):('2 -> '1)):('2 -> '1)       : Update('1 => ('2 -> '1))
3:-------------------
'2 -> '1
