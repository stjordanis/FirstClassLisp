﻿(define nil '())
(define list (lambda x x))
(define car (lambda ((x . y)) x))
(define cdr (lambda ((x . y)) y))
(define nil? (lambda (()) #t _ #f))
(define pair? (lambda ((_ . _)) #t _ #f))
; Our let macro is like the one in arc - just
; a single variable, single expression, and no
; nesting.
(define let (macro
	      (lambda (var value body)
		     (list (list lambda (list var) body) value))))
; Y combinator allows us to write recursive code without
; mutating the environment.
(define Y 
    (lambda (m)
       (let z (lambda (f) (m (lambda (a) ((f f) a))))
	     (z z))))
; We could use the Y combinator here, but because we are defining
; 'length' using a define form, we can just recurse directly
(define length
        ; Here, we make use of the "pattern matching" in lambda
    (lambda
        (()) 0
        ((x . y)) (+ 1 (length y))))

; Now, let's implement simple non-nested quasiquote in terms of Lisp itself
; Using the builtin pattern matching of our lambda primitive makes
; this significantly simpler to implement.
(define expand-quasiquote
    (lambda
        (('unquote e))
            e
        ((('unquote-splicing x) . y))
            (list append x (expand-quasiquote y))
        ((x . y))
            (list cons (expand-quasiquote x) (expand-quasiquote y))
        x
            (cons quote x)))
(define quasiquote
    (macro expand-quasiquote))

(define fold-right
    (lambda (op initial ()) initial
            (op initial (x . y)) (fold-right op (op x initial) y)))
