﻿using System;
using System.Collections.Generic;
using System.Linq;
using LispEngine.Bootstrap;
using LispEngine.Datums;
using LispEngine.Evaluation;
using LispEngine.Lexing;
using LispEngine.Parsing;
using NUnit.Framework;
using Environment = LispEngine.Evaluation.Environment;

namespace LispTests.Evaluation
{
    [TestFixture]
    class EvaluatorTest : DatumHelpers
    {
        private readonly Environment env;

        public EvaluatorTest()
        {
            env = StandardEnvironment.Create().Extend("life", atom(42));
        }

        private void test(string sexp, Datum expected)
        {
            var e = new Evaluator();

            var datum = new Parser(Scanner.Create(sexp)).parse();
            var result = e.Evaluate(env, datum);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void testAtom()
        {
            test("23", atom(23));
        }

        [Test]
        public void testBoolAtom()
        {
            test("#t", atom(true));
            test("#f", atom(false));
        }

        [Test]
        public void testSymbol()
        {
            test("life", atom(42));
        }

        [Test]
        public void testDatumEnumerate()
        {
            var five = atom("5");
            var listfive = compound(five);
            var l = enumerate(listfive);
            Assert.AreEqual(1, l.Count());

            var listfivefive = compound(five, five);
            l = enumerate(listfivefive);
            Assert.AreEqual(2, l.Count());
        }

        [Test]
        public void testFunction()
        {
            test("((lambda (x) x) 5)", atom(5));
        }

        [Test]
        public void testConstantFunction()
        {
            test("((lambda () 6))", atom(6));
        }

        [Test]
        public void testLambdaList()
        {
            test("((lambda x x) 1 2 3)", atomList(1,2,3));
        }

        [Test]
        public void testList()
        {
            test("(list 1 2 3)", atomList(1, 2, 3));
        }

        [Test]
        public void testRecursiveFunctions()
        {
            test("(cons 3 (cons 4 5))", cons(atom(3), cons(atom(4), atom(5))));
        }

        [Test]
        public void testCons()
        {
            test("(cons 3 4)", cons(atom(3), atom(4)));
        }

        [Test]
        public void testApply()
        {
            test("(apply cons (list 3 4))", cons(atom(3), atom(4)));
        }

        [Test]
        public void testDotArgList()
        {
            test("((lambda (x . y) x) 4)", atom(4));
        }

        [Test]
        public void testCarList()
        {
            test("(car (list 4 5))", atom(4));
        }

        [Test]
        public void testCdrList()
        {
            test("(cdr (list 4 5))", atomList(5));
        }

        // Our lambda macro actually does full "structure" matching
        // on all of its arguments - this was actually as simple to
        // implement as the standard, which is a subset.
        // If then allowed multiple alternative argument lists
        // we could then support pattern matching.
        [Test]
        public void testStructuredMatching()
        {
            test("((lambda (a (b c)) c) 4 (list 5 6)))", atom(6));
            test("((lambda ((a b) c) b) (list 5 6) 4))", atom(6));
            test("((lambda (((a))) a) (list (list 5)))", atom(5));
        }

        [Test]
        public void testEq()
        {
            test("(eq? 4 4)", atom(true));
            test("(eq? 4 3)", atom(false));
        }

        [Test]
        public void testEqIsFlat()
        {
            // We don't want Eq to be doing deep comparisons
            test("(eq? (list 1 2) (list 1 2))", atom(false));
        }

        [Test]
        public void testIf()
        {
            test("(if #t 5 undefined)", atom(5));
            test("(if #f undefined 5)", atom(5));
        }

        [Test]
        public void testQuotedList()
        {
            test("'(3 4)", atomList(3, 4));
        }

        [Test]
        public void testQuotedAtom()
        {
            test("'3", atom(3));
        }

        [Test]
        public void testQuotedQuote()
        {
            test("''3", cons(quote, atom(3)));
        }

        [Test]
        public void testLet()
        {
            // We'll use "arc" let syntax because it's cleaner (http://ycombinator.com/arc/tut.txt)
            test("(let x 3 x)", atom(3));
        }

        [Test]
        public void testLetEvaluatesBody()
        {
            test("(let x 3 (eq? x 3))", atom(true));
        }
    }
}
