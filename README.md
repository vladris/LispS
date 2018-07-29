Toy LISP dialect in C#

```
> (store fact (lambda n (if (eq n 1) 1 (* n (fact (- n 1))))))
fact
> (fact 5)
120
```

Run application for REPL.

### S-expressions
* Atom (Number | String | Name)
  * There are two predefined atoms: ``:true``, representing a true value; ``()``, representing a false value/empty list.
* List (S-expression S-expression)
  * Lambda (``(arguments)`` and ``(functions)`` plus captured context)

### Built-ins
* Arithmetic expressions ``+``, ``-``, ``*``, ``/`` .
  Example: ``(+ 1 2) => 3``.
  
* ``atom``. Example: ``(atom ()) => :true``, ``(atom (cons 1 2)) => ()``.
* ``car``, ``cdr``, ``cons``. Example ``(car (cons 1 (cons 2 3))) => 1``, ``(cdr (cons 1 (cons 2 3)) => (2 . 3)``.
* ``eq``. Example: ``(eq 1 1) => :true``, ``(eq 1 2) => ()``.
* Quote as ``'``. Example: ``'(+ 1 2) => (+ . (1 . 2))``.
* ``eval``. Example: ``(eval '(+ 1 2)) => 3``.
* ``if``. Example: ``(if (eq 1 2) "that's odd" "nope") => nope``.
* ``lambda`` defines a closure. Example: ``(lambda (x) (+ x 1)) => (x) => ((+ . (x . 1))``.
* ``store`` saves an s-expression with the given name in the environment. Example ``(store foo 42) => foo``.

Instead of providing a primitive for defining function, functions can be defined by storing lambdas, which names them:

```
> (store add1 (lambda x (+ x 1)))
add1
> (add1 1)
2
```

### Sources

| File | Description |
|-----------------|------------------|
| SExpressions.cs | S-expression types as described [above](#s-expressions). |
| Parser.cs       | Parser outputs an s-expression from source text. |
| Context.cs      | Context for symbol resolution. |
| Evaluator.cs    | S-expression evaluation logic. |
| Printer.cs      | Prints any s-expression. |
| Program.cs      | Implements REPL. |

### Other examples

Reverse list:
```
(store reverse (lambda l (if (atom l) l (cons (reverse (cdr l)) (car l)))))
(reverse (cons 1 (cons 2 (cons 3 (cons 4 ()))))
((((() . 4) . 3) . 2) . 1)
```

Map:
```
> (store add1 (lambda x (+ x 1)))
add1
> (store map (lambda (f l) (if (eq l ()) () (cons (f (car l)) (map f (cdr l))))))
map
> (map add1 (cons 1 (cons 2 (cons 3 (cons 4 ())))))
(2 . (3 . (4 . (5 . ()))))
```

Fold:
```
> (store fold (lambda (f z l) (if (eq l ()) z (f (car l) (fold f z (cdr l))))))
fold
> (store add (lambda (x y) (+ x y)))
add
> (fold add 0 (cons 1 (cons 2 (cons 3 (cons 4 ())))))
10
```
